using Microsoft.EntityFrameworkCore;
using TalentBridge.API.Interfaces.IMentorship;
using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories.MentorshipRepositories
{
    public class MentorshipSessionRepository : IMentorshipSessionRepository
    {
        private readonly ApplicationDbContext _context;

        public MentorshipSessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MentorshipSession?> GetByIdAsync(int id)
        {
            return await _context.MentorshipSessions
                .Include(s => s.Mentorship)
                    .ThenInclude(m => m.Mentor)
                .Include(s => s.Mentorship)
                    .ThenInclude(m => m.Mentee)
                .Include(s => s.Attendances)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<MentorshipSession?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.MentorshipSessions
                .Include(s => s.Mentorship)
                .Include(s => s.Attendances)
                .Include(s => s.Resources)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<MentorshipSession>> GetByMentorshipIdAsync(int mentorshipId)
        {
            return await _context.MentorshipSessions
                .Where(s => s.MentorshipId == mentorshipId)
                .Include(s => s.Attendances)
                .OrderByDescending(s => s.ScheduledDate)
                .ThenByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipSession>> GetByUserIdAsync(int userId)
        {
            return await _context.MentorshipSessions
                .Where(s => s.Mentorship.MentorId == userId || s.Mentorship.MenteeId == userId)
                .Include(s => s.Mentorship)
                .Include(s => s.Attendances)
                .OrderByDescending(s => s.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipSession>> GetUpcomingSessionsAsync(int mentorshipId)
        {
            var now = DateTime.UtcNow;
            return await _context.MentorshipSessions
                .Where(s => s.MentorshipId == mentorshipId &&
                           s.Status != SessionStatus.Cancelled &&
                           s.Status != SessionStatus.Completed &&
                           (s.ScheduledDate > now.Date ||
                            (s.ScheduledDate.Date == now.Date && s.StartTime > now)))
                .Include(s => s.Mentorship)
                .OrderBy(s => s.ScheduledDate)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipSession>> GetPastSessionsAsync(int mentorshipId)
        {
            var now = DateTime.UtcNow;
            return await _context.MentorshipSessions
                .Where(s => s.MentorshipId == mentorshipId &&
                           (s.Status == SessionStatus.Completed ||
                            s.ScheduledDate.Date < now.Date ||
                            (s.ScheduledDate.Date == now.Date && s.EndTime < now)))
                .Include(s => s.Mentorship)
                .Include(s => s.Attendances)
                .OrderByDescending(s => s.ScheduledDate)
                .ThenByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipSession>> GetSessionsByDateRangeAsync(int mentorshipId, DateTime startDate, DateTime endDate)
        {
            return await _context.MentorshipSessions
                .Where(s => s.MentorshipId == mentorshipId &&
                           s.ScheduledDate.Date >= startDate.Date &&
                           s.ScheduledDate.Date <= endDate.Date)
                .Include(s => s.Mentorship)
                .Include(s => s.Attendances)
                .OrderBy(s => s.ScheduledDate)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<MentorshipSession> CreateAsync(MentorshipSession session)
        {
            _context.MentorshipSessions.Add(session);
            await _context.SaveChangesAsync();

            // Crear automáticamente registros de asistencia
            var mentorship = await _context.Mentorships
                .Include(m => m.Mentor)
                .Include(m => m.Mentee)
                .FirstOrDefaultAsync(m => m.Id == session.MentorshipId);

            if (mentorship != null)
            {
                // Agregar asistencia del mentor
                var mentorAttendance = new SessionAttendance
                {
                    SessionId = session.Id,
                    UserId = mentorship.MentorId,
                    Status = AttendanceStatus.Invited,
                    CreatedAt = DateTime.UtcNow
                };
                _context.SessionAttendances.Add(mentorAttendance);

                // Agregar asistencia del mentee
                var menteeAttendance = new SessionAttendance
                {
                    SessionId = session.Id,
                    UserId = mentorship.MenteeId,
                    Status = AttendanceStatus.Invited,
                    CreatedAt = DateTime.UtcNow
                };
                _context.SessionAttendances.Add(menteeAttendance);

                await _context.SaveChangesAsync();
            }

            return session;
        }

        public async Task<MentorshipSession> UpdateAsync(MentorshipSession session)
        {
            session.UpdatedAt = DateTime.UtcNow;
            _context.MentorshipSessions.Update(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var session = await GetByIdAsync(id);
            if (session == null) return false;

            session.Status = SessionStatus.Cancelled;
            session.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(session);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.MentorshipSessions.AnyAsync(s => s.Id == id);
        }

        public async Task<bool> StartSessionAsync(int id)
        {
            var session = await GetByIdAsync(id);
            if (session == null || session.Status != SessionStatus.Scheduled) return false;

            session.Status = SessionStatus.InProgress;
            session.StartedAt = DateTime.UtcNow;
            await UpdateAsync(session);
            return true;
        }

        public async Task<bool> EndSessionAsync(int id)
        {
            var session = await GetByIdAsync(id);
            if (session == null || session.Status != SessionStatus.InProgress) return false;

            session.Status = SessionStatus.Completed;
            session.EndedAt = DateTime.UtcNow;

            // Calcular duración si se había iniciado
            if (session.StartedAt.HasValue)
            {
                var duration = session.EndedAt.Value - session.StartedAt.Value;
                // Actualizar tiempo de asistencia para los usuarios
                var attendances = await _context.SessionAttendances
                    .Where(a => a.SessionId == id && a.Status == AttendanceStatus.Attended)
                    .ToListAsync();

                foreach (var attendance in attendances)
                {
                    attendance.AttendanceMinutes = (int)duration.TotalMinutes;
                    attendance.LeftAt = session.EndedAt.Value;
                }
            }

            await UpdateAsync(session);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelSessionAsync(int id, string reason)
        {
            var session = await GetByIdAsync(id);
            if (session == null) return false;

            session.Status = SessionStatus.Cancelled;
            session.Notes = reason;
            session.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(session);
            return true;
        }

        public async Task<bool> RescheduleSessionAsync(int id, DateTime newDate, DateTime newStartTime)
        {
            var session = await GetByIdAsync(id);
            if (session == null) return false;

            // Calcular la duración original de la sesión
            var originalDuration = session.EndTime - session.StartTime;

            session.ScheduledDate = newDate;
            session.StartTime = newStartTime;
            session.EndTime = newStartTime.Add(originalDuration);
            session.Status = SessionStatus.Rescheduled;
            session.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(session);
            return true;
        }

        public async Task<bool> AddSessionNotesAsync(int id, string notes, string? homework)
        {
            var session = await GetByIdAsync(id);
            if (session == null) return false;

            session.Notes = notes;
            session.Homework = homework;
            session.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(session);
            return true;
        }

        public async Task<int> CountCompletedSessionsAsync(int mentorshipId)
        {
            return await _context.MentorshipSessions
                .CountAsync(s => s.MentorshipId == mentorshipId && s.Status == SessionStatus.Completed);
        }

        public async Task<decimal> CalculateAverageSessionRatingAsync(int mentorshipId)
        {
            var ratings = await _context.SessionAttendances
                .Where(a => a.Session.MentorshipId == mentorshipId && a.Rating.HasValue)
                .Select(a => a.Rating!.Value)
                .ToListAsync();

            return ratings.Any() ? (decimal)ratings.Average() : 0;
        }
    }
}