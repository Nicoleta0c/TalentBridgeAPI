using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;

namespace TalentBridge.API.Interfaces.IMentorship
{
    public interface IMentorApplicationRepository
    {
        Task<MentorApplication?> GetByIdAsync(int id);
        Task<IEnumerable<MentorApplication>> GetByRequestIdAsync(int requestId);
        Task<IEnumerable<MentorApplication>> GetByMentorIdAsync(int mentorId);
        Task<IEnumerable<MentorApplication>> GetByStatusAsync(ApplicationStatus status);
        Task<MentorApplication?> GetUserApplicationAsync(int requestId, int mentorId);

        Task<MentorApplication> CreateAsync(MentorApplication application);
        Task<MentorApplication> UpdateAsync(MentorApplication application);
        Task<bool> DeleteAsync(int id);

        // Operaciones específicas
        Task<bool> ChangeStatusAsync(int id, ApplicationStatus status, string? rejectionReason = null);
        Task<bool> WithdrawApplicationAsync(int id);
        Task<int> CountByRequestIdAsync(int requestId);
        Task<int> CountByMentorIdAsync(int mentorId);
    }
}