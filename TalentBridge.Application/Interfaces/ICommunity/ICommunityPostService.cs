using TalentBridge.API.DTOs.CommunityDTOs;

namespace TalentBridge.API.Interfaces.ICommunity
{
    public interface ICommunityPostService
    {
        Task<CommunityPostDto?> GetByIdAsync(int id, int? currentUserId = null);
        Task<PostDetailDto?> GetDetailByIdAsync(int id, int? currentUserId = null);
        Task<IEnumerable<CommunityPostDto>> GetByCommunityIdAsync(int communityId, int? currentUserId = null);
        Task<IEnumerable<CommunityPostDto>> GetByAuthorIdAsync(int authorId, int? currentUserId = null);
        Task<CommunityPostDto> CreateAsync(CreatePostDto dto, int userId);
        Task<CommunityPostDto> UpdateAsync(int id, UpdatePostDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<bool> TogglePinAsync(int id, int userId);
        Task<bool> ToggleLockAsync(int id, int userId);
        Task<bool> LikePostAsync(int postId, int userId);
        Task<bool> UnlikePostAsync(int postId, int userId);
    }
}