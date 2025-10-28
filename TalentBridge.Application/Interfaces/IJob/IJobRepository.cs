using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBridge.Domain.Entities;

namespace TalentBridge.Application.Interfaces
{
    public interface IJobRepository
    {
        Task<Job?> GetByIdAsync(int id);
        Task<IEnumerable<Job>> GetAllAsync();
        Task<IEnumerable<Job>> GetActiveJobsAsync();
        Task AddAsync(Job job);
        Task UpdateAsync(Job job);
        Task DeleteAsync(Job job);
    }
}
