using InfertilityTreatment.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.Entities
{
    public class Medication : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ActiveIngredient { get; set; }
        [Required]
        public string Manufacturer { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string StorageInstructions { get; set; }
        [Required]
        public string SideEffects { get; set; }

        // Navigation Property
        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
