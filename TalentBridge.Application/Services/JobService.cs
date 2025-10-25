using TalentBridge.Application.DTOs;
using TalentBridge.Application.Interfaces;
using TalentBridge.Application.Interfaces.IUser;
using TalentBridge.Domain.Entities;

namespace TalentBridge.Application.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IUserRepository _userRepository;

        public JobService(
            IJobRepository jobRepository,
            IJobApplicationRepository jobApplicationRepository,
            IUserRepository userRepository)
        {
            _jobRepository = jobRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _userRepository = userRepository;
        }

        public async Task<JobDto?> GetJobByIdAsync(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            return job != null ? MapToJobDto(job) : null;
        }

        public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
        {
            var jobs = await _jobRepository.GetAllAsync();
            return jobs.Select(MapToJobDto);
        }

        public async Task<IEnumerable<JobDto>> GetActiveJobsAsync()
        {
            var jobs = await _jobRepository.GetActiveJobsAsync();
            return jobs.Select(MapToJobDto);
        }

        public async Task<JobDto> CreateJobAsync(CreateJobDto createJobDto)
        {
            var job = new Job
            {
                Title = createJobDto.Title,
                Company = createJobDto.Company,
                Description = createJobDto.Description,
                Requirements = createJobDto.Requirements,
                Location = createJobDto.Location,
                SalaryRange = createJobDto.SalaryRange,
                JobType = createJobDto.JobType,
                IsActive = true,
                PostedDate = DateTime.UtcNow
            };

            await _jobRepository.AddAsync(job);
            return MapToJobDto(job);
        }

        public async Task<JobDto?> UpdateJobAsync(int id, CreateJobDto updateJobDto)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null) return null;

            job.Title = updateJobDto.Title;
            job.Company = updateJobDto.Company;
            job.Description = updateJobDto.Description;
            job.Requirements = updateJobDto.Requirements;
            job.Location = updateJobDto.Location;
            job.SalaryRange = updateJobDto.SalaryRange;
            job.JobType = updateJobDto.JobType;
            job.UpdatedAt = DateTime.UtcNow;

            await _jobRepository.UpdateAsync(job);
            return MapToJobDto(job);
        }

        public async Task<bool> DeleteJobAsync(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null) return false;

            await _jobRepository.DeleteAsync(job);
            return true;
        }

        public async Task<JobApplicationDto> ApplyToJobAsync(int userId, ApplyToJobDto applyDto)
        {
            var hasApplied = await _jobApplicationRepository.HasUserAppliedAsync(userId, applyDto.JobId);
            if (hasApplied)
                throw new InvalidOperationException("Ya has aplicado a esta vacante");

            var job = await _jobRepository.GetByIdAsync(applyDto.JobId);
            if (job == null || !job.IsActive)
                throw new InvalidOperationException("La vacante no existe o no está disponible");

            var application = new JobApplication
            {
                JobId = applyDto.JobId,
                UserId = userId,
                CVId = applyDto.CVId,
                CoverLetter = applyDto.CoverLetter,
                Status = "Applied",
                AppliedDate = DateTime.UtcNow
            };

            await _jobApplicationRepository.AddAsync(application);

            var user = await _userRepository.GetByIdAsync(userId);

            return new JobApplicationDto
            {
                Id = application.Id,
                JobId = application.JobId,
                UserId = application.UserId,
                CVId = application.CVId,
                Status = application.Status,
                AppliedDate = application.AppliedDate,
                CoverLetter = application.CoverLetter,
                JobTitle = job.Title,
                Company = job.Company,
                UserName = user?.FullName ?? "Usuario"
            };
        }

        public async Task<IEnumerable<JobApplicationDto>> GetUserApplicationsAsync(int userId)
        {
            var applications = await _jobApplicationRepository.GetByUserIdAsync(userId);
            return applications.Select(MapToJobApplicationDto);
        }

        public async Task<JobApplicationDto?> UpdateApplicationStatusAsync(int applicationId, string status)
        {
            var application = await _jobApplicationRepository.GetByIdAsync(applicationId);
            if (application == null) return null;

            application.Status = status;
            await _jobApplicationRepository.UpdateAsync(application);

            return MapToJobApplicationDto(application);
        }

        private static JobDto MapToJobDto(Job job)
        {
            return new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Company = job.Company,
                Description = job.Description,
                Requirements = job.Requirements,
                Location = job.Location,
                SalaryRange = job.SalaryRange,
                JobType = job.JobType,
                IsActive = job.IsActive,
                PostedDate = job.PostedDate
            };
        }

        private static JobApplicationDto MapToJobApplicationDto(JobApplication application)
        {
            return new JobApplicationDto
            {
                Id = application.Id,
                JobId = application.JobId,
                UserId = application.UserId,
                CVId = application.CVId,
                Status = application.Status,
                AppliedDate = application.AppliedDate,
                CoverLetter = application.CoverLetter,
                JobTitle = application.Job?.Title ?? string.Empty,
                Company = application.Job?.Company ?? string.Empty,
                UserName = application.User?.FullName ?? string.Empty
            };
        }
    }
}