using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Review;
using InfertilityTreatment.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto);
        Task<PaginatedResultDto<ReviewDto>> GetReviewsAsync(ReviewFilterDto filter,string userRole);
        Task<List<ReviewDto>> GetReviewsByDoctorAsync(int doctorId);
        Task<ReviewStatisticsDto> GetReviewStatisticsAsync(int doctorId);
        Task<bool> ApproveReviewAsync(int reviewId);
    }
}
