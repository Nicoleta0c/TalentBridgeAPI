using Microsoft.EntityFrameworkCore;
using TalentBridge.API.Interfaces.ICommunity;
using TalentBridge.Domain.Entities;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories
{
    public class CommunityRepository : ICommunityRepository
    {
        private readonly ApplicationDbContext _context;

        public CommunityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene comunidad CON AsNoTracking (para READ)
        /// </summary>
        public async Task<Community?> GetByIdAsync(int id)
        {
            return await _context.Communities
                .Include(c => c.University)
                .Include(c => c.CreatedBy)
                .Where(c => c.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtiene comunidad SIN AsNoTracking (para UPDATE/DELETE)
        /// </summary>
        public async Task<Community?> GetByIdForUpdateAsync(int id)
        {
            return await _context.Communities
                .Include(c => c.University)
                .Include(c => c.CreatedBy)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Community?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Communities
                .Include(c => c.University)
                .Include(c => c.CreatedBy)
                .Include(c => c.Members)
                    .ThenInclude(m => m.User)
                .Include(c => c.Posts)
                    .ThenInclude(p => p.Author)
                .Where(c => c.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Community>> GetAllAsync()
        {
            return await _context.Communities
                .Include(c => c.University)
                .Include(c => c.CreatedBy)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Community>> GetByUniversityIdAsync(int universityId)
        {
            return await _context.Communities
                .Include(c => c.University)
                .Include(c => c.CreatedBy)
                .Where(c => c.UniversityId == universityId && c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Community>> GetPublicCommunitiesAsync()
        {
            return await _context.Communities
                .Include(c => c.University)
                .Include(c => c.CreatedBy)
                .Where(c => !c.IsPrivate && c.IsActive)
                .OrderByDescending(c => c.TotalMembers)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Community>> GetUserCommunitiesAsync(int userId)
        {
            return await _context.Communities
                .Include(c => c.University)
                .Include(c => c.CreatedBy)
                .Where(c => c.Members.Any(m => m.UserId == userId && m.IsActive))
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Community> CreateAsync(Community community)
        {
            _context.Communities.Add(community);
            await _context.SaveChangesAsync();
            return community;
        }

        public async Task<Community> UpdateAsync(Community community)
        {
            community.UpdatedAt = DateTime.UtcNow;
            _context.Communities.Update(community);
            await _context.SaveChangesAsync();
            return community;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // ✅ Usar GetByIdForUpdateAsync para DELETE
            var community = await GetByIdForUpdateAsync(id);
            if (community == null) return false;

            community.IsActive = false;
            community.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(community);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Communities
                .AsNoTracking()
                .AnyAsync(c => c.Id == id);
        }

        public async Task<int> GetMemberCountAsync(int communityId)
        {
            return await _context.CommunityMembers
                .AsNoTracking()
                .CountAsync(m => m.CommunityId == communityId && m.IsActive);
        }

        public async Task<int> GetPostCountAsync(int communityId)
        {
            return await _context.CommunityPosts
                .AsNoTracking()
                .CountAsync(p => p.CommunityId == communityId && p.IsActive);
        }

        public async Task UpdateStatsAsync(int communityId)
        {
            // Obtener WITH tracking (sin AsNoTracking) para poder actualizar
            var community = await _context.Communities
                .FirstOrDefaultAsync(c => c.Id == communityId);

            if (community == null) return;

            community.TotalMembers = await GetMemberCountAsync(communityId);
            community.TotalPosts = await GetPostCountAsync(communityId);
            community.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(community);
        }
    }
}