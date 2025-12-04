using TalentBridge.Domain.Entities;

namespace TalentBridge.API.Interfaces.ICommunity
{
    public interface ICommunityCommentRepository
    {
        Task<CommunityComment?> GetByIdAsync(int id);
        Task<CommunityComment?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<CommunityComment>> GetByPostIdAsync(int postId);
        Task<IEnumerable<CommunityComment>> GetByAuthorIdAsync(int authorId);
        Task<IEnumerable<CommunityComment>> GetRepliesAsync(int parentCommentId);
        Task<CommunityComment> CreateAsync(CommunityComment comment);
        Task<CommunityComment> UpdateAsync(CommunityComment comment);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task UpdateLikesCountAsync(int commentId);
    }
}