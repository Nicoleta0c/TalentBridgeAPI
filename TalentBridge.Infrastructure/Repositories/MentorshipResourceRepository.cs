using Microsoft.EntityFrameworkCore;
using TalentBridge.API.Interfaces.IMentorship;
using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories.MentorshipRepositories
{
    public class MentorshipResourceRepository : IMentorshipResourceRepository
    {
        private readonly ApplicationDbContext _context;

        public MentorshipResourceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MentorshipResource?> GetByIdAsync(int id)
        {
            return await _context.MentorshipResources
                .Include(r => r.Mentorship)
                .Include(r => r.Session)
                .Include(r => r.CreatedBy)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<MentorshipResource>> GetByMentorshipIdAsync(int mentorshipId)
        {
            return await _context.MentorshipResources
                .Where(r => r.MentorshipId == mentorshipId)
                .Include(r => r.CreatedBy)
                .Include(r => r.Session)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipResource>> GetBySessionIdAsync(int sessionId)
        {
            return await _context.MentorshipResources
                .Where(r => r.SessionId == sessionId)
                .Include(r => r.CreatedBy)
                .Include(r => r.Mentorship)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipResource>> GetByUserIdAsync(int userId)
        {
            return await _context.MentorshipResources
                .Where(r => r.CreatedByUserId == userId ||
                           r.Mentorship.MentorId == userId ||
                           r.Mentorship.MenteeId == userId)
                .Include(r => r.CreatedBy)
                .Include(r => r.Mentorship)
                .Include(r => r.Session)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipResource>> GetPublicResourcesAsync()
        {
            return await _context.MentorshipResources
                .Where(r => r.IsPublic)
                .Include(r => r.CreatedBy)
                .Include(r => r.Mentorship)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipResource>> GetByTypeAsync(int mentorshipId, ResourceType type)
        {
            return await _context.MentorshipResources
                .Where(r => r.MentorshipId == mentorshipId && r.Type == type)
                .Include(r => r.CreatedBy)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<MentorshipResource> CreateAsync(MentorshipResource resource)
        {
            _context.MentorshipResources.Add(resource);
            await _context.SaveChangesAsync();
            return resource;
        }

        public async Task<MentorshipResource> UpdateAsync(MentorshipResource resource)
        {
            resource.UpdatedAt = DateTime.UtcNow;
            _context.MentorshipResources.Update(resource);
            await _context.SaveChangesAsync();
            return resource;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var resource = await GetByIdAsync(id);
            if (resource == null) return false;

            _context.MentorshipResources.Remove(resource);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TogglePublicAsync(int id, bool isPublic)
        {
            var resource = await GetByIdAsync(id);
            if (resource == null) return false;

            resource.IsPublic = isPublic;
            resource.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(resource);
            return true;
        }

        public async Task<int> CountResourcesAsync(int mentorshipId)
        {
            return await _context.MentorshipResources
                .CountAsync(r => r.MentorshipId == mentorshipId);
        }

        public async Task<IEnumerable<string>> GetResourceCategoriesAsync(int mentorshipId)
        {
            var resources = await _context.MentorshipResources
                .Where(r => r.MentorshipId == mentorshipId)
                .Select(r => r.Type.ToString())
                .Distinct()
                .ToListAsync();

            return resources;
        }
    }
}
