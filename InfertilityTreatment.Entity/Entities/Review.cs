using InfertilityTreatment.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.Entities
{
    public class Review : BaseEntity
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int? DoctorId { get; set; }
        public int? CycleId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(50)]
        public string ReviewType { get; set; } = string.Empty; 

        public string Comment { get; set; } = string.Empty;

        public bool IsApproved { get; set; } = false;

        // Navigation Properties
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey(nameof(DoctorId))]
        public virtual Doctor Doctor { get; set; } = null!;

        [ForeignKey(nameof(CycleId))]
        public virtual TreatmentCycle TreatmentCycle { get; set; } = null!;
    }
}
