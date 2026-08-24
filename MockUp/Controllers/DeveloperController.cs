using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Models;

namespace RequestForm.Controllers
{
    public class DeveloperController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISubTaskService _subTaskService;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedResultExtensions =
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg", ".zip"
        };

        private const long MaxResultSizeBytes = 10 * 1024 * 1024; // 10 MB

        public DeveloperController(
            ApplicationDbContext context,
            ISubTaskService subTaskService,
            IWebHostEnvironment env)
        {
            _context = context;
            _subTaskService = subTaskService;
            _env = env;
        }

        // ===========================
        // MY ASSIGNED REQUESTS
        // ===========================
        // No real login system yet, so "which developer" is picked
        // from a dropdown rather than an authenticated identity -
        // same mockup-auth approach used throughout this project.
        public async Task<IActionResult> Index(string? developer)
        {
            var developers = FakeDevelopers.Developers;
            var selected = string.IsNullOrWhiteSpace(developer)
                ? developers.First()
                : developer;

            ViewBag.Developers = developers;
            ViewBag.SelectedDeveloper = selected;

            // Help Desk assigns the whole request. Subtasks no longer
            // carry their own developer assignment.
            var requestIds = await _context.RequestAssignments
                .Where(a => a.AssignedTo == selected && a.IsCurrent)
                .Select(a => a.RequestId)
                .ToListAsync();

            var requests = await _context.Requests
                .Include(r => r.Status)
                .Include(r => r.TicketType)
                .Include(r => r.Features)
                    .ThenInclude(f => f.SubTasks)
                .Where(r => requestIds.Contains(r.RequestId))
                .OrderBy(r => r.PreferredCompletionDate)
                .ToListAsync();

            return View(requests);
        }

        // ===========================
        // REQUEST DETAIL - FEATURES & SUBTASKS
        // ===========================
        public async Task<IActionResult> Details(int id, string? developer)
        {
            var request = await _context.Requests
                .Include(r => r.Status)
                .Include(r => r.TicketType)
                .Include(r => r.RequestAssignments)
                .Include(r => r.Features)
                    .ThenInclude(f => f.SubTasks)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null)
                return NotFound();

            ViewBag.Developers = FakeDevelopers.Developers;
            ViewBag.SelectedDeveloper = developer;

            return View(request);
        }

        // ===========================
        // UPDATE SUBTASK STATUS
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSubTaskStatus(
            int subTaskId,
            int requestId,
            string status,
            string? completionRemarks,
            decimal? actualManDays,
            IFormFile? ResultFile,
            string? developer)
        {
            string? resultPath = null;

            if (ResultFile != null && ResultFile.Length > 0)
            {
                var extension = Path.GetExtension(ResultFile.FileName).ToLowerInvariant();

                if (!AllowedResultExtensions.Contains(extension))
                {
                    TempData["Error"] = "Unsupported result file type.";

                    return RedirectToAction(
                        nameof(Details),
                        new { id = requestId, developer });
                }

                if (ResultFile.Length > MaxResultSizeBytes)
                {
                    TempData["Error"] = "Result file is too large. Maximum size is 10 MB.";

                    return RedirectToAction(
                        nameof(Details),
                        new { id = requestId, developer });
                }

                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "subtask-results");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ResultFile.CopyToAsync(stream);
                }

                resultPath = $"/uploads/subtask-results/{uniqueFileName}";
            }

            await _subTaskService.UpdateStatus(
                subTaskId,
                status,
                completionRemarks,
                actualManDays,
                resultPath);

            return RedirectToAction(
                nameof(Details),
                new { id = requestId, developer });
        }
    }
}
