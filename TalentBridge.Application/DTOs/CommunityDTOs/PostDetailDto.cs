using TalentBridge.Application.DTOs.CommentsDTOs;

namespace TalentBridge.API.DTOs.CommunityDTOs
{
    public class PostDetailDto : CommunityPostDto
    {
        public List<CommunityCommentDto> Comments { get; set; } = new();
    }
}