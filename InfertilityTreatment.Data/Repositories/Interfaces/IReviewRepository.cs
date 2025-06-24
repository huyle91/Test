using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Review;
using InfertilityTreatment.Entity.Entities;
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
        Task<PaginatedResultDto<Review>> GetReviewsAsync(ReviewFilterDto filter);
        Task<Review> AddReviewAsync(Review review);
        Task<bool> ApproveReviewAsync(int reviewId);
        Task<ReviewStatisticsDto> GetReviewStatisticsAsync(int doctorId);
    }
}
