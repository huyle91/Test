using InfertilityTreatment.Entity.DTOs.Analytics;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface IAnalyticsService
    {
        // Dashboard Analytics
        Task<DashboardStatsDto> GetDashboardStatsAsync(UserRole role, int? userId, DateRangeDto dateRange);
        
        // Treatment Success Rates
        Task<PaginatedResultDto<TreatmentSuccessRateDto>> GetTreatmentSuccessRatesAsync(SuccessRateFilterDto filter, PaginationQueryDTO pagination);
        
        // Revenue Analytics
        Task<RevenueReportDto> GetRevenueReportAsync(RevenueFilterDto filter);
        
        // Doctor Performance
        Task<DoctorPerformanceDto> GetDoctorPerformanceAsync(int doctorId);
        
        // Patient Demographics
        Task<PatientDemographicsDto> GetPatientDemographicsAsync(DateRangeDto dateRange);
        
        // Export Functionality
        Task<byte[]> ExportReportToPdfAsync(ExportReportDto exportRequest);
        Task<byte[]> ExportReportToExcelAsync(ExportReportDto exportRequest);

        Task<bool> CheckIsDoctorIdWithUserId(int userId, int doctorId);

        Task<OutcomeAnalysisResultDto> GetTreatmentOutcomesAsync(OutcomeAnalysisDto filters);
        Task<EfficiencyMetrics> GetEfficiencyMetricsAsync(EfficiencyQueryDto query);
        Task<PatientJourneyResultDto> GetPatientJourneyAnalyticsAsync(PatientJourneyDto filters);
        Task<PredictiveAnalyticsResultDto> GetPredictiveAnalyticsAsync(PredictiveQueryDto query);
        Task<CustomReportResultDto> GenerateCustomReportAsync(CustomReportDto dto);
    }
}
