namespace TalentBridge.API.DTOs.CommunityDTOs
{
    public class CommunityMemberDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime JoinedAt { get; set; }
        public int TotalPosts { get; set; }
        public int TotalComments { get; set; }
        public int ReputationPoints { get; set; }
    }
}