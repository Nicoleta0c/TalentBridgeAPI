using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;

namespace TalentBridge.API.Interfaces.ICommunity
{
    public interface ICommunityMemberRepository
    {
        Task<CommunityMember?> GetByIdAsync(int id);
        Task<CommunityMember?> GetMembershipAsync(int communityId, int userId);
        Task<IEnumerable<CommunityMember>> GetCommunityMembersAsync(int communityId);
        Task<IEnumerable<CommunityMember>> GetUserMembershipsAsync(int userId);
        Task<CommunityMember> AddMemberAsync(CommunityMember member);
        Task<CommunityMember> UpdateMemberAsync(CommunityMember member);
        Task<bool> RemoveMemberAsync(int id);
        Task<bool> IsMemberAsync(int communityId, int userId);
        Task<bool> IsModeratorOrAboveAsync(int communityId, int userId);
        Task<CommunityRole?> GetUserRoleAsync(int communityId, int userId);
    }
}