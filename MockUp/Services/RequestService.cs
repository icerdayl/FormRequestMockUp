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
            existing.StartDate = request.StartDate;
            existing.PreferredCompletionDate = request.PreferredCompletionDate;
            existing.AttachmentPath = request.AttachmentPath;

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

        public async Task<List<Request>> GetMyRequests(
    string? search,
    string? status)
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

            return await query
                .OrderByDescending(r => r.DateSubmitted)
                .ToListAsync();
        }
        public async Task<List<RequestType>> GetRequestTypes()
        {
            return await _context.RequestTypes
                .OrderBy(r => r.RequestTypeName)
                .ToListAsync();
        }

    }
}