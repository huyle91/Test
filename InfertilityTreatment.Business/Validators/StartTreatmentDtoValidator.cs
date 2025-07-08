using FluentValidation;
using InfertilityTreatment.Entity.DTOs.TreatmentCycles;
using System;

namespace InfertilityTreatment.Business.Validators
{
    public class StartTreatmentDtoValidator : AbstractValidator<StartTreatmentDto>
    {
        public StartTreatmentDtoValidator()
        {
            RuleFor(x => x.ActualStartDate)
                .Must(date => !date.HasValue || date.Value <= DateTime.UtcNow.AddDays(1))
                .WithMessage("Actual start date cannot be more than 1 day in the future");

            RuleFor(x => x.DoctorNotes)
                .MaximumLength(1000)
                .WithMessage("Doctor notes cannot exceed 1000 characters");
        }
    }
}
