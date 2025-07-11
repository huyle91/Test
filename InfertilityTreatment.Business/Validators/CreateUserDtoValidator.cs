using FluentValidation;
using InfertilityTreatment.Entity.DTOs.Users;
using InfertilityTreatment.Entity.Enums;
using InfertilityTreatment.Data.Repositories.Interfaces;

namespace InfertilityTreatment.Business.Validators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters")
                .MustAsync(BeUniqueEmail).WithMessage("Email is already registered");

            // Password validation
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters");

            // Full name validation
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MinimumLength(2).WithMessage("Full name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");

            // Phone number validation - Optional
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[0-9+\-\s()]+$").WithMessage("Phone number contains invalid characters")
                .Length(10, 15).WithMessage("Phone number must be between 10-15 characters")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            // Role validation - Only manager and doctor allowed
            RuleFor(x => x.Role)
                .Must(BeValidRole).WithMessage("Only Manager and Doctor roles are allowed for user creation");
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            var emailExists = await _unitOfWork.Users.EmailExistsAsync(email);
            return !emailExists;
        }

        private bool BeValidRole(UserRole role)
        {
            return role == UserRole.Manager || role == UserRole.Doctor;
        }
    }
}