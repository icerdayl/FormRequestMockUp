using Microsoft.EntityFrameworkCore;
using RequestForm.Data;
using RequestForm.Interfaces;
using RequestForm.Models;

namespace RequestForm.Services
{
    public class RequestService : IRequestService
    {
        private readonly ApplicationDbContext _context;

        public RequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Request>> GetAll()
        {
            return await _context.Requests
                .Include(r => r.RequestType)
                .Include(r => r.Status)
                .OrderByDescending(r => r.DateSubmitted)
                .ToListAsync();
        }

        public async Task<Request?> GetById(int id)
        {
            return await _context.Requests
                .Include(r => r.RequestType)
                .Include(r => r.Status)
                .FirstOrDefaultAsync(r => r.RequestId == id);
        }

        public async Task Create(Request request)
        {
            request.StatusId = 1;
            request.DateSubmitted = DateTime.Now;

            _context.Requests.Add(request);

            await _context.SaveChangesAsync();

            request.ReferenceNumber =
                $"UL-{DateTime.Now:yyyy}-{request.RequestId:D6}";

            await _context.SaveChangesAsync();
        }

        public async Task Update(Request request)
        {
            var existing = await _context.Requests.FindAsync(request.RequestId);

            if (existing == null)
                return;

            existing.Title = request.Title;
            existing.Description = request.Description;
            existing.RequestTypeId = request.RequestTypeId;
            existing.Department = request.Department;
            existing.Priority = request.Priority;
            existing.PreferredCompletionDate = request.PreferredCompletionDate;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
                return;

            _context.Requests.Remove(request);

            await _context.SaveChangesAsync();
        }
    }
}