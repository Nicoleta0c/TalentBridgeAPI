using Microsoft.EntityFrameworkCore;
using TalentBridge.API.Interfaces.IMentorship;
using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories.MentorshipRepositories
{
    public class MentorshipMilestoneRepository : IMentorshipMilestoneRepository
    {
        private readonly ApplicationDbContext _context;

        public MentorshipMilestoneRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MentorshipMilestone?> GetByIdAsync(int id)
        {
            return await _context.MentorshipMilestones
                .Include(m => m.Mentorship)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<MentorshipMilestone>> GetByMentorshipIdAsync(int mentorshipId)
        {
            return await _context.MentorshipMilestones
                .Where(m => m.MentorshipId == mentorshipId)
                .OrderBy(m => m.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipMilestone>> GetByStatusAsync(int mentorshipId, MilestoneStatus status)
        {
            return await _context.MentorshipMilestones
                .Where(m => m.MentorshipId == mentorshipId && m.Status == status)
                .OrderBy(m => m.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipMilestone>> GetUpcomingMilestonesAsync(int mentorshipId)
        {
            var today = DateTime.UtcNow.Date; // Usa DateTime en lugar de DateOnly
            return await _context.MentorshipMilestones
                .Where(m => m.MentorshipId == mentorshipId &&
                           m.Status == MilestoneStatus.Pending &&
                           m.DueDate >= today)
                .OrderBy(m => m.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipMilestone>> GetOverdueMilestonesAsync(int mentorshipId)
        {
            var today = DateTime.UtcNow.Date; // Usa DateTime en lugar de DateOnly
            return await _context.MentorshipMilestones
                .Where(m => m.MentorshipId == mentorshipId &&
                           m.Status == MilestoneStatus.Pending &&
                           m.DueDate < today)
                .OrderBy(m => m.DueDate)
                .ToListAsync();
        }

        public async Task<MentorshipMilestone> CreateAsync(MentorshipMilestone milestone)
        {
            _context.MentorshipMilestones.Add(milestone);
            await _context.SaveChangesAsync();
            return milestone;
        }

        public async Task<MentorshipMilestone> UpdateAsync(MentorshipMilestone milestone)
        {
            milestone.UpdatedAt = DateTime.UtcNow;
            _context.MentorshipMilestones.Update(milestone);
            await _context.SaveChangesAsync();
            return milestone;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var milestone = await GetByIdAsync(id);
            if (milestone == null) return false;

            _context.MentorshipMilestones.Remove(milestone);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkCompleteAsync(int id, DateTime completedDate, string? evidence)
        {
            var milestone = await GetByIdAsync(id);
            if (milestone == null) return false;

            milestone.Status = MilestoneStatus.Completed;
            milestone.CompletedDate = completedDate;
            milestone.ProgressPercentage = 100;
            milestone.Evidence = evidence;
            milestone.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(milestone);
            return true;
        }

        public async Task<bool> UpdateProgressAsync(int id, int progressPercentage, string? notes)
        {
            var milestone = await GetByIdAsync(id);
            if (milestone == null) return false;

            milestone.ProgressPercentage = Math.Clamp(progressPercentage, 0, 100);
            milestone.Notes = notes;
            milestone.UpdatedAt = DateTime.UtcNow;

            if (milestone.ProgressPercentage == 100)
            {
                milestone.Status = MilestoneStatus.Completed;
                milestone.CompletedDate = DateTime.UtcNow;
            }
            else if (milestone.ProgressPercentage > 0)
            {
                milestone.Status = MilestoneStatus.InProgress;
            }

            await UpdateAsync(milestone);
            return true;
        }

        public async Task<int> CountMilestonesAsync(int mentorshipId)
        {
            return await _context.MentorshipMilestones
                .CountAsync(m => m.MentorshipId == mentorshipId);
        }

        public async Task<int> CountCompletedMilestonesAsync(int mentorshipId)
        {
            return await _context.MentorshipMilestones
                .CountAsync(m => m.MentorshipId == mentorshipId && m.Status == MilestoneStatus.Completed);
        }

        public async Task<decimal> CalculateProgressPercentageAsync(int mentorshipId)
        {
            var totalMilestones = await CountMilestonesAsync(mentorshipId);
            if (totalMilestones == 0) return 0;

            var totalProgress = await _context.MentorshipMilestones
                .Where(m => m.MentorshipId == mentorshipId)
                .SumAsync(m => m.ProgressPercentage);

            return (decimal)totalProgress / totalMilestones;
        }
    }
}