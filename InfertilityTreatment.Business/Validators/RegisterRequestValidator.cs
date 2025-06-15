using FluentValidation;
using InfertilityTreatment.Entity.DTOs.Auth;
using InfertilityTreatment.Data.Repositories.Interfaces;

namespace InfertilityTreatment.Business.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters")
                .MustAsync(BeUniqueEmail).WithMessage("Email is already registered");

            // Password validation - Simplified (just minimum length)
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters");

            // Full name validation - Support Vietnamese characters
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MinimumLength(2).WithMessage("Full name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");

            // Phone number validation - Optional, Vietnamese format
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[0-9+\-\s()]+$").WithMessage("Phone number contains invalid characters")
                .Length(10, 15).WithMessage("Phone number must be between 10-15 characters")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            // Role validation
            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role specified");

            // Customer specific validations - All optional
            When(x => x.Role == Entity.Enums.UserRole.Customer, () =>
            {
                RuleFor(x => x.Address)
                    .MaximumLength(500).WithMessage("Address cannot exceed 500 characters")
                    .When(x => !string.IsNullOrEmpty(x.Address));

                RuleFor(x => x.EmergencyContactName)
                    .MaximumLength(200).WithMessage("Emergency contact name cannot exceed 200 characters")
                    .When(x => !string.IsNullOrEmpty(x.EmergencyContactName));

                RuleFor(x => x.EmergencyContactPhone)
                    .Matches(@"^[0-9+\-\s()]+$").WithMessage("Emergency contact phone contains invalid characters")
                    .Length(10, 15).WithMessage("Emergency contact phone must be between 10-15 characters")
                    .When(x => !string.IsNullOrEmpty(x.EmergencyContactPhone));

                RuleFor(x => x.MaritalStatus)
.MaximumLength(50).WithMessage("Marital status cannot exceed 50 characters")
                    .When(x => !string.IsNullOrEmpty(x.MaritalStatus));

                RuleFor(x => x.Occupation)
                    .MaximumLength(200).WithMessage("Occupation cannot exceed 200 characters")
                    .When(x => !string.IsNullOrEmpty(x.Occupation));
            });
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            var emailExists = await _unitOfWork.Users.EmailExistsAsync(email);
            return !emailExists;
        }
    }
}