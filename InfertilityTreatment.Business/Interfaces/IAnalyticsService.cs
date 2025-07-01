using InfertilityTreatment.Entity.DTOs.Analytics;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface IAnalyticsService
    {
        // Prepare for BE019: Analytics & Dashboard Foundation
        Task<DashboardStatsDto> GetDashboardStatsAsync(UserRole role, int userId, DateRangeDto dateRange);
        Task<List<TreatmentSuccessRateDto>> GetTreatmentSuccessRatesAsync();
        Task<RevenueReportDto> GetRevenueReportAsync(RevenueFilterDto filter);
        Task<DoctorPerformanceDto> GetDoctorPerformanceAsync(int doctorId);
    }
}
