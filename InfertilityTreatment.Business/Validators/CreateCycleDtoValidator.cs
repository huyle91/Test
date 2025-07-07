using FluentValidation;
using InfertilityTreatment.Entity.DTOs.TreatmentCycles;
using InfertilityTreatment.Data.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace InfertilityTreatment.Business.Validators
{
    public class CreateCycleDtoValidator : AbstractValidator<CreateCycleDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ITreatmentPackageRepository _packageRepository;

        public CreateCycleDtoValidator(
            ICustomerRepository customerRepository,
            IDoctorRepository doctorRepository,
            ITreatmentPackageRepository packageRepository,
            ITreatmentCycleRepository cycleRepository)
        {
            _customerRepository = customerRepository;
            _doctorRepository = doctorRepository;
            _packageRepository = packageRepository;

            RuleFor(x => x.DoctorId)
                .MustAsync(DoctorAvailable).WithMessage("Doctor is inactive or not found");

            RuleFor(x => x.PackageId)
                .MustAsync(PackageActive).WithMessage("Treatment package is not available");

            RuleFor(x => x.ExpectedEndDate)
                .GreaterThan(x => x.StartDate.Value)
                .When(x => x.StartDate.HasValue)
                .WithMessage("Expected end date must be after start date");

            RuleFor(x => x.ActualEndDate)
                .GreaterThan(x => x.StartDate.Value)
                .When(x => x.StartDate.HasValue && x.ActualEndDate.HasValue)
                .WithMessage("Actual end date must be after start date");

            RuleFor(x => x.StartDate)
                .Must(date => !date.HasValue || date.Value.Year >= 2000)
                .WithMessage("Start date cannot be before year 2000");

            RuleFor(x => x.ExpectedEndDate)
                .Must(date => !date.HasValue || date.Value <= DateTime.UtcNow.AddYears(2))
                .WithMessage("Expected end date cannot be more than 2 years in the future");

            RuleFor(x => x.StartDate)
                .Must(date => !date.HasValue || date.Value <= DateTime.UtcNow.AddYears(2))
                .WithMessage("Start date cannot be more than 2 years in the future");

            RuleFor(x => x.ExpectedEndDate)
                .Must(date => !date.HasValue || date.Value.Year <= 2100)
                .WithMessage("Expected end date cannot be after year 2100");

            RuleFor(x => x.ActualEndDate)
                .Must(date => !date.HasValue || date.Value.Year <= 2100)
                .WithMessage("Actual end date cannot be after year 2100");
        }

        private async Task<bool> DoctorAvailable(int doctorId, CancellationToken ct)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            return doctor != null && doctor.IsActive;
        }

        private async Task<bool> PackageActive(int packageId, CancellationToken ct)
        {
            var package = await _packageRepository.GetByIdAsync(packageId);
            return package != null && package.IsActive;
        }
    }
}
