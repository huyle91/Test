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

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters")
                .MustAsync(BeUniqueEmail).WithMessage("Email is already registered");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
                .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one number");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Full name can only contain letters and spaces");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(\+84|84|0)?[0-9]{9,10}$").WithMessage("Invalid phone number format. Use Vietnamese format: 0901234567 or +84901234567")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role specified");

            // Customer specific validations
            When(x => x.Role == Entity.Enums.UserRole.Customer, () =>
            {
                RuleFor(x => x.Address)
                    .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");

                RuleFor(x => x.EmergencyContactName)
                    .MaximumLength(200).WithMessage("Emergency contact name cannot exceed 200 characters");

                RuleFor(x => x.EmergencyContactPhone)
                    .Matches(@"^(\+84|84|0)?[0-9]{9,10}$").WithMessage("Invalid emergency contact phone format. Use Vietnamese format: 0901234567 or +84901234567")
                    .When(x => !string.IsNullOrEmpty(x.EmergencyContactPhone));

                RuleFor(x => x.MaritalStatus)
                    .MaximumLength(50).WithMessage("Marital status cannot exceed 50 characters");

                RuleFor(x => x.Occupation)
                    .MaximumLength(200).WithMessage("Occupation cannot exceed 200 characters");
            });
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == email);
            return existingUser == null;
        }
    }
}
