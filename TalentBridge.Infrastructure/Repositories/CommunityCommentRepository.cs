using Microsoft.EntityFrameworkCore;
using TalentBridge.API.Interfaces.ICommunity;
using TalentBridge.Domain.Entities;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories
{
    public class CommunityCommentRepository : ICommunityCommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommunityCommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommunityComment?> GetByIdAsync(int id)
        {
            return await _context.CommunityComments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CommunityComment?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.CommunityComments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .Include(c => c.Replies.Where(r => r.IsActive))
                    .ThenInclude(r => r.Author)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CommunityComment>> GetByPostIdAsync(int postId)
        {
            return await _context.CommunityComments
                .Where(c => c.PostId == postId && c.IsActive && c.ParentCommentId == null)
                .Include(c => c.Author)
                .Include(c => c.Replies.Where(r => r.IsActive))
                    .ThenInclude(r => r.Author)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CommunityComment>> GetByAuthorIdAsync(int authorId)
        {
            return await _context.CommunityComments
                .Where(c => c.AuthorId == authorId && c.IsActive)
                .Include(c => c.Post)
                    .ThenInclude(p => p.Community)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CommunityComment>> GetRepliesAsync(int parentCommentId)
        {
            return await _context.CommunityComments
                .Where(c => c.ParentCommentId == parentCommentId && c.IsActive)
                .Include(c => c.Author)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<CommunityComment> CreateAsync(CommunityComment comment)
        {
            _context.CommunityComments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<CommunityComment> UpdateAsync(CommunityComment comment)
        {
            comment.UpdatedAt = DateTime.UtcNow;
            comment.IsEdited = true;
            _context.CommunityComments.Update(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var comment = await GetByIdAsync(id);
            if (comment == null) return false;

            comment.IsActive = false;
            comment.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(comment);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.CommunityComments.AnyAsync(c => c.Id == id);
        }

        public async Task UpdateLikesCountAsync(int commentId)
        {
            var comment = await GetByIdAsync(commentId);
            if (comment == null) return;

            comment.LikesCount = await _context.CommentLikes.CountAsync(l => l.CommentId == commentId);
            await UpdateAsync(comment);
        }
    }
}