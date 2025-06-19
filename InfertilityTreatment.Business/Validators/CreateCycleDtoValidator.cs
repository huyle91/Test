using FluentValidation;
using InfertilityTreatment.Entity.DTOs.TreatmentCycles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Validators
{
    public class CreateCycleDtoValidator : AbstractValidator<CreateCycleDto>
    {
        public CreateCycleDtoValidator()
        {
            RuleFor(x => x.CustomerId).GreaterThan(0);
            RuleFor(x => x.DoctorId).GreaterThan(0);
            RuleFor(x => x.PackageId).GreaterThan(0);
            RuleFor(x => x.ExpectedEndDate).GreaterThan(x => x.StartDate).When(x => x.StartDate.HasValue);
        }
    }
}
