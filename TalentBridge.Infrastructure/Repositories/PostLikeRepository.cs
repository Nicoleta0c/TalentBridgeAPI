using Microsoft.EntityFrameworkCore;
using TalentBridge.API.Interfaces.ICommunity;
using TalentBridge.Domain.Entities;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories
{
    public class PostLikeRepository : IPostLikeRepository
    {
        private readonly ApplicationDbContext _context;

        public PostLikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PostLike?> GetLikeAsync(int postId, int userId)
        {
            return await _context.PostLikes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
        }

        public async Task<PostLike> AddLikeAsync(PostLike like)
        {
            _context.PostLikes.Add(like);
            await _context.SaveChangesAsync();
            return like;
        }

        public async Task<bool> RemoveLikeAsync(int postId, int userId)
        {
            var like = await GetLikeAsync(postId, userId);
            if (like == null) return false;

            _context.PostLikes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasUserLikedAsync(int postId, int userId)
        {
            return await _context.PostLikes
                .AnyAsync(l => l.PostId == postId && l.UserId == userId);
        }

        public async Task<int> GetLikesCountAsync(int postId)
        {
            return await _context.PostLikes
                .CountAsync(l => l.PostId == postId);
        }
    }
}