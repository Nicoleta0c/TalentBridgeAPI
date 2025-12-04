using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;

namespace TalentBridge.API.Interfaces.IMentorship
{
    public interface IMentorshipMilestoneRepository
    {
        Task<MentorshipMilestone?> GetByIdAsync(int id);
        Task<IEnumerable<MentorshipMilestone>> GetByMentorshipIdAsync(int mentorshipId);
        Task<IEnumerable<MentorshipMilestone>> GetByStatusAsync(int mentorshipId, MilestoneStatus status);
        Task<IEnumerable<MentorshipMilestone>> GetUpcomingMilestonesAsync(int mentorshipId);
        Task<IEnumerable<MentorshipMilestone>> GetOverdueMilestonesAsync(int mentorshipId);

        Task<MentorshipMilestone> CreateAsync(MentorshipMilestone milestone);
        Task<MentorshipMilestone> UpdateAsync(MentorshipMilestone milestone);
        Task<bool> DeleteAsync(int id);

        // Operaciones específicas
        Task<bool> MarkCompleteAsync(int id, DateTime completedDate, string? evidence);
        Task<bool> UpdateProgressAsync(int id, int progressPercentage, string? notes);
        Task<int> CountMilestonesAsync(int mentorshipId);
        Task<int> CountCompletedMilestonesAsync(int mentorshipId);
        Task<decimal> CalculateProgressPercentageAsync(int mentorshipId);
    }
}