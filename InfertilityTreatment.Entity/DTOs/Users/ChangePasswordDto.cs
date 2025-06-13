using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.Users
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Current password must be at least 8 characters.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "New password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$",
            ErrorMessage = "Password must contain uppercase, lowercase letters and at least one number.")]
        public string NewPassword { get; set; }
    }
}
