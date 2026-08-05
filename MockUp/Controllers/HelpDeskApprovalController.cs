using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestForm.Data;

namespace RequestForm.Controllers
{
    public class HelpDeskApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HelpDeskApprovalController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _context.Requests
                .Include(r => r.RequestType)
                .Include(r => r.Status)
                .OrderByDescending(r => r.DateSubmitted)
                .ToListAsync();

            return View(requests);
        }
    }
}