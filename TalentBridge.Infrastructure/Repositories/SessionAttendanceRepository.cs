using Microsoft.EntityFrameworkCore;
using TalentBridge.API.Interfaces.IMentorship;
using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories.MentorshipRepositories
{
    public class SessionAttendanceRepository : ISessionAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public SessionAttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SessionAttendance?> GetByIdAsync(int id)
        {
            return await _context.SessionAttendances
                .Include(a => a.User)
                .Include(a => a.Session)
                    .ThenInclude(s => s.Mentorship)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<SessionAttendance?> GetBySessionAndUserAsync(int sessionId, int userId)
        {
            return await _context.SessionAttendances
                .Include(a => a.User)
                .Include(a => a.Session)
                .FirstOrDefaultAsync(a => a.SessionId == sessionId && a.UserId == userId);
        }

        public async Task<IEnumerable<SessionAttendance>> GetBySessionIdAsync(int sessionId)
        {
            return await _context.SessionAttendances
                .Where(a => a.SessionId == sessionId)
                .Include(a => a.User)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SessionAttendance>> GetByUserIdAsync(int userId)
        {
            return await _context.SessionAttendances
                .Where(a => a.UserId == userId)
                .Include(a => a.Session)
                    .ThenInclude(s => s.Mentorship)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SessionAttendance>> GetByMentorshipIdAsync(int mentorshipId)
        {
            return await _context.SessionAttendances
                .Where(a => a.Session.MentorshipId == mentorshipId)
                .Include(a => a.User)
                .Include(a => a.Session)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<SessionAttendance> CreateAsync(SessionAttendance attendance)
        {
            _context.SessionAttendances.Add(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task<SessionAttendance> UpdateAsync(SessionAttendance attendance)
        {
            attendance.UpdatedAt = DateTime.UtcNow;
            _context.SessionAttendances.Update(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var attendance = await GetByIdAsync(id);
            if (attendance == null) return false;

            _context.SessionAttendances.Remove(attendance);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAttendanceAsync(int sessionId, int userId, AttendanceStatus status)
        {
            var attendance = await GetBySessionAndUserAsync(sessionId, userId);
            if (attendance == null) return false;

            attendance.Status = status;
            attendance.UpdatedAt = DateTime.UtcNow;

            if (status == AttendanceStatus.Attended && !attendance.JoinedAt.HasValue)
            {
                attendance.JoinedAt = DateTime.UtcNow;
            }

            await UpdateAsync(attendance);
            return true;
        }

        public async Task<bool> JoinSessionAsync(int sessionId, int userId)
        {
            var attendance = await GetBySessionAndUserAsync(sessionId, userId);
            if (attendance == null) return false;

            attendance.Status = AttendanceStatus.Attended;
            attendance.JoinedAt = DateTime.UtcNow;
            attendance.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(attendance);
            return true;
        }

        public async Task<bool> LeaveSessionAsync(int sessionId, int userId)
        {
            var attendance = await GetBySessionAndUserAsync(sessionId, userId);
            if (attendance == null || attendance.Status != AttendanceStatus.Attended) return false;

            attendance.LeftAt = DateTime.UtcNow;
            attendance.UpdatedAt = DateTime.UtcNow;

            if (attendance.JoinedAt.HasValue)
            {
                var duration = attendance.LeftAt.Value - attendance.JoinedAt.Value;
                attendance.AttendanceMinutes = (int)duration.TotalMinutes;
            }

            await UpdateAsync(attendance);
            return true;
        }

        public async Task<bool> AddFeedbackAsync(int sessionId, int userId, int rating, string feedback)
        {
            var attendance = await GetBySessionAndUserAsync(sessionId, userId);
            if (attendance == null) return false;

            attendance.Rating = Math.Clamp(rating, 1, 5);
            attendance.Feedback = feedback;
            attendance.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(attendance);
            return true;
        }

        public async Task<decimal> CalculateAttendanceRateAsync(int mentorshipId, int userId)
        {
            var totalSessions = await _context.MentorshipSessions
                .CountAsync(s => s.MentorshipId == mentorshipId &&
                               s.Status == SessionStatus.Completed);

            if (totalSessions == 0) return 0;

            var attendedSessions = await _context.SessionAttendances
                .CountAsync(a => a.Session.MentorshipId == mentorshipId &&
                               a.UserId == userId &&
                               a.Status == AttendanceStatus.Attended);

            return (decimal)attendedSessions / totalSessions * 100;
        }

        public async Task<int> CalculateTotalAttendanceMinutesAsync(int mentorshipId, int userId)
        {
            return await _context.SessionAttendances
                .Where(a => a.Session.MentorshipId == mentorshipId &&
                           a.UserId == userId &&
                           a.Status == AttendanceStatus.Attended)
                .SumAsync(a => a.AttendanceMinutes);
        }
    }
}