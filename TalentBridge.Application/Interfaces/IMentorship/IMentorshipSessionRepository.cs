using TalentBridge.Domain.Entities;

namespace TalentBridge.API.Interfaces.IMentorship
{
    public interface IMentorshipSessionRepository
    {
        Task<MentorshipSession?> GetByIdAsync(int id);
        Task<MentorshipSession?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<MentorshipSession>> GetByMentorshipIdAsync(int mentorshipId);
        Task<IEnumerable<MentorshipSession>> GetByUserIdAsync(int userId);
        Task<IEnumerable<MentorshipSession>> GetUpcomingSessionsAsync(int mentorshipId);
        Task<IEnumerable<MentorshipSession>> GetPastSessionsAsync(int mentorshipId);
        Task<IEnumerable<MentorshipSession>> GetSessionsByDateRangeAsync(int mentorshipId, DateTime startDate, DateTime endDate);

        Task<MentorshipSession> CreateAsync(MentorshipSession session);
        Task<MentorshipSession> UpdateAsync(MentorshipSession session);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Operaciones específicas
        Task<bool> StartSessionAsync(int id);
        Task<bool> EndSessionAsync(int id);
        Task<bool> CancelSessionAsync(int id, string reason);
        Task<bool> RescheduleSessionAsync(int id, DateTime newDate, DateTime newStartTime);
        Task<bool> AddSessionNotesAsync(int id, string notes, string? homework);
        Task<int> CountCompletedSessionsAsync(int mentorshipId);
        Task<decimal> CalculateAverageSessionRatingAsync(int mentorshipId);
    }
}