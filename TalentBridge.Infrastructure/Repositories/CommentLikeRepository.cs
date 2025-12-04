using Microsoft.EntityFrameworkCore;
using TalentBridge.API.Interfaces.ICommunity;
using TalentBridge.Domain.Entities;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories
{
    public class CommentLikeRepository : ICommentLikeRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentLikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommentLike?> GetLikeAsync(int commentId, int userId)
        {
            return await _context.CommentLikes
                .FirstOrDefaultAsync(l => l.CommentId == commentId && l.UserId == userId);
        }

        public async Task<CommentLike> AddLikeAsync(CommentLike like)
        {
            _context.CommentLikes.Add(like);
            await _context.SaveChangesAsync();
            return like;
        }

        public async Task<bool> RemoveLikeAsync(int commentId, int userId)
        {
            var like = await GetLikeAsync(commentId, userId);
            if (like == null) return false;

            _context.CommentLikes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasUserLikedAsync(int commentId, int userId)
        {
            return await _context.CommentLikes
                .AnyAsync(l => l.CommentId == commentId && l.UserId == userId);
        }

        public async Task<int> GetLikesCountAsync(int commentId)
        {
            return await _context.CommentLikes
                .CountAsync(l => l.CommentId == commentId);
        }
    }
}