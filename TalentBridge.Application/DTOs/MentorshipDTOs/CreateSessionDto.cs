using TalentBridge.Domain.Enums;

namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class CreateSessionDto
    {
        public int MentorshipId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Agenda { get; set; }
        public MeetingMethod MeetingMethod { get; set; }
        public string? MeetingLink { get; set; }
        public bool SendReminder { get; set; } = true;
        public int ReminderHoursBefore { get; set; } = 24;
    }
}

// UpdateSessionDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class UpdateSessionDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Agenda { get; set; }
        public MeetingMethod? MeetingMethod { get; set; }
        public string? MeetingLink { get; set; }
        public SessionStatus? Status { get; set; }
        public string? Notes { get; set; }
        public string? Homework { get; set; }
    }
}

// MentorshipSessionDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class MentorshipSessionDto
    {
        public int Id { get; set; }
        public int MentorshipId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public SessionStatus Status { get; set; }
        public MeetingMethod MeetingMethod { get; set; }
        public string? MeetingLink { get; set; }
        public string? Notes { get; set; }
        public string? Homework { get; set; }
        public bool SendReminder { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<AttendanceDto> Attendances { get; set; } = new();
    }
}