namespace TalentBridge.API.DTOs.CommunityDTOs
{
    public class CreateCommunityDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int UniversityId { get; set; }
        public string? Purpose { get; set; }
        public string? Rules { get; set; }
        public int MaxMembers { get; set; } = 0;
        public bool IsPrivate { get; set; } = false;
    }
}