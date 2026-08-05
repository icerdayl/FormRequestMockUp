using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Models;

namespace RequestForm.Controllers
{
    public class RequestController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly ApplicationDbContext _context;

        public RequestController(
            IRequestService requestService,
            ApplicationDbContext context)
        {
            _requestService = requestService;
            _context = context;
        }

        // ===========================
        // CREATE (GET)
        // ===========================

        public async Task<IActionResult> Create()
        {
            ViewBag.RequestTypes = await _context.RequestTypes.ToListAsync();

            return View(new Request
            {
                PreferredCompletionDate = DateTime.Today
            });
        }

        // ===========================
        // CREATE (POST)
        // ===========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Request request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RequestTypes = await _context.RequestTypes.ToListAsync();

                return View(request);
            }

            await _requestService.Create(request);

            return RedirectToAction(nameof(MyRequests));
        }

        public async Task<IActionResult> MyRequests(string search, string status)
        {
            var query = _context.Requests
                .Include(r => r.Status)
                .Include(r => r.RequestType)
                .AsQueryable();

            // Later, filter by logged-in user here
            // query = query.Where(r => r.RequestedBy == currentUser);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r =>
                    r.Title.Contains(search) ||
                    r.ReferenceNumber.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status) &&
                status != "All")
            {
                query = query.Where(r =>
                    r.Status!.StatusName == status);
            }

            var requests = await query
                .OrderByDescending(r => r.DateSubmitted)
                .ToListAsync();

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

            ViewBag.RequestTypes = await _context.RequestTypes.ToListAsync();

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Request request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RequestTypes = await _context.RequestTypes.ToListAsync();
                return View(request);
            }

            await _requestService.Update(request);

            return RedirectToAction(nameof(MyRequests));
        }
    }
}