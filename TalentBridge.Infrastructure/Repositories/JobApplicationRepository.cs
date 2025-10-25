using Microsoft.EntityFrameworkCore;
using TalentBridge.Application.Interfaces;
using TalentBridge.Domain.Entities;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public JobApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            return await _context.JobApplications
                .Include(ja => ja.Job)
                .Include(ja => ja.User)
                .FirstOrDefaultAsync(ja => ja.Id == id);
        }

        public async Task<IEnumerable<JobApplication>> GetByUserIdAsync(int userId)
        {
            return await _context.JobApplications
                .Include(ja => ja.Job)
                .Where(ja => ja.UserId == userId)
                .OrderByDescending(ja => ja.AppliedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobApplication>> GetByJobIdAsync(int jobId)
        {
            return await _context.JobApplications
                .Include(ja => ja.User)
                .Where(ja => ja.JobId == jobId)
                .OrderByDescending(ja => ja.AppliedDate)
                .ToListAsync();
        }

        public async Task<bool> HasUserAppliedAsync(int userId, int jobId)
        {
            return await _context.JobApplications
                .AnyAsync(ja => ja.UserId == userId && ja.JobId == jobId);
        }

        public async Task AddAsync(JobApplication application)
        {
            await _context.JobApplications.AddAsync(application);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobApplication application)
        {
            _context.JobApplications.Update(application);
            await _context.SaveChangesAsync();
        }
    }
}