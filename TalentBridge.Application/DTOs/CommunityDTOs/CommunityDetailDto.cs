namespace TalentBridge.API.DTOs.CommunityDTOs
{
    public class CommunityDetailDto : CommunityDto
    {
        public string? Purpose { get; set; }
        public string? Rules { get; set; }
        public int CreatedByUserId { get; set; }
        public bool IsUserMember { get; set; } = false;
        public string? UserRole { get; set; }
        public List<CommunityMemberDto> RecentMembers { get; set; } = new();
        public List<CommunityPostDto> RecentPosts { get; set; } = new();
    }
}