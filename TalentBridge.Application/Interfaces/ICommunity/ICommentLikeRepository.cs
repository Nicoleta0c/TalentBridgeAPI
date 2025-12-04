using TalentBridge.Domain.Entities;

namespace TalentBridge.API.Interfaces.ICommunity
{
    public interface ICommentLikeRepository
    {
        Task<CommentLike?> GetLikeAsync(int commentId, int userId);
        Task<CommentLike> AddLikeAsync(CommentLike like);
        Task<bool> RemoveLikeAsync(int commentId, int userId);
        Task<bool> HasUserLikedAsync(int commentId, int userId);
        Task<int> GetLikesCountAsync(int commentId);
    }
}