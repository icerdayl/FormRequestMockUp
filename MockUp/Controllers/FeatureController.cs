using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Models;

namespace RequestForm.Controllers
{
    public class FeatureController : Controller
    {
        private readonly IFeatureService _featureService;
        private readonly ApplicationDbContext _context;

        public FeatureController(
            IFeatureService featureService,
            ApplicationDbContext context)
        {
            _featureService = featureService;
            _context = context;
        }

        // ===========================
        // LIST / MANAGE FEATURES FOR A REQUEST
        // ===========================
        public async Task<IActionResult> Index(int requestId)
        {
            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null)
                return NotFound();

            ViewBag.Request = request;
            ViewBag.CommonFeatures = CommonFeatures.Templates;
            ViewBag.TotalFeaturesInDb = await _context.Features.CountAsync();

            var features = await _featureService.GetByRequestId(requestId);

            return View(features);
        }

        // ===========================
        // CREATE
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Feature feature)
        {
            if (string.IsNullOrWhiteSpace(feature.Title))
            {
                TempData["Error"] = "Title is required.";

                return RedirectToAction(
                    nameof(Index),
                    new { requestId = feature.RequestId });
            }

            await _featureService.Create(feature);

            return RedirectToAction(
                nameof(Index),
                new { requestId = feature.RequestId });
        }

        // ===========================
        // DELETE
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int requestId)
        {
            await _featureService.Delete(id);

            return RedirectToAction(nameof(Index), new { requestId });
        }
    }
}