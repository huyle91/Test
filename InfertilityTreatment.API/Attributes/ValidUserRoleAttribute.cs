using System.ComponentModel.DataAnnotations;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.API.Attributes
{
    public class ValidUserRoleAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is UserRole role)
            {
                return role == UserRole.Manager || role == UserRole.Doctor;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return "Role must be either Manager or Doctor";
        }
    }
}