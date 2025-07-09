using FluentValidation;
using InfertilityTreatment.Entity.DTOs.Notifications;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.Business.Validators
{
    public class BroadcastNotificationDtoValidator : AbstractValidator<BroadcastNotificationDto>
    {
        public BroadcastNotificationDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .Length(5, 200).WithMessage("Title must be between 5 and 200 characters");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required")
                .Length(10, 1000).WithMessage("Message must be between 10 and 1000 characters");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid notification type");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid notification priority");

            RuleFor(x => x.ScheduledAt)
                .GreaterThan(DateTime.UtcNow).WithMessage("Scheduled time must be in the future")
                .When(x => x.ScheduledAt.HasValue);

            RuleFor(x => x.ExpiresAt)
                .GreaterThan(DateTime.UtcNow).WithMessage("Expiration time must be in the future")
                .When(x => x.ExpiresAt.HasValue);

            RuleFor(x => x.TargetRoles)
                .Must(BeValidRoles).WithMessage("Invalid role specified")
                .When(x => x.TargetRoles != null && x.TargetRoles.Any());
        }

        private bool BeValidRoles(List<string> roles)
        {
            var validRoles = new[] { "Customer", "Doctor", "Admin", "Manager" };
            return roles.All(role => validRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
        }
    }
}
