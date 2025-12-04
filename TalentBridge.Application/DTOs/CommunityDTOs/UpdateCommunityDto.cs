namespace TalentBridge.API.DTOs.CommunityDTOs
{
    public class UpdateCommunityDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Purpose { get; set; }
        public string? Rules { get; set; }
        public int? MaxMembers { get; set; }
        public bool? IsPrivate { get; set; }
    }
}