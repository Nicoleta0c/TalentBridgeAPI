namespace TalentBridge.Domain.Entities
{
    public class CommunityComment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;

        // Relaciones
        public int PostId { get; set; }
        public CommunityPost Post { get; set; } = null!;

        public int AuthorId { get; set; }
        public User Author { get; set; } = null!;

        // Comentario padre (para respuestas)
        public int? ParentCommentId { get; set; }
        public CommunityComment? ParentComment { get; set; }
        public ICollection<CommunityComment> Replies { get; set; } = new List<CommunityComment>();

        // Estadísticas
        public int LikesCount { get; set; } = 0;

        // Estado
        public bool IsActive { get; set; } = true;
        public bool IsEdited { get; set; } = false;

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Relaciones
        public ICollection<CommentLike> Likes { get; set; } = new List<CommentLike>();
    }
}