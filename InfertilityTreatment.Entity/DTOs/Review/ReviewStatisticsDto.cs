using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.Review
{
    public class ReviewStatisticsDto
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }                 
        public Dictionary<int, int> RatingDistribution { get; set; } = new();            
        public DateTime? LastReviewDate { get; set; }
    }
}
