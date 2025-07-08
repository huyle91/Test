using InfertilityTreatment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.Review
{
    public class CreateReviewDto
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public int CycleId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;


    }
}
