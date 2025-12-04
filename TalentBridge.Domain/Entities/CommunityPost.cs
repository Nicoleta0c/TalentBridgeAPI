using TalentBridge.Domain.Enums;

namespace TalentBridge.Domain.Entities
{
    public class CommunityPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? AttachmentUrl { get; set; }

        // Relaciones
        public int CommunityId { get; set; }
        public Community Community { get; set; } = null!;

        public int AuthorId { get; set; }
        public User Author { get; set; } = null!;

        // Tipo de post
        public PostType Type { get; set; } = PostType.Discussion;

        // Estadísticas
        public int ViewsCount { get; set; } = 0;
        public int LikesCount { get; set; } = 0;
        public int CommentsCount { get; set; } = 0;

        // Estado
        public bool IsActive { get; set; } = true;
        public bool IsPinned { get; set; } = false; // Post fijado
        public bool IsLocked { get; set; } = false; // No permite más comentarios

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Relaciones
        public ICollection<CommunityComment> Comments { get; set; } = new List<CommunityComment>();
        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    }
}