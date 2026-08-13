using Microsoft.EntityFrameworkCore;
using RequestForm.Data;
using RequestForm.Models.ViewModels;

namespace RequestForm.Services
{
    public static class ApprovalRemarksLookup
    {
        // APPROVED BY LIST FOR LEVELS OF APPROVALS
        public static async Task<ApprovalRemarksViewModel> GetApprovalRemarksAsync(
            this ApplicationDbContext context,
            int requestId)
        {
            var approvals = await context.RequestApprovals
                .Where(a => a.RequestId == requestId)
                .ToListAsync();

            return new ApprovalRemarksViewModel
            {
                HelpDeskRemarks = approvals
                    .FirstOrDefault(a => a.ApprovedBy == "Help Desk")
                    ?.Remarks,

                SupervisorRemarks = approvals
                    .FirstOrDefault(a => a.ApprovedBy == "Dummy Supervisor")
                    ?.Remarks,

                ManagerRemarks = approvals
                    .FirstOrDefault(a => a.ApprovedBy == "Dummy Manager")
                    ?.Remarks
            };
        }
    }
}
