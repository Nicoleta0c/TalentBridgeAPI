namespace TalentBridge.API.DTOs.CommunityDTOs
{
    public class UpdatePostDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public string? AttachmentUrl { get; set; }
    }
}