using TalentBridge.Domain.Enums;

namespace TalentBridge.Domain.Entities
{
    public class MentorApplication
    {
        public int Id { get; set; }

        public int RequestId { get; set; }
        public MentorshipRequest Request { get; set; } = null!;

        public int MentorId { get; set; }
        public User Mentor { get; set; } = null!;

        // Propuesta del mentor
        public string Proposal { get; set; } = string.Empty;
        public string? Experience { get; set; }
        public string? WhyChooseMe { get; set; }

        // Términos propuestos
        public int? ProposedDurationWeeks { get; set; }
        public int? ProposedSessionsPerWeek { get; set; }
        public MeetingMethod? ProposedMeetingMethod { get; set; }

        // Estado
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}