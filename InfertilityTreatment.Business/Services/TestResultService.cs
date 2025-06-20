using InfertilityTreatment.Entity.Enums;
using InfertilityTreatment.Entity.Entities;

using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Results;
using InfertilityTreatment.Entity.DTOs.Common;

namespace InfertilityTreatment.Business.Services
{
    public class TestResultService : ITestResultService
    {
        private readonly ITestResultRepository _repo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly ITreatmentCycleRepository _cycleRepo;
        private readonly IMapper _mapper;

        public TestResultService(ITestResultRepository repo, IAppointmentRepository appointmentRepo, ITreatmentCycleRepository cycleRepo, IMapper mapper)
        {
            _repo = repo;
            _appointmentRepo = appointmentRepo;
            _cycleRepo = cycleRepo;
            _mapper = mapper;
        }

        public async Task<TestResultDto> CreateTestResultAsync(CreateTestResultDto dto)
        {
            await ValidateTestResultDtoAsync(dto);
            var entity = _mapper.Map<TestResult>(dto);
            var created = await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return _mapper.Map<TestResultDto>(created);
        }

        private async Task ValidateTestResultDtoAsync(CreateTestResultDto dto)
        {
            // Business validation: TestDate không được ở tương lai
            if (dto.TestDate > DateTime.UtcNow)
                throw new ArgumentException("Test date cannot be in the future");

            // Check CycleId tồn tại
            var cycle = await _cycleRepo.GetByIdAsync(dto.CycleId);
            if (cycle == null)
                throw new ArgumentException($"Treatment cycle with id {dto.CycleId} does not exist");
            // Check appointment
            var apm = await _appointmentRepo.GetByIdAsync(dto.AppointmentId);
            if (apm == null)
                throw new ArgumentException($"Appointment with id {dto.AppointmentId} does not exist");

            // Check enum hợp lệ
            if (!Enum.IsDefined(typeof(TestResultType), dto.TestType))
                throw new ArgumentException($"TestType '{dto.TestType}' is not valid");

            if (!Enum.IsDefined(typeof(TestResultStatus), dto.Status))
                throw new ArgumentException($"Status '{dto.Status}' is not valid");
        }

        private async Task ValidateUpdateTestResultDtoAsync(UpdateTestResultDto dto)
        {
            // Business validation: TestDate không được ở tương lai
            if (dto.TestDate > DateTime.UtcNow)
                throw new ArgumentException("Test date cannot be in the future");

            // Check CycleId tồn tại
            var cycle = await _cycleRepo.GetByIdAsync(dto.CycleId);
            if (cycle == null)
                throw new ArgumentException($"Treatment cycle with id {dto.CycleId} does not exist");
            // Check appointment
            var apm = await _appointmentRepo.GetByIdAsync(dto.AppointmentId);
            if (apm == null)
                throw new ArgumentException($"Appointment with id {dto.AppointmentId} does not exist");

            // Check enum hợp lệ
            if (!Enum.IsDefined(typeof(TestResultType), dto.TestType))
                throw new ArgumentException($"TestType '{dto.TestType}' is not valid");

            if (!Enum.IsDefined(typeof(TestResultStatus), dto.Status))
                throw new ArgumentException($"Status '{dto.Status}' is not valid");
        }

        public async Task<PaginatedResultDto<TestResultDto>> GetTestResultsByCycleAsync(int cycleId, PaginationQueryDTO pagination)
        {
            var paged = await _repo.GetTestResultsByCycleAsync(cycleId, pagination);
            return new PaginatedResultDto<TestResultDto>(
                _mapper.Map<List<TestResultDto>>(paged.Items),
                paged.TotalCount,
                paged.PageNumber,
                paged.PageSize
            );
        }

        public async Task<TestResultDetailDto?> GetTestResultByIdAsync(int testResultId)
        {
            var entity = await _repo.GetByIdAsync(testResultId);
            if (entity == null) return null;
            var detailDto = _mapper.Map<TestResultDetailDto>(entity);
            detailDto.ResultInterpretation = InterpretResult(entity);
            return detailDto;
        }

        // Helper for result interpretation logic
        private string InterpretResult(TestResult entity)
        {
            if (!decimal.TryParse(entity.Results, out var resultValue))
                return "RequiresAttention";

            if (TryParseRange(entity.ReferenceRange, out var min, out var max, out var type))
            {
                switch (type)
                {
                    case RangeType.MinMax:
                        if (resultValue < min * 0.8m)
                            return "CriticalLow";
                        if (resultValue < min)
                            return "Low";
                        if (resultValue > max * 1.2m)
                            return "CriticalHigh";
                        if (resultValue > max)
                            return "High";
                        return "Normal";

                    case RangeType.GreaterThan:
                        if (resultValue <= min * 0.8m)
                            return "CriticalLow";
                        if (resultValue <= min)
                            return "Low";
                        return "Normal";

                    case RangeType.LessThan:
                        if (resultValue >= max * 1.2m)
                            return "CriticalHigh";
                        if (resultValue >= max)
                            return "High";
                        return "Normal";
                }
            }

            return "RequiresAttention";
        }

        private bool TryParseRange(string? range, out decimal min, out decimal max, out RangeType type)
        {
            min = max = 0;
            type = RangeType.MinMax;

            if (string.IsNullOrWhiteSpace(range)) return false;

            range = range.Trim();

            if (range.Contains('-'))
            {
                var parts = range.Split('-', StringSplitOptions.TrimEntries);
                if (parts.Length == 2 &&
                    decimal.TryParse(parts[0], out min) &&
                    decimal.TryParse(parts[1], out max))
                {
                    type = RangeType.MinMax;
                    return true;
                }
            }
            else if (range.StartsWith(">") && decimal.TryParse(range[1..], out min))
            {
                type = RangeType.GreaterThan;
                return true;
            }
            else if (range.StartsWith("<") && decimal.TryParse(range[1..], out max))
            {
                type = RangeType.LessThan;
                return true;
            }

            return false;
        }

        public async Task<TestResultDto> UpdateTestResultAsync(int testResultId, UpdateTestResultDto dto)
        {
            await ValidateUpdateTestResultDtoAsync(dto);
            var entity = await _repo.GetByIdAsync(testResultId);
            if (entity == null) throw new KeyNotFoundException("TestResult not found");
            _mapper.Map(dto, entity);
            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
            return _mapper.Map<TestResultDto>(entity);
        }

        public async Task<PaginatedResultDto<TestResultDto>> GetTestResultsByTypeAsync(int cycleId, TestResultType type, PaginationQueryDTO pagination)
        {
            var paged = await _repo.GetTestResultsByTypeAsync(cycleId, type, pagination);
            return new PaginatedResultDto<TestResultDto>(
                _mapper.Map<List<TestResultDto>>(paged.Items),
                paged.TotalCount,
                paged.PageNumber,
                paged.PageSize
            );
        }

        public async Task<PaginatedResultDto<TestResultDto>> GetTestResultsAsync(int? cycleId, TestResultType? type, DateTime? date, PaginationQueryDTO pagination)
        {
            var paged = await _repo.GetTestResultsAsync(cycleId, type, date, pagination);
            return new PaginatedResultDto<TestResultDto>(
                _mapper.Map<List<TestResultDto>>(paged.Items),
                paged.TotalCount,
                paged.PageNumber,
                paged.PageSize
            );
        }
    }
}