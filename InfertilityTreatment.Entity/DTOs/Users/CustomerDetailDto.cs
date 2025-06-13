using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.Users
{
    public class CustomerDetailDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? MedicalHistory { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Occupation { get; set; }
        public UserProfileDto User { get; set; } = null!;
    }
}
