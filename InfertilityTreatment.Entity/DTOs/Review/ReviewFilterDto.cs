using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.Review
{
    public class ReviewFilterDto
    {
        public int? DoctorId { get; set; }
        public int? CycleId { get; set; }
        public int? Rating { get; set; }                
        public bool? IsApproved { get; set; }          
        public DateTime? FromDate { get; set; }         
        public DateTime? ToDate { get; set; }          

        // Paging
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
