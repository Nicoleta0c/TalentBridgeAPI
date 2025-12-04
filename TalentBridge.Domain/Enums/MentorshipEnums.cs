namespace TalentBridge.Domain.Enums
{
    public enum MentorshipStatus
    {
        Requested = 0,      // Solicitud enviada
        Pending = 1,        // Pendiente de aceptación
        Active = 2,         // Activa
        Paused = 3,         // En pausa
        Completed = 4,      // Completada
        Cancelled = 5,      // Cancelada
        Expired = 6         // Expirada
    }

    public enum SessionStatus
    {
        Scheduled = 0,      // Programada
        Confirmed = 1,      // Confirmada
        InProgress = 2,     // En progreso
        Completed = 3,      // Completada
        Cancelled = 4,      // Cancelada
        Rescheduled = 5     // Reprogramada
    }

    public enum AttendanceStatus
    {
        Invited = 0,        // Invitado
        Attending = 1,      // Asistirá
        Attended = 2,       // Asistió
        NoShow = 3,         // No se presentó
        Cancelled = 4       // Canceló asistencia
    }

    public enum MilestoneStatus
    {
        Pending = 0,        // Pendiente
        InProgress = 1,     // En progreso
        Completed = 2,      // Completado
        Delayed = 3,        // Retrasado
        Cancelled = 4       // Cancelado
    }

    public enum ResourceType
    {
        Document = 0,       // Documento (PDF, Word)
        Video = 1,          // Video
        Link = 2,           // Enlace externo
        Presentation = 3,   // Presentación
        Code = 4,           // Código/Repositorio
        Audio = 5,          // Audio/Podcast
        Image = 6           // Imagen/Diagrama
    }

    public enum MeetingMethod
    {
        Virtual = 0,        // Virtual (Zoom, Teams, etc.)
        InPerson = 1,       // Presencial
        Hybrid = 2,         // Híbrido
        Phone = 3           // Llamada telefónica
    }

    public enum RequestStatus
    {
        Pending = 0,        // Pendiente
        Reviewing = 1,      // En revisión
        Accepted = 2,       // Aceptada
        Rejected = 3,       // Rechazada
        Expired = 4,        // Expirada
        Matched = 5         // Emparejada con mentor
    }

    public enum ApplicationStatus
    {
        Pending = 0,        // Pendiente
        Reviewed = 1,       // Revisada
        Accepted = 2,       // Aceptada
        Rejected = 3,       // Rechazada
        Withdrawn = 4       // Retirada
    }
}