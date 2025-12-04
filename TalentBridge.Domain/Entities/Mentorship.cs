using TalentBridge.Domain.Enums;

namespace TalentBridge.Domain.Entities
{
    public class Mentorship
    {
        public int Id { get; set; }

        // Mentor y Mentee
        public int MentorId { get; set; }
        public User Mentor { get; set; } = null!;

        public int MenteeId { get; set; }
        public User Mentee { get; set; } = null!;

        // Información de la mentoría
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Goals { get; set; } // Metas del mentee
        public string? Expectations { get; set; } // Expectativas del mentor

        // Área de especialización
        public string Category { get; set; } = string.Empty; // Ej: "Desarrollo de Software", "Diseño UX/UI"
        public List<string> Tags { get; set; } = new List<string>(); // Ej: ["C#", ".NET", "React"]

        // Configuración de la mentoría
        public MentorshipStatus Status { get; set; } = MentorshipStatus.Requested;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int DurationWeeks { get; set; } = 4; // Duración en semanas
        public int SessionsPerWeek { get; set; } = 1; // Sesiones por semana
        public int SessionDurationMinutes { get; set; } = 60; // Duración por sesión

        // Método de reunión
        public MeetingMethod MeetingMethod { get; set; } = MeetingMethod.Virtual;
        public string? MeetingLink { get; set; } // Para reuniones virtuales

        // Calificaciones
        public decimal? MentorRating { get; set; } // Calificación del mentor (1-5)
        public decimal? MenteeRating { get; set; } // Calificación del mentee
        public string? MentorFeedback { get; set; } // Feedback del mentor
        public string? MenteeFeedback { get; set; } // Feedback del mentee

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        // Relaciones
        public ICollection<MentorshipSession> Sessions { get; set; } = new List<MentorshipSession>();
        public ICollection<MentorshipMilestone> Milestones { get; set; } = new List<MentorshipMilestone>();
        public ICollection<MentorshipResource> Resources { get; set; } = new List<MentorshipResource>();
    }
}