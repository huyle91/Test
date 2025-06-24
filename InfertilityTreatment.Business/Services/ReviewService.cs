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
        private readonly IMapper _mapper;
        public ReviewService(IReviewRepository reviewRepository, IMapper mapper, ITreatmentCycleRepository cycleRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _cycleRepository = cycleRepository;
        }
        public async Task<bool> ApproveReviewAsync(int reviewId) => await _reviewRepository.ApproveReviewAsync(reviewId);

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto)
        {
            try
            {
                var cycle = await _cycleRepository.GetByIdAsync(createReviewDto.CycleId);
                if (cycle == null  || cycle.Status != CycleStatus.Completed)
                {
                    throw new InvalidOperationException("You can only review a completed treatment cycle.");
                }
                var hasReviewed = await _reviewRepository.ExistsAsync(createReviewDto.CycleId);
                if (hasReviewed)
                {
                    throw new InvalidOperationException("You have already reviewed this treatment cycle.");
                }

                var review = await _reviewRepository.AddReviewAsync(_mapper.Map<Review>(createReviewDto));

                if (review == null)
                {
                    throw new Exception("Unable to create Review.");
                }

                return _mapper.Map<ReviewDto>(review);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating review.", ex);
            }
        }

        public async Task<PaginatedResultDto<ReviewDto>> GetReviewsAsync(ReviewFilterDto filter)
        {
            var pagedResult = await _reviewRepository.GetReviewsAsync(filter);

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

        public Task<ReviewStatisticsDto> GetReviewStatisticsAsync(int doctorId)=>  _reviewRepository.GetReviewStatisticsAsync(doctorId);
        
    }
}
