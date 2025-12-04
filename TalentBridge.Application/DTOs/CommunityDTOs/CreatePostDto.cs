namespace TalentBridge.API.DTOs.CommunityDTOs
{
    public class CreatePostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int CommunityId { get; set; }
        public string? ImageUrl { get; set; }
        public string? AttachmentUrl { get; set; }
        public string Type { get; set; } = "Discussion";
    }
}