namespace TalentBridge.Application.DTOs.CommentsDTOs
{
    public class CreateCommentDto
    {
        public string Content { get; set; } = string.Empty;
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; }
    }
}