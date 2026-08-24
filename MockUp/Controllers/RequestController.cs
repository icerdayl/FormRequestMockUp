using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Models;
using RequestForm.Services;

namespace RequestForm.Controllers
{
    public class RequestController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly IFeatureService _featureService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedAttachmentExtensions =
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg"
        };

        private const long MaxAttachmentSizeBytes = 10 * 1024 * 1024; // 10 MB

        public RequestController(
            IRequestService requestService,
            IFeatureService featureService,
            ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _requestService = requestService;
            _featureService = featureService;
            _context = context;
            _env = env;
        }

        // ===========================
        // ATTACHMENT HELPERS
        // ===========================

        private (bool IsValid, string? Error) ValidateAttachment(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedAttachmentExtensions.Contains(extension))
            {
                return (false,
                    "Unsupported file type. Allowed types: PDF, Word, Excel, PNG, JPG.");
            }

            if (file.Length > MaxAttachmentSizeBytes)
            {
                return (false, "File is too large. Maximum size is 10 MB.");
            }

            return (true, null);
        }

        private async Task<string> SaveAttachmentAsync(IFormFile file)
        {
            var uploadsFolder =
                Path.Combine(_env.WebRootPath, "uploads", "requests");

            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName =
                $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/requests/{uniqueFileName}";
        }

        private void DeletePhysicalAttachment(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var fullPath = Path.Combine(
                _env.WebRootPath,
                relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        // ===========================
        // CREATE (GET)
        // ===========================

        public async Task<IActionResult> Create()
        {
            ViewBag.TicketTypes = await _context.TicketTypes.ToListAsync();
            return View(new Request
            {
                StartDate = DateTime.Today,
                PreferredCompletionDate = DateTime.Today
            });
        }

        // ===========================
        // CREATE (POST)
        // ===========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Request request,
            IFormFile? Attachment,
            string? FeaturesJson)
        {
            var today = DateTime.Today;

            // Request-level date validation must happen on the server too.
            if (!request.StartDate.HasValue)
            {
                ModelState.AddModelError(
                    nameof(request.StartDate),
                    "Start date is required.");
            }
            else if (request.StartDate.Value.Date < today)
            {
                ModelState.AddModelError(
                    nameof(request.StartDate),
                    "Start date cannot be earlier than today.");
            }

            if (request.PreferredCompletionDate.Date < today)
            {
                ModelState.AddModelError(
                    nameof(request.PreferredCompletionDate),
                    "Completion date cannot be earlier than today.");
            }

            if (request.StartDate.HasValue &&
                request.PreferredCompletionDate.Date < request.StartDate.Value.Date)
            {
                ModelState.AddModelError(
                    nameof(request.PreferredCompletionDate),
                    "Completion date cannot be earlier than the start date.");
            }

            List<FeatureSubmissionDto>? features = null;

            // Parse and validate Features/Subtasks BEFORE creating the Request.
            // This prevents an invalid subtask date from creating a partial request.
            if (!string.IsNullOrWhiteSpace(FeaturesJson))
            {
                try
                {
                    features = System.Text.Json.JsonSerializer.Deserialize<
                        List<FeatureSubmissionDto>>(
                            FeaturesJson,
                            new System.Text.Json.JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                    if (features != null)
                    {
                        var subTaskDueDates = new List<DateTime>();

                        foreach (var feature in features)
                        {
                            foreach (var subTask in feature.SubTasks)
                            {
                                if (string.IsNullOrWhiteSpace(subTask.Title))
                                    continue;

                                // Every actual subtask needs both dates so its
                                // man-days can be calculated automatically.
                                if (!subTask.StartDate.HasValue || !subTask.DueDate.HasValue)
                                {
                                    ModelState.AddModelError(
                                        "",
                                        $"Subtask '{subTask.Title}' must have both a start date and a due date.");
                                    continue;
                                }

                                if (subTask.StartDate.Value.Date < today)
                                {
                                    ModelState.AddModelError(
                                        "",
                                        $"Subtask '{subTask.Title}' cannot have a start date earlier than today.");
                                }

                                if (subTask.DueDate.Value.Date < today)
                                {
                                    ModelState.AddModelError(
                                        "",
                                        $"Subtask '{subTask.Title}' cannot have a due date earlier than today.");
                                }

                                if (subTask.DueDate.Value.Date < subTask.StartDate.Value.Date)
                                {
                                    ModelState.AddModelError(
                                        "",
                                        $"Subtask '{subTask.Title}' cannot have a due date earlier than its start date.");
                                }

                                if (request.StartDate.HasValue &&
                                    subTask.StartDate.Value.Date < request.StartDate.Value.Date)
                                {
                                    ModelState.AddModelError(
                                        "",
                                        $"Subtask '{subTask.Title}' cannot start before the request start date.");
                                }

                                subTaskDueDates.Add(subTask.DueDate.Value.Date);

                                // Man-days are calculated from the date range on the
                                // server; ignore any client-supplied value.
                                subTask.EstimatedManDays =
                                    (decimal)(subTask.DueDate.Value.Date - subTask.StartDate.Value.Date).TotalDays + 1m;
                            }
                        }

                        if (subTaskDueDates.Count > 0)
                        {
                            var latestSubTaskDueDate = subTaskDueDates.Max();

                            if (latestSubTaskDueDate != request.PreferredCompletionDate.Date)
                            {
                                ModelState.AddModelError(
                                    nameof(request.PreferredCompletionDate),
                                    $"The request Completion Date must match the latest subtask due date ({latestSubTaskDueDate:MMMM dd, yyyy}).");
                            }
                        }
                    }
                }
                catch (System.Text.Json.JsonException)
                {
                    ModelState.AddModelError(
                        "",
                        "The Features & Subtasks data is invalid. Please review the selected features and try again.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TicketTypes = await _context.TicketTypes.ToListAsync();
                ViewBag.InitialFeatures = features;
                return View(request);
            }

            if (Attachment != null && Attachment.Length > 0)
            {
                var (isValid, error) = ValidateAttachment(Attachment);

                if (!isValid)
                {
                    ModelState.AddModelError("Attachment", error!);
                    ViewBag.TicketTypes = await _context.TicketTypes.ToListAsync();
                    ViewBag.InitialFeatures = features;
                    return View(request);
                }

                request.AttachmentPath = await SaveAttachmentAsync(Attachment);
            }

            await _requestService.Create(request);

            if (features != null && features.Count > 0)
            {
                try
                {
                    await _featureService.CreateBatchForRequest(
                        request.RequestId,
                        features);

                    TempData["FeaturesSuccess"] =
                        $"Request created with {features.Count} feature(s).";
                }
                catch (Exception ex)
                {
                    TempData["FeaturesWarning"] =
                        "Request saved, but saving the Features & Subtasks failed: " +
                        ex.Message +
                        " You can add them manually from the Request Details page.";
                }
            }

            return RedirectToAction(nameof(MyRequests));
        }

        public async Task<IActionResult> MyRequests(string search, string status)
        {
            var requests = await _requestService.GetMyRequests(search, status);

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(requests);
        }

        // ===========================
        // DELETE (GET)
        // ===========================
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _requestService.GetById(id);

            if (request == null)
                return NotFound();

            return View(request);
        }

        // ===========================
        // DELETE (POST)
        // ===========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _requestService.GetById(id);

            if (request != null)
            {
                DeletePhysicalAttachment(request.AttachmentPath);
            }

            await _requestService.Delete(id);

            return RedirectToAction(nameof(MyRequests));
        }

        // ===========================
        // VIEW DETAILS
        // ===========================
        public async Task<IActionResult> Details(int id)
        {
            var request = await _requestService.GetById(id);

            if (request == null)
                return NotFound();

            ViewBag.Remarks = await _context.GetApprovalRemarksAsync(id);

            return View(request);
        }

        // ===========================
        // EDIT DETAILS
        // ===========================
        public async Task<IActionResult> Edit(int id)
        {
            var request = await _requestService.GetById(id);

            if (request == null)
                return NotFound();

            ViewBag.TicketTypes = await _context.TicketTypes.ToListAsync();

            return View(request);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Request request, IFormFile? Attachment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TicketTypes = await _context.TicketTypes.ToListAsync();
                return View(request);
            }

            var existing = await _requestService.GetById(request.RequestId);

            if (Attachment != null && Attachment.Length > 0)
            {
                var (isValid, error) = ValidateAttachment(Attachment);

                if (!isValid)
                {
                    ModelState.AddModelError("Attachment", error!);
                    ViewBag.TicketTypes = await _context.TicketTypes.ToListAsync();

                    return View(request);
                }

                DeletePhysicalAttachment(existing?.AttachmentPath);

                request.AttachmentPath = await SaveAttachmentAsync(Attachment);
            }
            else
            {
                // No new file selected — keep whatever attachment
                // (if any) already exists on the record.
                request.AttachmentPath = existing?.AttachmentPath;
            }

            await _requestService.Update(request);

            return RedirectToAction(nameof(MyRequests));
        }
    }
}