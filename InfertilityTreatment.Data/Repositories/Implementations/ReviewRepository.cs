using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Review;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Data.Repositories.Implementations
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {

        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> ApproveReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null || review.IsApproved)
                return null;
            review.IsApproved = true;
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<bool> HasReviewedCycleAsync(int cycleId)
        {
            return await _context.Reviews.AnyAsync(r => r.CycleId == cycleId);
        }

        public async Task<PaginatedResultDto<Review>> GetReviewsAsync(ReviewFilterDto filter, UserRole userRole)
        {
            var query = _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Doctor)   
                .AsQueryable();

            if (filter.DoctorId.HasValue)
                query = query.Where(r => r.DoctorId == filter.DoctorId);

            if (filter.CycleId.HasValue)
                query = query.Where(r => r.CycleId == filter.CycleId);

            if (filter.Rating.HasValue)
                query = query.Where(r => r.Rating == filter.Rating);

            if (filter.IsApproved.HasValue)
                query = query.Where(r => r.IsApproved == filter.IsApproved);

            if (filter.FromDate.HasValue)
                query = query.Where(r => r.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(r => r.CreatedAt <= filter.ToDate.Value);
            if (userRole != UserRole.Admin)
            {
                query = query.Where(r => r.IsApproved == true);
            }
            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResultDto<Review>(items, totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<IEnumerable<Review>> GetReviewsByDoctorIdAsync(int doctorId)
        {
            return await _context.Reviews
                .Where(r => r.DoctorId == doctorId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
        public async Task<bool> IsTrustedCustomerAsync(int customerId)
        {
            var approvedCount = await _context.Reviews
                .CountAsync(r => r.CustomerId == customerId && r.IsApproved);

            return approvedCount >= 3;
        }

        public async Task<ReviewStatisticsDto> GetReviewStatisticsAsync(int doctorId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.DoctorId == doctorId && r.IsApproved)
                .ToListAsync();

            if (!reviews.Any()) return null;

            var total = reviews.Count;
            var avgRating = reviews.Average(r => r.Rating);
            var lastReviewDate = reviews.Max(r => r.CreatedAt);

            var ratingDist = reviews
                .GroupBy(r => r.Rating)
                .ToDictionary(g => g.Key, g => g.Count());


            return new ReviewStatisticsDto
            {
                TotalReviews = total,
                AverageRating = Math.Round(avgRating, 2),
                RatingDistribution = ratingDist,
                LastReviewDate = lastReviewDate
            };
        }

        public async Task<decimal> CalculateDoctorSuccessRate(int doctorId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.DoctorId == doctorId && r.IsApproved)
                .ToListAsync();

            if (reviews == null || !reviews.Any())
                return 0m;

            return Math.Round((decimal)reviews.Average(r => r.Rating), 2);
        }

    }
}
