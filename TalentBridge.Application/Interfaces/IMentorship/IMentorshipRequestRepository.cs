using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;

namespace TalentBridge.API.Interfaces.IMentorship
{
    public interface IMentorshipRequestRepository
    {
        Task<MentorshipRequest?> GetByIdAsync(int id);
        Task<MentorshipRequest?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<MentorshipRequest>> GetAllAsync();
        Task<IEnumerable<MentorshipRequest>> GetByMenteeIdAsync(int menteeId);
        Task<IEnumerable<MentorshipRequest>> GetByMentorIdAsync(int mentorId);
        Task<IEnumerable<MentorshipRequest>> GetByStatusAsync(RequestStatus status);
        Task<IEnumerable<MentorshipRequest>> GetOpenRequestsAsync(); // Pendientes y en revisión
        Task<IEnumerable<MentorshipRequest>> GetByCategoryAsync(string category);

        Task<MentorshipRequest> CreateAsync(MentorshipRequest request);
        Task<MentorshipRequest> UpdateAsync(MentorshipRequest request);
        Task<bool> DeleteAsync(int id);

        // Operaciones específicas
        Task<bool> ChangeStatusAsync(int id, RequestStatus status, string? rejectionReason = null);
        Task<bool> AssignMentorAsync(int id, int mentorId);
        Task<bool> LinkToMentorshipAsync(int id, int mentorshipId);
        Task<int> CountApplicationsAsync(int requestId);
        Task<bool> HasUserAppliedAsync(int requestId, int mentorId);
        Task<bool> ExpireRequestAsync(int id);
    }
}