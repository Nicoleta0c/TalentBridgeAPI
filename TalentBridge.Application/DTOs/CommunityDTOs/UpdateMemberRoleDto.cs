namespace TalentBridge.API.DTOs.CommunityDTOs
{
    public class UpdateMemberRoleDto
    {
        public int MemberId { get; set; }
        public string Role { get; set; } = string.Empty; // Member, Moderator, Admin
    }
}