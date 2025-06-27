using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Implementations;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Constants;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Review;
using InfertilityTreatment.Entity.DTOs.TreatmentCycles;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ITreatmentCycleRepository _cycleRepository;
        private readonly IDoctorRepository _doctorRepository; 
        private readonly IMapper _mapper;
        public ReviewService(IReviewRepository reviewRepository, IMapper mapper, ITreatmentCycleRepository cycleRepository,IDoctorRepository doctorRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _cycleRepository = cycleRepository;
            _doctorRepository = doctorRepository;
        }
        public async Task<bool> ApproveReviewAsync(int reviewId)
        {
            var review = await _reviewRepository.ApproveReviewAsync(reviewId);
            if (review == null)
            {
                throw new Exception("Unable to approve review. Review may not exist or is already approved.");
            }
            if (review.DoctorId == null)
            {
                throw new Exception("Approved review does not have a valid DoctorId.");
            }

            var successRate = await _reviewRepository.CalculateDoctorSuccessRate(review.DoctorId.Value);

            var updateResult = await _doctorRepository.UpdateSuccessRateAsync(review.DoctorId.Value, successRate);
            if (!updateResult)
            {
                throw new Exception("Failed to update doctor's success rate.");
            }

            return true;
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto)
        {
            try
            {
                if (createReviewDto.Rating < 1 || createReviewDto.Rating > 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(createReviewDto.Rating), "Rating must be between 1 and 5.");
                }

                var cycle = await _cycleRepository.GetByIdAsync(createReviewDto.CycleId);
                if (cycle == null)
                {
                    throw new InvalidOperationException("The specified treatment cycle does not exist.");
                }

                if (cycle.CustomerId != createReviewDto.CustomerId)
                {
                    throw new UnauthorizedAccessException("You can only review your own treatment cycle.");
                }

                if (cycle.Status != CycleStatus.Completed)
                {
                    throw new InvalidOperationException("You can only review a completed treatment cycle.");
                }

                if (await _reviewRepository.HasReviewedCycleAsync(createReviewDto.CycleId))
                {
                    throw new InvalidOperationException("This treatment cycle has already been reviewed.");
                }

                var reviewEntity = _mapper.Map<Review>(createReviewDto);


                var isTrusted = await _reviewRepository.IsTrustedCustomerAsync(createReviewDto.CustomerId);
                reviewEntity.IsApproved = isTrusted;

                var savedReview = await _reviewRepository.AddReviewAsync(reviewEntity);
                if (savedReview == null)
                {
                    throw new Exception("Unable to create review.");
                }

                if (reviewEntity.IsApproved && reviewEntity.DoctorId.HasValue)
                {
                    var successRate = await _reviewRepository.CalculateDoctorSuccessRate(reviewEntity.DoctorId.Value);
                    await _doctorRepository.UpdateSuccessRateAsync(reviewEntity.DoctorId.Value, successRate);
                }

                return _mapper.Map<ReviewDto>(savedReview);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating the review.", ex);
            }
        }


        public async Task<PaginatedResultDto<ReviewDto>> GetReviewsAsync(ReviewFilterDto filter, string role)
        {
            var userRole = Enum.Parse<UserRole>(role);
            var pagedResult = await _reviewRepository.GetReviewsAsync(filter,userRole);

            var mappedItems = _mapper.Map<List<ReviewDto>>(pagedResult.Items);

            return new PaginatedResultDto<ReviewDto>(
                mappedItems,
                pagedResult.TotalCount,
                pagedResult.PageNumber,
                pagedResult.PageSize
            );
        }

        public async Task<List<ReviewDto>> GetReviewsByDoctorAsync(int doctorId)
        {
            var result = await _reviewRepository.GetReviewsByDoctorIdAsync(doctorId);

            var mappedItems = _mapper.Map<List<ReviewDto>>(result);

            return mappedItems;
        }

        public Task<ReviewStatisticsDto> GetReviewStatisticsAsync(int doctorId) => _reviewRepository.GetReviewStatisticsAsync(doctorId);

    }
}
