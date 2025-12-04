using TalentBridge.Domain.Enums;
using TalentBridge.Domain.Entities;

namespace TalentBridge.API.Interfaces.IMentorship
{
    public interface IMentorshipResourceRepository
    {
        Task<MentorshipResource?> GetByIdAsync(int id);
        Task<IEnumerable<MentorshipResource>> GetByMentorshipIdAsync(int mentorshipId);
        Task<IEnumerable<MentorshipResource>> GetBySessionIdAsync(int sessionId);
        Task<IEnumerable<MentorshipResource>> GetByUserIdAsync(int userId);
        Task<IEnumerable<MentorshipResource>> GetPublicResourcesAsync();
        Task<IEnumerable<MentorshipResource>> GetByTypeAsync(int mentorshipId, ResourceType type);

        Task<MentorshipResource> CreateAsync(MentorshipResource resource);
        Task<MentorshipResource> UpdateAsync(MentorshipResource resource);
        Task<bool> DeleteAsync(int id);

        // Operaciones específicas
        Task<bool> TogglePublicAsync(int id, bool isPublic);
        Task<int> CountResourcesAsync(int mentorshipId);
        Task<IEnumerable<string>> GetResourceCategoriesAsync(int mentorshipId);
    }
}