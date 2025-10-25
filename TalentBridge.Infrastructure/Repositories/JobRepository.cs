using Microsoft.EntityFrameworkCore;
using TalentBridge.Application.Interfaces;
using TalentBridge.Domain.Entities;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _context;

        public JobRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Job?> GetByIdAsync(int id)
        {
            return await _context.Jobs.FindAsync(id);
        }

        public async Task<IEnumerable<Job>> GetAllAsync()
        {
            return await _context.Jobs
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetActiveJobsAsync()
        {
            return await _context.Jobs
                .Where(j => j.IsActive)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();
        }

        public async Task AddAsync(Job job)
        {
            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Job job)
        {
            job.UpdatedAt = DateTime.UtcNow;
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Job job)
        {
            job.IsActive = false;
            await UpdateAsync(job);
        }
    }
}