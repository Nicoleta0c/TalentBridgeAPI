using TalentBridge.Domain.Entities;

namespace TalentBridge.API.Interfaces.ICommunity
{
    public interface IPostLikeRepository
    {
        Task<PostLike?> GetLikeAsync(int postId, int userId);
        Task<PostLike> AddLikeAsync(PostLike like);
        Task<bool> RemoveLikeAsync(int postId, int userId);
        Task<bool> HasUserLikedAsync(int postId, int userId);
        Task<int> GetLikesCountAsync(int postId);
    }
}