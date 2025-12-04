using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;

namespace TalentBridge.API.Interfaces.IMentorship
{
    public interface ISessionAttendanceRepository
    {
        Task<SessionAttendance?> GetByIdAsync(int id);
        Task<SessionAttendance?> GetBySessionAndUserAsync(int sessionId, int userId);
        Task<IEnumerable<SessionAttendance>> GetBySessionIdAsync(int sessionId);
        Task<IEnumerable<SessionAttendance>> GetByUserIdAsync(int userId);
        Task<IEnumerable<SessionAttendance>> GetByMentorshipIdAsync(int mentorshipId);

        Task<SessionAttendance> CreateAsync(SessionAttendance attendance);
        Task<SessionAttendance> UpdateAsync(SessionAttendance attendance);
        Task<bool> DeleteAsync(int id);

        // Operaciones específicas
        Task<bool> MarkAttendanceAsync(int sessionId, int userId, AttendanceStatus status);
        Task<bool> JoinSessionAsync(int sessionId, int userId);
        Task<bool> LeaveSessionAsync(int sessionId, int userId);
        Task<bool> AddFeedbackAsync(int sessionId, int userId, int rating, string feedback);
        Task<decimal> CalculateAttendanceRateAsync(int mentorshipId, int userId);
        Task<int> CalculateTotalAttendanceMinutesAsync(int mentorshipId, int userId);
    }
}