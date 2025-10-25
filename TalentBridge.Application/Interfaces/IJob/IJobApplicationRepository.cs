using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBridge.Domain.Entities;

namespace TalentBridge.Application.Interfaces
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication?> GetByIdAsync(int id);
        Task<IEnumerable<JobApplication>> GetByUserIdAsync(int userId);
        Task<IEnumerable<JobApplication>> GetByJobIdAsync(int jobId);
        Task<bool> HasUserAppliedAsync(int userId, int jobId);
        Task AddAsync(JobApplication application);
        Task UpdateAsync(JobApplication application);
    }
}