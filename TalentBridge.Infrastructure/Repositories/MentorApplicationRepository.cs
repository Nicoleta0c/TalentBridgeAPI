using Microsoft.EntityFrameworkCore;
using TalentBridge.API.Interfaces.IMentorship;
using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories.MentorshipRepositories
{
    public class MentorApplicationRepository : IMentorApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public MentorApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MentorApplication?> GetByIdAsync(int id)
        {
            return await _context.MentorApplications
                .Include(a => a.Request)
                    .ThenInclude(r => r.Mentee)
                .Include(a => a.Mentor)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<MentorApplication>> GetByRequestIdAsync(int requestId)
        {
            return await _context.MentorApplications
                .Where(a => a.RequestId == requestId)
                .Include(a => a.Mentor)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorApplication>> GetByMentorIdAsync(int mentorId)
        {
            return await _context.MentorApplications
                .Where(a => a.MentorId == mentorId)
                .Include(a => a.Request)
                    .ThenInclude(r => r.Mentee)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorApplication>> GetByStatusAsync(ApplicationStatus status)
        {
            return await _context.MentorApplications
                .Where(a => a.Status == status)
                .Include(a => a.Request)
                .Include(a => a.Mentor)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<MentorApplication?> GetUserApplicationAsync(int requestId, int mentorId)
        {
            return await _context.MentorApplications
                .Include(a => a.Request)
                .Include(a => a.Mentor)
                .FirstOrDefaultAsync(a => a.RequestId == requestId && a.MentorId == mentorId);
        }

        public async Task<MentorApplication> CreateAsync(MentorApplication application)
        {
            _context.MentorApplications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<MentorApplication> UpdateAsync(MentorApplication application)
        {
            application.UpdatedAt = DateTime.UtcNow;
            _context.MentorApplications.Update(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var application = await GetByIdAsync(id);
            if (application == null) return false;

            _context.MentorApplications.Remove(application);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeStatusAsync(int id, ApplicationStatus status, string? rejectionReason = null)
        {
            var application = await GetByIdAsync(id);
            if (application == null) return false;

            application.Status = status;
            application.RejectionReason = rejectionReason;
            application.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(application);
            return true;
        }

        public async Task<bool> WithdrawApplicationAsync(int id)
        {
            var application = await GetByIdAsync(id);
            if (application == null) return false;

            application.Status = ApplicationStatus.Withdrawn;
            application.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(application);
            return true;
        }

        public async Task<int> CountByRequestIdAsync(int requestId)
        {
            return await _context.MentorApplications
                .CountAsync(a => a.RequestId == requestId);
        }

        public async Task<int> CountByMentorIdAsync(int mentorId)
        {
            return await _context.MentorApplications
                .CountAsync(a => a.MentorId == mentorId);
        }
    }
}