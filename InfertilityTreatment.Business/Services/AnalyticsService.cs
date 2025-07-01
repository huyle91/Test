using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Analytics;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(UserRole role, int userId, DateRangeDto dateRange)
        {
            var startDate = dateRange.StartDate;
            var endDate = dateRange.EndDate;

            // Get all cycles and filter by date
            var allCycles = await _unitOfWork.TreatmentCycles.GetAllAsync();
            var cycles = allCycles.Where(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate);

            // Filter by user role
            if (role == UserRole.Doctor)
            {
                cycles = cycles.Where(c => c.DoctorId == userId);
            }
            else if (role == UserRole.Customer)
            {
                cycles = cycles.Where(c => c.CustomerId == userId);
            }

            var cyclesList = cycles.ToList();

            // Get all appointments and filter by date
            var allAppointments = await _unitOfWork.Appointments.GetAllAsync();
            var appointments = allAppointments.Where(a => a.CreatedAt >= startDate && a.CreatedAt <= endDate);

            if (role == UserRole.Doctor)
            {
                appointments = appointments.Where(a => a.DoctorId == userId);
            }
            else if (role == UserRole.Customer)
            {
                var customerCycleIds = cyclesList.Select(c => c.Id).ToList();
                appointments = appointments.Where(a => customerCycleIds.Contains(a.CycleId));
            }

            var appointmentsList = appointments.ToList();

            // Calculate stats
            var totalPatients = role == UserRole.Doctor 
                ? cyclesList.Select(c => c.CustomerId).Distinct().Count()
                : (role == UserRole.Customer ? 1 : cyclesList.Select(c => c.CustomerId).Distinct().Count());

            var activeTreatments = cyclesList.Count(c => c.Status == CycleStatus.InProgress);
            var completedTreatments = cyclesList.Count(c => c.Status == CycleStatus.Completed);
            var cancelledTreatments = cyclesList.Count(c => c.Status == CycleStatus.Cancelled);
            var successRate = (completedTreatments + cancelledTreatments) > 0 ? 
                (double)completedTreatments / (completedTreatments + cancelledTreatments) * 100 : 0;

            // Calculate revenue from packages
            var packageIds = cyclesList.Select(c => c.PackageId).Distinct().ToList();
            var allPackages = await _unitOfWork.TreatmentPackages.GetAllAsync();
            var packages = allPackages.Where(p => packageIds.Contains(p.Id)).ToList();

            var totalRevenue = cyclesList.Sum(c => packages.FirstOrDefault(p => p.Id == c.PackageId)?.Price ?? 0);

            return new DashboardStatsDto
            {
                TotalPatients = totalPatients,
                TotalAppointments = appointmentsList.Count,
                TotalTreatmentCycles = cyclesList.Count,
                TotalRevenue = totalRevenue,
                ActiveTreatments = activeTreatments,
                CompletedTreatments = completedTreatments,
                SuccessRate = successRate,
                LastUpdated = DateTime.Now
            };
        }

        public async Task<List<TreatmentSuccessRateDto>> GetTreatmentSuccessRatesAsync()
        {
            var treatmentServices = (await _unitOfWork.TreatmentServices.GetAllAsync()).ToList();
            var packages = (await _unitOfWork.TreatmentPackages.GetAllAsync()).ToList();
            var cycles = (await _unitOfWork.TreatmentCycles.GetAllAsync()).ToList();

            var result = new List<TreatmentSuccessRateDto>();

            foreach (var service in treatmentServices)
            {
                var servicePackages = packages.Where(p => p.ServiceId == service.Id).Select(p => p.Id).ToList();
                var serviceCycles = cycles.Where(c => servicePackages.Contains(c.PackageId)).ToList();

                if (serviceCycles.Any())
                {
                    var totalCycles = serviceCycles.Count;
                    var successfulCycles = serviceCycles.Count(c => c.Status == CycleStatus.Completed);
                    var successRate = totalCycles > 0 ? (double)successfulCycles / totalCycles * 100 : 0;

                    result.Add(new TreatmentSuccessRateDto
                    {
                        TreatmentType = service.Name,
                        TotalCycles = totalCycles,
                        SuccessfulCycles = successfulCycles,
                        SuccessRate = successRate,
                        PeriodStart = serviceCycles.Min(c => c.CreatedAt),
                        PeriodEnd = serviceCycles.Max(c => c.CreatedAt)
                    });
                }
            }

            return result.OrderByDescending(r => r.SuccessRate).ToList();
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync(RevenueFilterDto filter)
        {
            var allCycles = await _unitOfWork.TreatmentCycles.GetAllAsync();
            var cycles = allCycles.Where(c => c.CreatedAt >= filter.StartDate && c.CreatedAt <= filter.EndDate);

            if (filter.DoctorId.HasValue)
            {
                cycles = cycles.Where(c => c.DoctorId == filter.DoctorId.Value);
            }

            var cyclesList = cycles.ToList();

            // Get packages for revenue calculation
            var packageIds = cyclesList.Select(c => c.PackageId).Distinct().ToList();
            var allPackages = await _unitOfWork.TreatmentPackages.GetAllAsync();
            var packages = allPackages.Where(p => packageIds.Contains(p.Id)).ToList();

            // Get treatment services for package details
            var serviceIds = packages.Select(p => p.ServiceId).Distinct().ToList();
            var allServices = await _unitOfWork.TreatmentServices.GetAllAsync();
            var services = allServices.Where(s => serviceIds.Contains(s.Id)).ToList();

            var totalRevenue = cyclesList.Sum(c => packages.FirstOrDefault(p => p.Id == c.PackageId)?.Price ?? 0);

            // Monthly breakdown
            var monthlyData = cyclesList
                .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
                .Select(g => new MonthlyRevenueDto
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(c => packages.FirstOrDefault(p => p.Id == c.PackageId)?.Price ?? 0),
                    TransactionCount = g.Count()
                })
                .OrderBy(m => m.Year).ThenBy(m => m.Month)
                .ToList();

            // Service breakdown
            var serviceData = new List<ServiceRevenueDto>();
            foreach (var service in services)
            {
                var servicePackageIds = packages.Where(p => p.ServiceId == service.Id).Select(p => p.Id).ToList();
                var serviceCycles = cyclesList.Where(c => servicePackageIds.Contains(c.PackageId)).ToList();
                
                if (serviceCycles.Any())
                {
                    var serviceRevenue = serviceCycles.Sum(c => packages.FirstOrDefault(p => p.Id == c.PackageId)?.Price ?? 0);
                    serviceData.Add(new ServiceRevenueDto
                    {
                        ServiceName = service.Name,
                        Revenue = serviceRevenue,
                        BookingCount = serviceCycles.Count,
                        Percentage = totalRevenue > 0 ? (double)(serviceRevenue / totalRevenue) * 100 : 0
                    });
                }
            }

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var monthlyRevenue = monthlyData.FirstOrDefault(m => m.Month == currentMonth && m.Year == currentYear)?.Revenue ?? 0;
            var yearlyRevenue = monthlyData.Where(m => m.Year == currentYear).Sum(m => m.Revenue);

            return new RevenueReportDto
            {
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,
                YearlyRevenue = yearlyRevenue,
                MonthlyBreakdown = monthlyData,
                ServiceBreakdown = serviceData.OrderByDescending(s => s.Revenue).ToList(),
                ReportDate = DateTime.Now
            };
        }

        public async Task<DoctorPerformanceDto> GetDoctorPerformanceAsync(int doctorId)
        {
            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
            {
                throw new KeyNotFoundException($"Doctor with ID {doctorId} not found");
            }

            // Get user details
            var user = await _unitOfWork.Users.GetByIdAsync(doctor.UserId);

            var allCycles = await _unitOfWork.TreatmentCycles.GetAllAsync();
            var cycles = allCycles.Where(c => c.DoctorId == doctorId).ToList();

            var allAppointments = await _unitOfWork.Appointments.GetAllAsync();
            var appointments = allAppointments.Where(a => a.DoctorId == doctorId).ToList();

            var allReviews = await _unitOfWork.Reviews.GetAllAsync();
            var reviews = allReviews.Where(r => r.DoctorId == doctorId).ToList();

            // Get packages for revenue calculation
            var packageIds = cycles.Select(c => c.PackageId).Distinct().ToList();
            var allPackages = await _unitOfWork.TreatmentPackages.GetAllAsync();
            var packages = allPackages.Where(p => packageIds.Contains(p.Id)).ToList();

            var totalPatients = cycles.Select(c => c.CustomerId).Distinct().Count();
            var completedTreatments = cycles.Count(c => c.Status == CycleStatus.Completed);
            var totalTreatments = cycles.Count(c => c.Status == CycleStatus.Completed || c.Status == CycleStatus.Cancelled);
            var successRate = totalTreatments > 0 ? (double)completedTreatments / totalTreatments * 100 : 0;

            var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            var totalRevenue = cycles.Sum(c => packages.FirstOrDefault(p => p.Id == c.PackageId)?.Price ?? 0);

            // Monthly performance
            var monthlyPerformance = cycles
                .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
                .Select(g => new MonthlyPerformanceDto
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Appointments = appointments.Count(a => a.CreatedAt.Year == g.Key.Year && a.CreatedAt.Month == g.Key.Month),
                    Revenue = g.Sum(c => packages.FirstOrDefault(p => p.Id == c.PackageId)?.Price ?? 0),
                    Rating = reviews.Where(r => r.CreatedAt.Year == g.Key.Year && r.CreatedAt.Month == g.Key.Month)
                                   .Any() ? reviews.Where(r => r.CreatedAt.Year == g.Key.Year && r.CreatedAt.Month == g.Key.Month)
                                                  .Average(r => r.Rating) : 0
                })
                .OrderBy(m => m.Year).ThenBy(m => m.Month)
                .ToList();

            return new DoctorPerformanceDto
            {
                DoctorId = doctorId,
                DoctorName = user?.FullName ?? "Unknown",
                Specialization = doctor.Specialization ?? "General",
                TotalPatients = totalPatients,
                TotalAppointments = appointments.Count,
                CompletedTreatments = completedTreatments,
                SuccessRate = successRate,
                AverageRating = averageRating,
                TotalReviews = reviews.Count,
                TotalRevenue = totalRevenue,
                MonthlyPerformance = monthlyPerformance,
                LastUpdated = DateTime.Now
            };
        }
    }
}
