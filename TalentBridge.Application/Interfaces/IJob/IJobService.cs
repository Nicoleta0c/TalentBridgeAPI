using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBridge.Application.DTOs.JobDTOs;

namespace TalentBridge.Application.Interfaces
{
    public interface IJobService
    {
        Task<IEnumerable<JobDto>> GetAllJobsAsync(bool activeOnly = true);
        Task<JobDto> GetJobByIdAsync(int id);
        Task<JobDto> CreateJobAsync(CreateJobDto createJobDto);
        Task<bool> UpdateJobAsync(int id, UpdateJobDto updateJobDto); 
        Task<bool> DeleteJobAsync(int id);
        Task<JobApplicationDto> ApplyToJobAsync(int userId, ApplyToJobDto applyDto);
        Task<IEnumerable<JobApplicationDto>> GetUserApplicationsAsync(int userId);
        Task<JobApplicationDto?> UpdateApplicationStatusAsync(int applicationId, string status);
    }
}