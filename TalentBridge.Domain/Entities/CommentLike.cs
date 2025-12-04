namespace TalentBridge.Domain.Entities
{
    public class CommentLike
    {
        public int Id { get; set; }

        public int CommentId { get; set; }
        public CommunityComment Comment { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}