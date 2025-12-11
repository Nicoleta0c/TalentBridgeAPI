using FluentValidation;
using TalentBridge.Application.DTOs.JobDTOs;
using TalentBridge.Application.Interfaces;

namespace TalentBridge.Application.Validations
{
    public class ApplyToJobValidator : AbstractValidator<ApplyToJobDto>
    {
        private readonly IJobRepository _jobRepository;
        private readonly ICVRepository _cvRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;

        public ApplyToJobValidator(
            IJobRepository jobRepository,
            ICVRepository cvRepository,
            IJobApplicationRepository jobApplicationRepository)
        {
            _jobRepository = jobRepository;
            _cvRepository = cvRepository;
            _jobApplicationRepository = jobApplicationRepository;

            RuleFor(x => x.JobId)
                .GreaterThan(0).WithMessage("Valid job ID is required")
                .MustAsync(JobExistsAndIsActive).WithMessage("Job not found or is inactive");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Valid user ID is required");

            RuleFor(x => x.CVId)
                .GreaterThan(0).WithMessage("Valid CV ID is required")
                .MustAsync(CVExistsAndBelongsToUser).WithMessage("CV not found or does not belong to user");

            RuleFor(x => x.CoverLetter)
                .MaximumLength(2000).WithMessage("Cover letter cannot exceed 2000 characters")
                .When(x => !string.IsNullOrEmpty(x.CoverLetter));

            RuleFor(x => x)
                .MustAsync(UserHasNotAlreadyApplied).WithMessage("User has already applied to this job");
        }

        private async Task<bool> JobExistsAndIsActive(int jobId, CancellationToken cancellationToken)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            return job != null && job.IsActive;
        }

        private async Task<bool> CVExistsAndBelongsToUser(ApplyToJobDto dto, int cvId, CancellationToken cancellationToken)
        {
            var cv = await _cvRepository.GetByIdAsync(cvId);
            return cv != null && cv.UserId == dto.UserId && cv.IsActive;
        }

        private async Task<bool> UserHasNotAlreadyApplied(ApplyToJobDto dto, CancellationToken cancellationToken)
        {
            var hasApplied = await _jobApplicationRepository.HasUserAppliedAsync(dto.UserId, dto.JobId);
            return !hasApplied;
        }
    }
}