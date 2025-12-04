using TalentBridge.Domain.Entities;

public class UniversityCareer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Ej: Desarrollo de Software
    public string Code { get; set; } = string.Empty; // Código interno
    public string Description { get; set; } = string.Empty;
    public string AreaOfStudy { get; set; } = string.Empty; // Ej: Tecnología, Ingeniería
    public int DurationYears { get; set; } // Duración en años
    public string DegreeType { get; set; } = string.Empty; // Técnico, Licenciatura, Ingeniería

    // Relación con universidad
    public int UniversityId { get; set; }
    public University University { get; set; } = null!;

    // Estado
    public bool IsActive { get; set; } = true;

    // Relaciones
    public ICollection<User> Students { get; set; } = new List<User>();
}