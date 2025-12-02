namespace TalentBridge.Application.DTOs.UserDTOs
{
    public class CreateAdminDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}