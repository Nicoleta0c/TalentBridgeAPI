using Microsoft.EntityFrameworkCore;
using TalentBridge.API.Interfaces.IMentorship;
using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories.MentorshipRepositories
{
    public class MentorshipRepository : IMentorshipRepository
    {
        private readonly ApplicationDbContext _context;

        public MentorshipRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Mentorship?> GetByIdAsync(int id)
        {
            return await _context.Mentorships
                .Include(m => m.Mentor)
                .Include(m => m.Mentee)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Mentorship?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Mentorships
                .Include(m => m.Mentor)
                .Include(m => m.Mentee)
                .Include(m => m.Sessions.Where(s => s.Status != SessionStatus.Cancelled))
                .Include(m => m.Milestones)
                .Include(m => m.Resources.Where(r => r.IsPublic))
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Mentorship>> GetAllAsync()
        {
            return await _context.Mentorships
                .Include(m => m.Mentor)
                .Include(m => m.Mentee)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mentorship>> GetByMentorIdAsync(int mentorId)
        {
            return await _context.Mentorships
                .Where(m => m.MentorId == mentorId)
                .Include(m => m.Mentee)
                .Include(m => m.Sessions)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mentorship>> GetByMenteeIdAsync(int menteeId)
        {
            return await _context.Mentorships
                .Where(m => m.MenteeId == menteeId)
                .Include(m => m.Mentor)
                .Include(m => m.Sessions)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mentorship>> GetByUserIdAsync(int userId)
        {
            return await _context.Mentorships
                .Where(m => m.MentorId == userId || m.MenteeId == userId)
                .Include(m => m.Mentor)
                .Include(m => m.Mentee)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mentorship>> GetActiveMentorshipsAsync()
        {
            return await _context.Mentorships
                .Where(m => m.Status == MentorshipStatus.Active)
                .Include(m => m.Mentor)
                .Include(m => m.Mentee)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mentorship>> GetCompletedMentorshipsAsync()
        {
            return await _context.Mentorships
                .Where(m => m.Status == MentorshipStatus.Completed)
                .Include(m => m.Mentor)
                .Include(m => m.Mentee)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mentorship>> GetPendingMentorshipsAsync()
        {
            return await _context.Mentorships
                .Where(m => m.Status == MentorshipStatus.Pending || m.Status == MentorshipStatus.Requested)
                .Include(m => m.Mentor)
                .Include(m => m.Mentee)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Mentorship> CreateAsync(Mentorship mentorship)
        {
            _context.Mentorships.Add(mentorship);
            await _context.SaveChangesAsync();
            return mentorship;
        }

        public async Task<Mentorship> UpdateAsync(Mentorship mentorship)
        {
            mentorship.UpdatedAt = DateTime.UtcNow;
            _context.Mentorships.Update(mentorship);
            await _context.SaveChangesAsync();
            return mentorship;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var mentorship = await GetByIdAsync(id);
            if (mentorship == null) return false;

            _context.Mentorships.Remove(mentorship);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Mentorships.AnyAsync(m => m.Id == id);
        }

        public async Task<int> CountActiveMentorshipsAsync(int mentorId)
        {
            return await _context.Mentorships
                .CountAsync(m => m.MentorId == mentorId && m.Status == MentorshipStatus.Active);
        }

        public async Task<int> CountCompletedMentorshipsAsync(int mentorId)
        {
            return await _context.Mentorships
                .CountAsync(m => m.MentorId == mentorId && m.Status == MentorshipStatus.Completed);
        }

        public async Task<int> CountTotalMenteesAsync(int mentorId)
        {
            return await _context.Mentorships
                .Where(m => m.MentorId == mentorId)
                .Select(m => m.MenteeId)
                .Distinct()
                .CountAsync();
        }

        public async Task<decimal> CalculateMentorRatingAsync(int mentorId)
        {
            var ratings = await _context.Mentorships
                .Where(m => m.MentorId == mentorId && m.MenteeRating.HasValue)
                .Select(m => m.MenteeRating!.Value)
                .ToListAsync();

            return ratings.Any() ? ratings.Average() : 0;
        }

        public async Task<bool> ChangeStatusAsync(int id, MentorshipStatus status)
        {
            var mentorship = await GetByIdAsync(id);
            if (mentorship == null) return false;

            mentorship.Status = status;
            mentorship.UpdatedAt = DateTime.UtcNow;

            if (status == MentorshipStatus.Active && !mentorship.StartDate.HasValue)
            {
                mentorship.StartDate = DateTime.UtcNow;
            }
            else if (status == MentorshipStatus.Completed && !mentorship.EndDate.HasValue)
            {
                mentorship.EndDate = DateTime.UtcNow;
                mentorship.ClosedAt = DateTime.UtcNow;
            }

            await UpdateAsync(mentorship);
            return true;
        }

        public async Task<bool> AddRatingAsync(int id, decimal mentorRating, decimal? menteeRating, string? feedback)
        {
            var mentorship = await GetByIdAsync(id);
            if (mentorship == null) return false;

            mentorship.MentorRating = mentorRating;
            mentorship.MenteeRating = menteeRating;
            mentorship.MentorFeedback = feedback;
            mentorship.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(mentorship);
            return true;
        }

        public async Task<bool> CompleteMentorshipAsync(int id)
        {
            return await ChangeStatusAsync(id, MentorshipStatus.Completed);
        }

        public async Task<bool> CancelMentorshipAsync(int id, string reason)
        {
            return await ChangeStatusAsync(id, MentorshipStatus.Cancelled);
        }
    }
}