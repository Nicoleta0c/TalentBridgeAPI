using Microsoft.EntityFrameworkCore;
using TalentBridge.API.Interfaces.ICommunity;
using TalentBridge.Domain.Entities;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories
{
    public class CommunityPostRepository : ICommunityPostRepository
    {
        private readonly ApplicationDbContext _context;

        public CommunityPostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommunityPost?> GetByIdAsync(int id)
        {
            return await _context.CommunityPosts
                .Include(p => p.Author)
                .Include(p => p.Community)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<CommunityPost?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.CommunityPosts
                .Include(p => p.Author)
                .Include(p => p.Community)
                .Include(p => p.Comments.Where(c => c.IsActive && c.ParentCommentId == null))
                    .ThenInclude(c => c.Author)
                .Include(p => p.Comments.Where(c => c.IsActive && c.ParentCommentId == null))
                    .ThenInclude(c => c.Replies.Where(r => r.IsActive))
                        .ThenInclude(r => r.Author)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<CommunityPost>> GetByCommunityIdAsync(int communityId)
        {
            return await _context.CommunityPosts
                .Where(p => p.CommunityId == communityId && p.IsActive)
                .Include(p => p.Author)
                .Include(p => p.Community)
                .OrderByDescending(p => p.IsPinned)
                .ThenByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CommunityPost>> GetByAuthorIdAsync(int authorId)
        {
            return await _context.CommunityPosts
                .Where(p => p.AuthorId == authorId && p.IsActive)
                .Include(p => p.Community)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CommunityPost>> GetPinnedPostsAsync(int communityId)
        {
            return await _context.CommunityPosts
                .Where(p => p.CommunityId == communityId && p.IsPinned && p.IsActive)
                .Include(p => p.Author)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<CommunityPost> CreateAsync(CommunityPost post)
        {
            _context.CommunityPosts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<CommunityPost> UpdateAsync(CommunityPost post)
        {
            post.UpdatedAt = DateTime.UtcNow;
            _context.CommunityPosts.Update(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var post = await GetByIdAsync(id);
            if (post == null) return false;

            post.IsActive = false;
            post.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(post);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.CommunityPosts.AnyAsync(p => p.Id == id);
        }

        public async Task IncrementViewsAsync(int postId)
        {
            var post = await GetByIdAsync(postId);
            if (post == null) return;

            post.ViewsCount++;
            await UpdateAsync(post);
        }

        public async Task UpdateLikesCountAsync(int postId)
        {
            var post = await GetByIdAsync(postId);
            if (post == null) return;

            post.LikesCount = await _context.PostLikes.CountAsync(l => l.PostId == postId);
            await UpdateAsync(post);
        }

        public async Task UpdateCommentsCountAsync(int postId)
        {
            var post = await GetByIdAsync(postId);
            if (post == null) return;

            post.CommentsCount = await _context.CommunityComments
                .CountAsync(c => c.PostId == postId && c.IsActive);
            await UpdateAsync(post);
        }
    }
}