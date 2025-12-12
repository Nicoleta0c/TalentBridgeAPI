using TalentBridge.Domain.Entities;
namespace TalentBridge.API.Interfaces.ICommunity
{
    public interface ICommunityRepository
    {
        Task<Community?> GetByIdAsync(int id);
        Task<Community?> GetByIdForUpdateAsync(int id);

        Task<Community?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Community>> GetAllAsync();
        Task<IEnumerable<Community>> GetByUniversityIdAsync(int universityId);
        Task<IEnumerable<Community>> GetPublicCommunitiesAsync();
        Task<IEnumerable<Community>> GetUserCommunitiesAsync(int userId);
        Task<Community> CreateAsync(Community community);
        Task<Community> UpdateAsync(Community community);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetMemberCountAsync(int communityId);
        Task<int> GetPostCountAsync(int communityId);
        Task UpdateStatsAsync(int communityId);
    }
}