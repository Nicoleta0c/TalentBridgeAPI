using TalentBridge.Domain.Enums;

namespace TalentBridge.Domain.Entities
{
    public class MentorshipRequest
    {
        public int Id { get; set; }

        public int MenteeId { get; set; }
        public User Mentee { get; set; } = null!;

        public int? MentorId { get; set; }
        public User? Mentor { get; set; }

        // Solicitud específica
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Goals { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();

        // Preferencias
        public int PreferredDurationWeeks { get; set; } = 4;
        public int PreferredSessionsPerWeek { get; set; } = 1;
        public MeetingMethod PreferredMeetingMethod { get; set; } = MeetingMethod.Virtual;

        // Estado
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public string? RejectionReason { get; set; }

        // Propuesta de mentoría (si la hay)
        public int? ProposedMentorshipId { get; set; }
        public Mentorship? ProposedMentorship { get; set; }

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; } // Fecha de expiración

        // Relaciones
        public ICollection<MentorApplication> MentorApplications { get; set; } = new List<MentorApplication>();
    }
}