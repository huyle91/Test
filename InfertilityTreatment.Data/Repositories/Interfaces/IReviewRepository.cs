using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Review;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Data.Repositories.Interfaces
{
    public interface IReviewRepository : IBaseRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByDoctorIdAsync(int doctorId);
        Task<PaginatedResultDto<Review>> GetReviewsAsync(ReviewFilterDto filter, UserRole userRole);
        Task<Review> AddReviewAsync(Review review);
        Task<Review> ApproveReviewAsync(int reviewId);
        Task<decimal> CalculateDoctorSuccessRate(int doctorId);
        Task<ReviewStatisticsDto> GetReviewStatisticsAsync(int doctorId);
        Task<bool> HasReviewedCycleAsync(int cycleId);
        Task<bool> IsTrustedCustomerAsync(int customerId);
    }
}
