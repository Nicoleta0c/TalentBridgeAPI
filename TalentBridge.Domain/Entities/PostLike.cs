namespace TalentBridge.Domain.Entities
{
    public class PostLike
    {
        public int Id { get; set; }

        public int PostId { get; set; }
        public CommunityPost Post { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}