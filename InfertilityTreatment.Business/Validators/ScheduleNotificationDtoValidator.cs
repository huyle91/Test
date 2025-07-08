using FluentValidation;
using InfertilityTreatment.Entity.DTOs.Notifications;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.Business.Validators
{
    public class ScheduleNotificationDtoValidator : AbstractValidator<ScheduleNotificationDto>
    {
        public ScheduleNotificationDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Valid User ID is required");

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
                .NotEmpty().WithMessage("Scheduled time is required")
                .GreaterThan(DateTime.UtcNow).WithMessage("Scheduled time must be in the future");

            RuleFor(x => x.RelatedEntityId)
                .GreaterThan(0).WithMessage("Related Entity ID must be greater than 0")
                .When(x => x.RelatedEntityId.HasValue);

            RuleFor(x => x.RelatedEntityType)
                .NotEmpty().WithMessage("Related Entity Type is required when Related Entity ID is specified")
                .When(x => x.RelatedEntityId.HasValue);

        }

        private bool BeValidReminderTimes(List<DateTime> reminderTimes)
        {
            return reminderTimes.All(time => time > DateTime.UtcNow);
        }
    }
}
