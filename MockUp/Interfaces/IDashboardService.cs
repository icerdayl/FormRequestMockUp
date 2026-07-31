using RequestForm.Models.ViewModels;

namespace RequestForm.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboard();
    }
}