using TalentBridge.Domain.Entities;

public class University
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Acronym { get; set; } = string.Empty; // Ej: ITLA, UASD
    public string Description { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty; // URL del logo
    public string Website { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    // Dirección
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;

    // Información adicional
    public string ContactPersonName { get; set; } = string.Empty;
    public string ContactPersonEmail { get; set; } = string.Empty;
    public string ContactPersonPhone { get; set; } = string.Empty;

    // Estado
    public bool IsActive { get; set; } = true;
    public bool IsVerified { get; set; } = false; // Verificada por admin

    // Auditoría
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Relaciones
    public ICollection<Community> Communities { get; set; } = new List<Community>();
    public ICollection<User> Students { get; set; } = new List<User>();
    public ICollection<UniversityCareer> Careers { get; set; } = new List<UniversityCareer>();
}