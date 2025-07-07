using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int? DoctorId { get; set; }
        public int? CycleId { get; set; }
        public int Rating { get; set; }
        public string ReviewType { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive
        {
            get; set;
        }
    }
}
