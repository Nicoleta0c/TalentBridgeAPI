using TalentBridge.Application.DTOs.CommentsDTOs;

namespace TalentBridge.API.Interfaces.ICommunity
{
    public interface ICommunityCommentService
    {
        Task<CommunityCommentDto?> GetByIdAsync(int id, int? currentUserId = null);
        Task<IEnumerable<CommunityCommentDto>> GetByPostIdAsync(int postId, int? currentUserId = null);
        Task<CommunityCommentDto> CreateAsync(CreateCommentDto dto, int userId);
        Task<CommunityCommentDto> UpdateAsync(int id, UpdateCommentDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<bool> LikeCommentAsync(int commentId, int userId);
        Task<bool> UnlikeCommentAsync(int commentId, int userId);
    }
}