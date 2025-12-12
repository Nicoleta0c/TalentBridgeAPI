namespace TalentBridge.API.DTOs.CommunityDTOs
{
    public class CommunityDto

    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int UniversityId { get; set; }
        public string UniversityName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsPrivate { get; set; }
        public int TotalMembers { get; set; }
        public int TotalPosts { get; set; }
        public int MaxMembers { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
    }
}