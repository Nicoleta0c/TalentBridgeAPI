using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBridge.Application.DTOs;

namespace TalentBridge.Application.Interfaces
{
    public interface IJobService
    {
        Task<JobDto?> GetJobByIdAsync(int id);
        Task<IEnumerable<JobDto>> GetAllJobsAsync();
        Task<IEnumerable<JobDto>> GetActiveJobsAsync();
        Task<JobDto> CreateJobAsync(CreateJobDto createJobDto);
        Task<JobDto?> UpdateJobAsync(int id, CreateJobDto updateJobDto);
        Task<bool> DeleteJobAsync(int id);

        Task<JobApplicationDto> ApplyToJobAsync(int userId, ApplyToJobDto applyDto);
        Task<IEnumerable<JobApplicationDto>> GetUserApplicationsAsync(int userId);
        Task<JobApplicationDto?> UpdateApplicationStatusAsync(int applicationId, string status);
    }
}