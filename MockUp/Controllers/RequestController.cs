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
            ViewBag.Developers = FakeDevelopers.Developers;

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
            if (!ModelState.IsValid)
            {
                ViewBag.TicketTypes = await _context.TicketTypes.ToListAsync();
                ViewBag.Developers = FakeDevelopers.Developers;

                return View(request);
            }

            if (Attachment != null && Attachment.Length > 0)
            {
                var (isValid, error) = ValidateAttachment(Attachment);

                if (!isValid)
                {
                    ModelState.AddModelError("Attachment", error!);
                    ViewBag.TicketTypes = await _context.TicketTypes.ToListAsync();
                    ViewBag.Developers = FakeDevelopers.Developers;

                    return View(request);
                }

                request.AttachmentPath = await SaveAttachmentAsync(Attachment);
            }

            await _requestService.Create(request);

            if (!string.IsNullOrWhiteSpace(FeaturesJson))
            {
                try
                {
                    var features = System.Text.Json.JsonSerializer.Deserialize<
                        List<FeatureSubmissionDto>>(
                            FeaturesJson,
                            new System.Text.Json.JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                    if (features != null)
                    {
                        await _featureService.CreateBatchForRequest(
                            request.RequestId,
                            features);
                    }
                }
                catch (System.Text.Json.JsonException)
                {
                    // Malformed JSON from the client — the Request
                    // itself is already saved successfully, so we
                    // don't fail the whole submission over this.
                    // The requester can still add Features manually
                    // afterward from the Request Details page.
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