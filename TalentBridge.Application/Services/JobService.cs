using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentBridge.Application.DTOs.JobDTOs;
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
        private readonly ICVRepository _cvRepository;

        public JobService(
            IJobRepository jobRepository,
            IJobApplicationRepository jobApplicationRepository,
            IUserRepository userRepository,
            ICVRepository cvRepository)
        {
            _jobRepository = jobRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _userRepository = userRepository;
            _cvRepository = cvRepository;
        }

        public async Task<IEnumerable<JobDto>> GetAllJobsAsync(bool activeOnly = true)
        {
            var jobs = activeOnly ?
                await _jobRepository.GetActiveJobsAsync() :
                await _jobRepository.GetAllAsync();

            return jobs.Select(MapToJobDto);
        }

        public async Task<JobDto> GetJobByIdAsync(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
                throw new KeyNotFoundException("Job not found");

            return MapToJobDto(job);
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

        public async Task<bool> UpdateJobAsync(int id, UpdateJobDto updateJobDto)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null) return false;

            if (!string.IsNullOrEmpty(updateJobDto.Title))
                job.Title = updateJobDto.Title;

            if (!string.IsNullOrEmpty(updateJobDto.Company))
                job.Company = updateJobDto.Company;

            if (!string.IsNullOrEmpty(updateJobDto.Description))
                job.Description = updateJobDto.Description;

            if (!string.IsNullOrEmpty(updateJobDto.Requirements))
                job.Requirements = updateJobDto.Requirements;

            if (!string.IsNullOrEmpty(updateJobDto.Location))
                job.Location = updateJobDto.Location;

            if (!string.IsNullOrEmpty(updateJobDto.SalaryRange))
                job.SalaryRange = updateJobDto.SalaryRange;

            if (!string.IsNullOrEmpty(updateJobDto.JobType))
                job.JobType = updateJobDto.JobType;

            job.IsActive = updateJobDto.IsActive;
            job.UpdatedAt = DateTime.UtcNow;

            await _jobRepository.UpdateAsync(job);
            return true;
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

            var cv = await _cvRepository.GetByIdAsync(applyDto.CVId);
            if (cv == null || cv.UserId != userId || !cv.IsActive)
                throw new InvalidOperationException("CV no válido o no disponible");

            var application = new JobApplication
            {
                JobId = applyDto.JobId,
                UserId = userId,
                CVId = applyDto.CVId,
                CoverLetter = applyDto.CoverLetter ?? string.Empty,
                Status = "Applied",
                AppliedDate = DateTime.UtcNow
            };

            await _jobApplicationRepository.AddAsync(application);

            var user = await _userRepository.GetByIdAsync(userId);

            return MapToJobApplicationDto(application, job, user, cv);
        }

        public async Task<IEnumerable<JobApplicationDto>> GetUserApplicationsAsync(int userId)
        {
            var applications = await _jobApplicationRepository.GetByUserIdAsync(userId);
            var result = new List<JobApplicationDto>();

            foreach (var application in applications)
            {
                var job = application.Job;
                var user = application.User;
                var cv = await _cvRepository.GetByIdAsync(application.CVId);

                result.Add(MapToJobApplicationDto(application, job, user, cv));
            }

            return result;
        }

        public async Task<JobApplicationDto?> UpdateApplicationStatusAsync(int applicationId, string status)
        {
            var application = await _jobApplicationRepository.GetByIdAsync(applicationId);
            if (application == null) return null;

            application.Status = status;
            await _jobApplicationRepository.UpdateAsync(application);

            var job = application.Job;
            var user = application.User;
            var cv = await _cvRepository.GetByIdAsync(application.CVId);

            return MapToJobApplicationDto(application, job, user, cv);
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
                PostedDate = job.PostedDate,
                UpdatedAt = job.UpdatedAt,
                ApplicationCount = 0 
            };
        }

        private static JobApplicationDto MapToJobApplicationDto(
            JobApplication application,
            Job? job,
            User? user,
            CV? cv)
        {
            return new JobApplicationDto
            {
                Id = application.Id,
                JobId = application.JobId,
                UserId = application.UserId,
                CVId = application.CVId,
                Status = application.Status,
                AppliedDate = application.AppliedDate,
                CoverLetter = application.CoverLetter ?? string.Empty,
                JobTitle = job?.Title ?? string.Empty,
                CompanyName = job?.Company ?? string.Empty, 
                UserFullName = user?.FullName ?? string.Empty, 
                UserEmail = user?.Email ?? string.Empty,
                CVFileName = cv?.FileName ?? string.Empty
            };
        }
    }
}