using Microsoft.EntityFrameworkCore;
using TalentBridge.API.Interfaces.ICommunity;
using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories
{
    public class CommunityMemberRepository : ICommunityMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public CommunityMemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommunityMember?> GetByIdAsync(int id)
        {
            return await _context.CommunityMembers
                .Include(m => m.User)
                .Include(m => m.Community)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<CommunityMember?> GetMembershipAsync(int communityId, int userId)
        {
            return await _context.CommunityMembers
                .Include(m => m.User)
                .Include(m => m.Community)
                .FirstOrDefaultAsync(m => m.CommunityId == communityId && m.UserId == userId);
        }

        public async Task<IEnumerable<CommunityMember>> GetCommunityMembersAsync(int communityId)
        {
            return await _context.CommunityMembers
                .Where(m => m.CommunityId == communityId && m.IsActive)
                .Include(m => m.User)
                .OrderByDescending(m => m.Role)
                .ThenByDescending(m => m.ReputationPoints)
                .ToListAsync();
        }

        public async Task<IEnumerable<CommunityMember>> GetUserMembershipsAsync(int userId)
        {
            return await _context.CommunityMembers
                .Where(m => m.UserId == userId && m.IsActive)
                .Include(m => m.Community)
                    .ThenInclude(c => c.University)
                .ToListAsync();
        }

        public async Task<CommunityMember> AddMemberAsync(CommunityMember member)
        {
            _context.CommunityMembers.Add(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<CommunityMember> UpdateMemberAsync(CommunityMember member)
        {
            _context.CommunityMembers.Update(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<bool> RemoveMemberAsync(int id)
        {
            var member = await GetByIdAsync(id);
            if (member == null) return false;

            member.IsActive = false;
            member.LeftAt = DateTime.UtcNow;
            await UpdateMemberAsync(member);
            return true;
        }

        public async Task<bool> IsMemberAsync(int communityId, int userId)
        {
            return await _context.CommunityMembers
                .AnyAsync(m => m.CommunityId == communityId && m.UserId == userId && m.IsActive);
        }

        public async Task<bool> IsModeratorOrAboveAsync(int communityId, int userId)
        {
            var member = await GetMembershipAsync(communityId, userId);
            if (member == null || !member.IsActive) return false;

            return member.Role >= CommunityRole.Moderator;
        }

        public async Task<CommunityRole?> GetUserRoleAsync(int communityId, int userId)
        {
            var member = await GetMembershipAsync(communityId, userId);
            return member?.IsActive == true ? member.Role : null;
        }
    }
}