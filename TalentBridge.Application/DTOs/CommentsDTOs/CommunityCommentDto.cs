namespace TalentBridge.Application.DTOs.CommentsDTOs
{
    public class CommunityCommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int PostId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int? ParentCommentId { get; set; }
        public int LikesCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsEdited { get; set; }
        public bool IsLikedByCurrentUser { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CommunityCommentDto> Replies { get; set; } = new();
    }
}