using TalentBridge.Domain.Enums;

namespace TalentBridge.Domain.Entities
{
    public class CommunityMember
    {
        public int Id { get; set; }

        // Relaciones
        public int CommunityId { get; set; }
        public Community Community { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Rol en la comunidad
        public CommunityRole Role { get; set; } = CommunityRole.Member;

        // Estado
        public bool IsActive { get; set; } = true;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LeftAt { get; set; }

        // Estadísticas del miembro
        public int TotalPosts { get; set; } = 0;
        public int TotalComments { get; set; } = 0;
        public int ReputationPoints { get; set; } = 0; // Para gamificación futura
    }
}