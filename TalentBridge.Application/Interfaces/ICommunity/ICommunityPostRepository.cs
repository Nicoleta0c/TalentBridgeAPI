using TalentBridge.Domain.Entities;

namespace TalentBridge.API.Interfaces.ICommunity
{
    public interface ICommunityPostRepository
    {
        Task<CommunityPost?> GetByIdAsync(int id);
        Task<CommunityPost?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<CommunityPost>> GetByCommunityIdAsync(int communityId);
        Task<IEnumerable<CommunityPost>> GetByAuthorIdAsync(int authorId);
        Task<IEnumerable<CommunityPost>> GetPinnedPostsAsync(int communityId);
        Task<CommunityPost> CreateAsync(CommunityPost post);
        Task<CommunityPost> UpdateAsync(CommunityPost post);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task IncrementViewsAsync(int postId);
        Task UpdateLikesCountAsync(int postId);
        Task UpdateCommentsCountAsync(int postId);
    }
}