using TalentBridge.Domain.Enums;

namespace TalentBridge.Domain.Entities
{
    public class MentorshipSession
    {
        public int Id { get; set; }

        // Relación con la mentoría
        public int MentorshipId { get; set; }
        public Mentorship Mentorship { get; set; } = null!;

        // Información de la sesión
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Estado de la sesión
        public SessionStatus Status { get; set; } = SessionStatus.Scheduled;
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

        // Detalles de la reunión
        public MeetingMethod MeetingMethod { get; set; }
        public string? MeetingLink { get; set; }
        public string? MeetingId { get; set; } // Para plataformas como Zoom, Teams

        // Contenido y seguimiento
        public string? Agenda { get; set; }
        public string? Notes { get; set; } // Notas de la sesión
        public string? Homework { get; set; } // Tareas asignadas
        public string? Resources { get; set; } // Recursos compartidos

        // Recordatorio
        public bool SendReminder { get; set; } = true;
        public int ReminderHoursBefore { get; set; } = 24;

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Relaciones
        public ICollection<SessionAttendance> Attendances { get; set; } = new List<SessionAttendance>();
    }
}