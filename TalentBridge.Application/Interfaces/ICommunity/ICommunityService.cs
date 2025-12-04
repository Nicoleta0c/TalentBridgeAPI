using TalentBridge.API.DTOs.CommunityDTOs;

namespace TalentBridge.API.Interfaces.ICommunity
{
    public interface ICommunityService
    {
        Task<CommunityDto?> GetByIdAsync(int id, int? currentUserId = null);
        Task<CommunityDetailDto?> GetDetailByIdAsync(int id, int? currentUserId = null);
        Task<IEnumerable<CommunityDto>> GetAllAsync();
        Task<IEnumerable<CommunityDto>> GetByUniversityIdAsync(int universityId);
        Task<IEnumerable<CommunityDto>> GetPublicCommunitiesAsync();
        Task<IEnumerable<CommunityDto>> GetUserCommunitiesAsync(int userId);
        Task<CommunityDto> CreateAsync(CreateCommunityDto dto, int userId);
        Task<CommunityDto> UpdateAsync(int id, UpdateCommunityDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<bool> JoinCommunityAsync(int communityId, int userId);
        Task<bool> LeaveCommunityAsync(int communityId, int userId);
        Task<IEnumerable<CommunityMemberDto>> GetMembersAsync(int communityId);
        Task<bool> UpdateMemberRoleAsync(int communityId, int memberId, string role, int currentUserId);
        Task<bool> RemoveMemberAsync(int communityId, int memberId, int currentUserId);
    }
}