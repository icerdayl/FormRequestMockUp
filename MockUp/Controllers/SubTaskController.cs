using Microsoft.AspNetCore.Mvc;
using RequestForm.Interfaces;
using RequestForm.Models;

namespace RequestForm.Controllers
{
    public class SubTaskController : Controller
    {
        private readonly ISubTaskService _subTaskService;
        private readonly IFeatureService _featureService;

        public SubTaskController(
            ISubTaskService subTaskService,
            IFeatureService featureService)
        {
            _subTaskService = subTaskService;
            _featureService = featureService;
        }

        // ===========================
        // CHECKLIST FOR A FEATURE
        // ===========================
        public async Task<IActionResult> Index(int featureId)
        {
            var feature = await _featureService.GetById(featureId);

            if (feature == null)
                return NotFound();

            ViewBag.Feature = feature;
            ViewBag.Developers = FakeDevelopers.Developers;

            var subTasks = await _subTaskService.GetByFeatureId(featureId);

            return View(subTasks);
        }

        // ===========================
        // CREATE
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubTask subTask)
        {
            if (string.IsNullOrWhiteSpace(subTask.Title))
            {
                TempData["Error"] = "Title is required.";

                return RedirectToAction(
                    nameof(Index),
                    new { featureId = subTask.FeatureId });
            }

            await _subTaskService.Create(subTask);

            return RedirectToAction(
                nameof(Index),
                new { featureId = subTask.FeatureId });
        }

        // ===========================
        // TOGGLE DONE (the checklist checkbox)
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleDone(int id, int featureId)
        {
            await _subTaskService.ToggleDone(id);

            return RedirectToAction(nameof(Index), new { featureId });
        }

        // ===========================
        // DELETE
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int featureId)
        {
            await _subTaskService.Delete(id);

            return RedirectToAction(nameof(Index), new { featureId });
        }
    }
}