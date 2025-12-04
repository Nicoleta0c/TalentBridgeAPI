using TalentBridge.Domain.Enums;

namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class UpdateAttendanceDto
    {
        public AttendanceStatus Status { get; set; }
        public int? Rating { get; set; }
        public string? Feedback { get; set; }
    }
}

// AttendanceDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class AttendanceDto
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public AttendanceStatus Status { get; set; }
        public DateTime? JoinedAt { get; set; }
        public int AttendanceMinutes { get; set; }
        public int? Rating { get; set; }
        public string? Feedback { get; set; }
    }
}