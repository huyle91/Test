using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Implementations;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.TreatmentCycles;
using InfertilityTreatment.Entity.DTOs.TreatmentPhase;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class CycleService : ICycleService
    {
        private readonly ITreatmentCycleRepository _treatmentCycleRepository;
        private readonly ITreatmentPhaseRepository _treatmentPhaseRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CycleService> _logger;
        public CycleService(ITreatmentCycleRepository treatmentCycleRepository, IMapper mapper, ITreatmentPhaseRepository treatmentPhaseRepository, IDoctorRepository doctorRepository, ILogger<CycleService> logger, IUnitOfWork unitOfWork)
        {
            _treatmentCycleRepository = treatmentCycleRepository;
            _mapper = mapper;
            _treatmentPhaseRepository = treatmentPhaseRepository;
            _doctorRepository = doctorRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<PhaseResponseDto> AddPhaseAsync(int cycleId, CreatePhaseDto createPhaseDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var cycle = await _treatmentCycleRepository.GetByIdAsync(cycleId);
                if (cycle == null)
                    throw new ArgumentException("Invalid cycle ID");

                var phase = _mapper.Map<TreatmentPhase>(createPhaseDto);
                phase.CycleId = cycleId;

                await _treatmentPhaseRepository.AddTreatmentPhaseAsync(phase);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<PhaseResponseDto>(phase);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }


        public async Task<bool> AssignDoctorToCycleAsync(int cycleId, int doctorId)
        {
            try
            {
                // 1. Validate doctor
                var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
                if (doctor is null)
                {
                    _logger.LogWarning("Doctor with ID {DoctorId} not found", doctorId);
                    throw new ArgumentException($"Doctor with ID {doctorId} does not exist");
                }

                if (!doctor.IsActive)
                {
                    _logger.LogWarning("Doctor with ID {DoctorId} is inactive", doctorId);
                    throw new InvalidOperationException("Cannot assign an inactive doctor to a treatment cycle");
                }

                // 2. Validate treatment cycle
                var cycle = await _treatmentCycleRepository.GetByIdAsync(cycleId);
                if (cycle is null)
                {
                    _logger.LogWarning($"Treatment cycle with ID {cycleId} not found", cycleId);
                    throw new ArgumentException($"Treatment cycle with ID {cycleId} does not exist");
                }

                if (cycle.Status == CycleStatus.Completed)
                {
                    _logger.LogWarning($"Cannot assign doctor to completed cycle {cycleId}", cycleId);
                    throw new InvalidOperationException("Cannot assign doctor to a completed treatment cycle");
                }

                // 3. Assign doctor
                var updated = await _treatmentCycleRepository.UpdateDoctorAsync(cycleId, doctorId);
                if (!updated)
                {
                    _logger.LogWarning($"Failed to assign doctor {doctorId} to cycle {cycleId}", doctorId, cycleId);
                    throw new InvalidOperationException("Failed to assign doctor to treatment cycle");
                }

                _logger.LogInformation($"Successfully assigned doctor {doctorId}  to cycle  {cycleId}", doctorId, cycleId);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while assigning doctor {doctorId} to cycle {cycleId}", doctorId, cycleId);
                throw new InvalidOperationException("A database error occurred while assigning the doctor", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error assigning doctor {doctorId} to cycle {cycleId}", doctorId, cycleId);
                throw;
            }
        }

        public Task<decimal> CalculateCycleCostAsync(int cycleId)
        {
            return _treatmentCycleRepository.CalculatePhaseCostAsync(cycleId);
        }
        public async Task<CycleResponseDto> CreateCycleAsync(CreateCycleDto createCycleDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Validate Package
                var package = await _unitOfWork.TreatmentPackages.GetByIdAsync(createCycleDto.PackageId);
                if (package == null || !package.IsActive)
                    throw new ArgumentException($"Treatment package with ID {createCycleDto.PackageId} does not exist or is inactive");

                // Validate Doctor
                    var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(createCycleDto.DoctorId);
                    if (doctor == null || !doctor.IsActive || !doctor.IsAvailable)
                        throw new ArgumentException($"Doctor with ID {createCycleDto.DoctorId} does not exist, is inactive, or not available");

                // Validate CycleNumber uniqueness
                var existingCycle = await _treatmentCycleRepository.GetCycleByCustomerAndNumberAsync(
                    createCycleDto.CustomerId, createCycleDto.CycleNumber);
                if (existingCycle != null)
                    throw new ArgumentException($"Cycle number {createCycleDto.CycleNumber} already exists for this customer");

                // Validate date logic
                ValidateDateLogic(createCycleDto);

                var cycle = await _treatmentCycleRepository.AddTreatmentCycleAsync(_mapper.Map<TreatmentCycle>(createCycleDto));
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<CycleResponseDto>(cycle);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        public async Task<CycleDetailDto> GetCycleByIdAsync(int cycleId)
        {
            try
            {
                var cycle = await _treatmentCycleRepository.GetCycleByIdAsync(cycleId);

                if (cycle == null)
                {
                    throw new InvalidOperationException("Unable to get TreatmentCycle.");
                }

                return _mapper.Map<CycleDetailDto>(cycle);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating the treatment cycle.", ex);
            }
        }
        public async Task<PaginatedResultDto<PhaseResponseDto>> GetCyclePhasesAsync(int cycleId, TreatmentPhaseFilterDto filter)
        {
            var pagedResult = await _treatmentPhaseRepository.GetCyclePhasesByCycleId(cycleId, filter);

            var mapped = _mapper.Map<List<PhaseResponseDto>>(pagedResult.Items);

            return new PaginatedResultDto<PhaseResponseDto>(
                mapped,
                pagedResult.TotalCount,
                pagedResult.PageNumber,
                pagedResult.PageSize
            );
        }

        public async Task<bool> UpdatePhaseAsync(int phaseId, UpdatePhaseDto updatePhaseDto)
        {
            var updated = await _treatmentPhaseRepository.UpdatePhaseAsync(
                phaseId,
                _mapper.Map<TreatmentPhase>(updatePhaseDto)
            );

            return updated;
        }
        public async Task<PaginatedResultDto<CycleResponseDto>> GetCyclesByCustomerAsync(int customerId, TreatmentCycleFilterDto filter)
        {
            var paginatedResult = await _treatmentCycleRepository.GetCyclesByCustomerAsync(customerId, filter);

            var mappedItems = paginatedResult.Items
                .Select(c => _mapper.Map<CycleResponseDto>(c))
                .ToList();

            return new PaginatedResultDto<CycleResponseDto>(
                mappedItems,
                paginatedResult.TotalCount,
                paginatedResult.PageNumber,
                paginatedResult.PageSize
            );
        }

        public async Task<PaginatedResultDto<CycleResponseDto>> GetCyclesByDoctorAsync(int doctorId, TreatmentCycleFilterDto filter)
        {
            var paginatedResult = await _treatmentCycleRepository.GetCyclesByDoctorAsync(doctorId, filter);

            var mappedItems = paginatedResult.Items
                .Select(c => _mapper.Map<CycleResponseDto>(c))
                .ToList();

            return new PaginatedResultDto<CycleResponseDto>(
                mappedItems,
                paginatedResult.TotalCount,
                paginatedResult.PageNumber,
                paginatedResult.PageSize
            );
        }

        Task<bool> ICycleService.UpdateCycleStatusAsync(int cycleId, CycleStatus status)
        {
            return _treatmentCycleRepository.UpdateStatusAsync(cycleId, status);
        }
        public Task<bool> UpdateCycleAsync(int cycleId, UpdateCycleDto dto)
        {
            var cycle = _mapper.Map<TreatmentCycle>(dto);
            cycle.Id = cycleId;
            return _treatmentCycleRepository.UpdateTreatmentCycleAsync(cycle);
        }

        private void ValidateDateLogic(CreateCycleDto dto)
        {
            if (dto.StartDate.HasValue && dto.ExpectedEndDate.HasValue)
            {
                if (dto.StartDate >= dto.ExpectedEndDate)
                    throw new ArgumentException("StartDate must be before ExpectedEndDate");
            }

            if (dto.ActualEndDate.HasValue && dto.StartDate.HasValue)
            {
                if (dto.ActualEndDate <= dto.StartDate)
                    throw new ArgumentException("ActualEndDate must be after StartDate");
            }

            if (dto.ExpectedEndDate.HasValue)
            {
                if (dto.ExpectedEndDate < DateTime.UtcNow.Date)
                    throw new ArgumentException("ExpectedEndDate cannot be in the past");
                if (dto.ExpectedEndDate > DateTime.UtcNow.AddYears(2))
                    throw new ArgumentException("ExpectedEndDate is too far in the future");
            }
        }

        #region Phase Management Methods BE-022

        public async Task<PhaseResponseDto> StartPhaseAsync(int cycleId, int phaseId, StartPhaseDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Validate cycle exists
                var cycle = await _treatmentCycleRepository.GetByIdAsync(cycleId);
                if (cycle == null)
                    throw new ArgumentException("Cycle ID is invalid");

                // Get the phase and validate it belongs to the cycle
                var phase = await _treatmentPhaseRepository.GetTreatmentPhaseByIdAsync(phaseId);
                if (phase == null || phase.CycleId != cycleId)
                    throw new ArgumentException("Invalid phase ID or phase does not belong to the specified cycle");

                // Validate phase can be started
                if (phase.Status != PhaseStatus.Pending)
                    throw new InvalidOperationException($"Phase cannot be started. Current status: {phase.Status}");

                // Check if previous phases are completed (if any): validate phase phai theo thu tu, khong lam 2 truoc 1
                var previousPhases = await _treatmentPhaseRepository.GetPhasesByCycleIdAsync(cycleId);
                var incompletePreviousPhases = previousPhases
                    .Where(p => p.PhaseOrder < phase.PhaseOrder && p.Status != PhaseStatus.Completed)
                    .Any();

                if (incompletePreviousPhases)
                    throw new InvalidOperationException("Previous phases must be completed before starting this phase");

                // Update phase
                phase.Status = PhaseStatus.InProgress;
                phase.StartDate = dto.StartDate;
                if (!string.IsNullOrEmpty(dto.Instructions))
                    phase.Instructions = dto.Instructions;
                if (!string.IsNullOrEmpty(dto.Notes))
                    phase.Notes = dto.Notes;
                phase.UpdatedAt = DateTime.UtcNow;

                await _treatmentPhaseRepository.UpdateTreatmentPhaseAsync(phase);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Phase {phaseId} started for cycle {cycleId}");
                return _mapper.Map<PhaseResponseDto>(phase);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<PhaseResponseDto> CompletePhaseAsync(int cycleId, int phaseId, CompletePhaseDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Validate cycle exists
                var cycle = await _treatmentCycleRepository.GetByIdAsync(cycleId);
                if (cycle == null)
                    throw new ArgumentException("Cycle ID is invalid");

                // Get the phase and validate it belongs to the cycle
                var phase = await _treatmentPhaseRepository.GetTreatmentPhaseByIdAsync(phaseId);
                if (phase == null || phase.CycleId != cycleId)
                    throw new ArgumentException("Invalid phase ID or phase does not belong to the specified cycle");

                // Validate phase can be completed
                if (phase.Status != PhaseStatus.InProgress)
                    throw new InvalidOperationException($"Phase cannot be completed. Current status: {phase.Status}");

                // Validate end date
                if (phase.StartDate.HasValue && dto.EndDate < phase.StartDate.Value)
                    throw new ArgumentException("End date cannot be before start date");

                // Update phase
                phase.Status = PhaseStatus.Completed;
                phase.EndDate = dto.EndDate;
                
                if (!string.IsNullOrEmpty(dto.Results))
                {
                    phase.Notes = string.IsNullOrEmpty(phase.Notes) 
                        ? $"Results: {dto.Results}" 
                        : $"{phase.Notes}\nResults: {dto.Results}";
                }
                
                if (!string.IsNullOrEmpty(dto.NextPhaseInstructions))
                {
                    phase.Notes = string.IsNullOrEmpty(phase.Notes) 
                        ? $"Next Phase Instructions: {dto.NextPhaseInstructions}" 
                        : $"{phase.Notes}\nNext Phase Instructions: {dto.NextPhaseInstructions}";
                }
                
                if (!string.IsNullOrEmpty(dto.Notes))
                {
                    phase.Notes = string.IsNullOrEmpty(phase.Notes) 
                        ? dto.Notes 
                        : $"{phase.Notes}\n{dto.Notes}";
                }
                
                phase.UpdatedAt = DateTime.UtcNow;

                await _treatmentPhaseRepository.UpdateTreatmentPhaseAsync(phase);

                // Check if this was the last phase of the cycle -> neu cac phase deu completed -> cycle status up lai
                var allPhases = await _treatmentPhaseRepository.GetPhasesByCycleIdAsync(cycleId);
                var isLastPhase = allPhases.All(p => p.Status == PhaseStatus.Completed || p.Id == phaseId);
                
                if (isLastPhase && cycle.Status == CycleStatus.InProgress)
                {
                    cycle.Status = CycleStatus.Completed;
                    cycle.ActualEndDate = dto.EndDate;
                    cycle.UpdatedAt = DateTime.UtcNow;
                    await _treatmentCycleRepository.UpdateTreatmentCycleAsync(cycle);
                    _logger.LogInformation($"Cycle {cycleId} completed as all phases are finished");
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Phase {phaseId} completed for cycle {cycleId}");
                return _mapper.Map<PhaseResponseDto>(phase);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<PhaseProgressDto> GetPhaseProgressAsync(int cycleId, int phaseId)
        {
            // Validate cycle exists
            var cycle = await _treatmentCycleRepository.GetByIdAsync(cycleId);
            if (cycle == null)
                throw new ArgumentException("Cycle ID is invalid");

            // Get the phase and validate it belongs to the cycle
            var phase = await _treatmentPhaseRepository.GetTreatmentPhaseByIdAsync(phaseId);
            if (phase == null || phase.CycleId != cycleId)
                throw new ArgumentException("Invalid phase ID or phase does not belong to the specified cycle");

            // Get appointments for this cycle (we'll consider them related to the current phase if in progress)
            var appointments = await _unitOfWork.Appointments.GetAppointmentsByCycleIdAsync(cycleId);
            var phaseAppointments = appointments.Where(a => 
                phase.StartDate.HasValue && a.ScheduledDateTime >= phase.StartDate.Value &&
                (!phase.EndDate.HasValue || a.ScheduledDateTime <= phase.EndDate.Value)
            ).ToList();

            // Get test results for this cycle
            var testResults = await _unitOfWork.TestResults.GetByCycleIdAsync(cycleId);
            var phaseTestResults = testResults.Where(t => 
                phase.StartDate.HasValue && t.CreatedAt >= phase.StartDate.Value &&
                (!phase.EndDate.HasValue || t.CreatedAt <= phase.EndDate.Value)
            ).ToList();

            // Calculate progress based on phase status and data
            double progressPercentage = phase.Status switch
            {
                PhaseStatus.Pending => 0,
                PhaseStatus.InProgress => CalculateInProgressPercentage(phase, phaseAppointments, phaseTestResults),
                PhaseStatus.Completed => 100,
                PhaseStatus.Cancelled => 0,
                PhaseStatus.OnHold => CalculateInProgressPercentage(phase, phaseAppointments, phaseTestResults),
                _ => 0
            };

            // Generate milestones based on treatment type and phase
            var (completed, pending, current) = GeneratePhaseMilestones(phase, phaseAppointments, phaseTestResults);

            var progressDto = new PhaseProgressDto
            {
                PhaseId = phase.Id,
                PhaseName = phase.PhaseName,
                Status = phase.Status,
                PhaseOrder = phase.PhaseOrder,
                ProgressPercentage = progressPercentage,
                CurrentMilestone = current,
                CompletedMilestones = completed,
                PendingMilestones = pending,
                StartDate = phase.StartDate,
                EstimatedEndDate = phase.StartDate?.AddDays(14), // Default 2 weeks per phase
                ActualEndDate = phase.EndDate,
                TotalAppointments = phaseAppointments.Count,
                CompletedAppointments = phaseAppointments.Count(a => a.Status == AppointmentStatus.Completed),
                TotalTestResults = phaseTestResults.Count,
                CompletedTestResults = phaseTestResults.Count(t => t.Status == TestResultStatus.Completed),
                Instructions = phase.Instructions,
                Notes = phase.Notes,
                LastUpdated = DateTime.UtcNow
            };

            return progressDto;
        }

        public async Task<List<PhaseResponseDto>> GenerateDefaultPhasesAsync(int cycleId, GeneratePhasesDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Validate cycle exists
                var cycle = await _treatmentCycleRepository.GetByIdAsync(cycleId);
                if (cycle == null)
                    throw new ArgumentException("Invalid cycle ID");

                // Check if phases already exist for this cycle
                var existingPhases = await _treatmentPhaseRepository.GetPhasesByCycleIdAsync(cycleId);
                if (existingPhases.Any())
                    throw new InvalidOperationException("Phases already exist for this cycle. Cannot generate default phases.");

                var phasesToCreate = new List<TreatmentPhase>();

                if (dto.TreatmentType == TreatmentType.IUI)
                {
                    phasesToCreate = GenerateIUIPhases(cycleId);
                }
                else if (dto.TreatmentType == TreatmentType.IVF)
                {
                    phasesToCreate = GenerateIVFPhases(cycleId);
                }

                // Add all phases
                foreach (var phase in phasesToCreate)
                {
                    await _treatmentPhaseRepository.AddTreatmentPhaseAsync(phase);
                }

                await _unitOfWork.CommitTransactionAsync();

                var createdPhases = await _treatmentPhaseRepository.GetPhasesByCycleIdAsync(cycleId);
                _logger.LogInformation($"Generated {createdPhases.Count} default phases for {dto.TreatmentType} cycle {cycleId}");

                return _mapper.Map<List<PhaseResponseDto>>(createdPhases.OrderBy(p => p.PhaseOrder));
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        #endregion

        #region Private Helper Methods

        private double CalculateInProgressPercentage(TreatmentPhase phase, List<Appointment> appointments, List<TestResult> testResults)
        {
            if (!phase.StartDate.HasValue)
                return 0;

            var daysSinceStart = (DateTime.UtcNow - phase.StartDate.Value).TotalDays;
            var estimatedDuration = 14; // Default 14 days per phase

            // Base progress on time elapsed
            var timeProgress = Math.Min(daysSinceStart / estimatedDuration * 50, 50); // Max 50% from time

            // Additional progress from appointments and test results
            var appointmentProgress = appointments.Any() 
                ? (appointments.Count(a => a.Status == AppointmentStatus.Completed) / (double)appointments.Count) * 25 
                : 0;

            var testProgress = testResults.Any() 
                ? (testResults.Count(t => t.Status == TestResultStatus.Completed) / (double)testResults.Count) * 25 
                : 0;

            return Math.Min(timeProgress + appointmentProgress + testProgress, 95); // Max 95% for in-progress
        }

        private (List<string> completed, List<string> pending, string current) GeneratePhaseMilestones(
            TreatmentPhase phase, List<Appointment> appointments, List<TestResult> testResults)
        {
            var completed = new List<string>();
            var pending = new List<string>();
            var current = "";

            if (phase.Status == PhaseStatus.Pending)
            {
                current = "Phase not started";
                pending.Add("Start phase");
                pending.Add("Complete initial assessments");
                pending.Add("Complete phase activities");
                pending.Add("Phase review and completion");
            }
            else if (phase.Status == PhaseStatus.InProgress)
            {
                completed.Add("Phase started");
                
                if (appointments.Any(a => a.Status == AppointmentStatus.Completed))
                {
                    completed.Add("Initial assessments completed");
                    current = "Ongoing phase activities";
                }
                else
                {
                    current = "Initial assessments";
                }

                if (appointments.All(a => a.Status == AppointmentStatus.Completed) && testResults.All(t => t.Status == TestResultStatus.Completed))
                {
                    current = "Ready for phase completion";
                    pending.Add("Phase review and completion");
                }
                else
                {
                    pending.Add("Complete phase activities");
                    pending.Add("Phase review and completion");
                }
            }
            else if (phase.Status == PhaseStatus.Completed)
            {
                completed.Add("Phase started");
                completed.Add("Initial assessments completed");
                completed.Add("Phase activities completed");
                completed.Add("Phase review and completion");
                current = "Phase completed";
            }

            return (completed, pending, current);
        }

        private List<TreatmentPhase> GenerateIUIPhases(int cycleId)
        {
            return new List<TreatmentPhase>
            {
                new TreatmentPhase
                {
                    CycleId = cycleId,
                    PhaseName = "Pre-treatment Assessment",
                    PhaseOrder = 1,
                    Status = PhaseStatus.Pending,
                    Instructions = "Initial consultation, baseline tests, and treatment planning",
                    Cost = 500,
                    CreatedAt = DateTime.UtcNow
                },
                new TreatmentPhase
                {
                    CycleId = cycleId,
                    PhaseName = "Ovulation Stimulation",
                    PhaseOrder = 2,
                    Status = PhaseStatus.Pending,
                    Instructions = "Medication administration and monitoring follicle development",
                    Cost = 800,
                    CreatedAt = DateTime.UtcNow
                },
                new TreatmentPhase
                {
                    CycleId = cycleId,
                    PhaseName = "Insemination Procedure",
                    PhaseOrder = 3,
                    Status = PhaseStatus.Pending,
                    Instructions = "Sperm preparation and intrauterine insemination",
                    Cost = 600,
                    CreatedAt = DateTime.UtcNow
                },
                new TreatmentPhase
                {
                    CycleId = cycleId,
                    PhaseName = "Post-Procedure Monitoring",
                    PhaseOrder = 4,
                    Status = PhaseStatus.Pending,
                    Instructions = "Luteal phase support and pregnancy testing",
                    Cost = 300,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        private List<TreatmentPhase> GenerateIVFPhases(int cycleId)
        {
            return new List<TreatmentPhase>
            {
                new TreatmentPhase
                {
                    CycleId = cycleId,
                    PhaseName = "Pre-treatment Assessment",
                    PhaseOrder = 1,
                    Status = PhaseStatus.Pending,
                    Instructions = "Comprehensive evaluation, baseline tests, and treatment planning",
                    Cost = 800,
                    CreatedAt = DateTime.UtcNow
                },
                new TreatmentPhase
                {
                    CycleId = cycleId,
                    PhaseName = "Ovarian Stimulation",
                    PhaseOrder = 2,
                    Status = PhaseStatus.Pending,
                    Instructions = "Controlled ovarian hyperstimulation with monitoring",
                    Cost = 2000,
                    CreatedAt = DateTime.UtcNow
                },
                new TreatmentPhase
                {
                    CycleId = cycleId,
                    PhaseName = "Egg Retrieval",
                    PhaseOrder = 3,
                    Status = PhaseStatus.Pending,
                    Instructions = "Oocyte retrieval procedure under sedation",
                    Cost = 3000,
                    CreatedAt = DateTime.UtcNow
                },
                new TreatmentPhase
                {
                    CycleId = cycleId,
                    PhaseName = "Fertilization & Culture",
                    PhaseOrder = 4,
                    Status = PhaseStatus.Pending,
                    Instructions = "In vitro fertilization and embryo culture",
                    Cost = 2500,
                    CreatedAt = DateTime.UtcNow
                },
                new TreatmentPhase
                {
                    CycleId = cycleId,
                    PhaseName = "Embryo Transfer",
                    PhaseOrder = 5,
                    Status = PhaseStatus.Pending,
                    Instructions = "Transfer of selected embryos to uterus",
                    Cost = 1500,
                    CreatedAt = DateTime.UtcNow
                },
                new TreatmentPhase
                {
                    CycleId = cycleId,
                    PhaseName = "Post-Transfer Monitoring",
                    PhaseOrder = 6,
                    Status = PhaseStatus.Pending,
                    Instructions = "Luteal support and pregnancy monitoring",
                    Cost = 400,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        #endregion
    }
}
