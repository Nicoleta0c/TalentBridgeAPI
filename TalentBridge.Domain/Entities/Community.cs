namespace TalentBridge.Domain.Entities
{
    public class Community
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty; // Logo o imagen de la comunidad

        // Relación con Universidad
        public int UniversityId { get; set; }
        public University University { get; set; } = null!;

        // Información adicional
        public string? Purpose { get; set; } // Propósito de la comunidad
        public string? Rules { get; set; } // Reglas de la comunidad
        public int MaxMembers { get; set; } = 0; // 0 = ilimitado

        // Estado
        public bool IsActive { get; set; } = true;
        public bool IsPrivate { get; set; } = false; // Pública o privada

        // Estadísticas
        public int TotalMembers { get; set; } = 0;
        public int TotalPosts { get; set; } = 0;

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;

        // Relaciones
        public ICollection<CommunityMember> Members { get; set; } = new List<CommunityMember>();
        public ICollection<CommunityPost> Posts { get; set; } = new List<CommunityPost>();
    }
}