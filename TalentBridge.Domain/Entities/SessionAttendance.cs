using TalentBridge.Domain.Enums;

namespace TalentBridge.Domain.Entities
{
    public class SessionAttendance
    {
        public int Id { get; set; }

        public int SessionId { get; set; }
        public MentorshipSession Session { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public AttendanceStatus Status { get; set; } = AttendanceStatus.Invited;
        public DateTime? JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public int AttendanceMinutes { get; set; } = 0;

        // Feedback de la sesión
        public int? Rating { get; set; } // 1-5
        public string? Feedback { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}