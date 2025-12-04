using Microsoft.EntityFrameworkCore;
using TalentBridge.API.Interfaces.IMentorship;
using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories.MentorshipRepositories
{
    public class MentorshipRequestRepository : IMentorshipRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public MentorshipRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MentorshipRequest?> GetByIdAsync(int id)
        {
            return await _context.MentorshipRequests
                .Include(r => r.Mentee)
                .Include(r => r.Mentor)
                .Include(r => r.ProposedMentorship)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<MentorshipRequest?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.MentorshipRequests
                .Include(r => r.Mentee)
                .Include(r => r.Mentor)
                .Include(r => r.ProposedMentorship)
                .Include(r => r.MentorApplications)
                    .ThenInclude(a => a.Mentor)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<MentorshipRequest>> GetAllAsync()
        {
            return await _context.MentorshipRequests
                .Include(r => r.Mentee)
                .Include(r => r.Mentor)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipRequest>> GetByMenteeIdAsync(int menteeId)
        {
            return await _context.MentorshipRequests
                .Where(r => r.MenteeId == menteeId)
                .Include(r => r.Mentor)
                .Include(r => r.MentorApplications)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipRequest>> GetByMentorIdAsync(int mentorId)
        {
            return await _context.MentorshipRequests
                .Where(r => r.MentorId == mentorId)
                .Include(r => r.Mentee)
                .Include(r => r.MentorApplications)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipRequest>> GetByStatusAsync(RequestStatus status)
        {
            return await _context.MentorshipRequests
                .Where(r => r.Status == status)
                .Include(r => r.Mentee)
                .Include(r => r.Mentor)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipRequest>> GetOpenRequestsAsync()
        {
            return await _context.MentorshipRequests
                .Where(r => r.Status == RequestStatus.Pending || r.Status == RequestStatus.Reviewing)
                .Include(r => r.Mentee)
                .Include(r => r.Mentor)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentorshipRequest>> GetByCategoryAsync(string category)
        {
            return await _context.MentorshipRequests
                .Where(r => r.Category == category &&
                           (r.Status == RequestStatus.Pending || r.Status == RequestStatus.Reviewing))
                .Include(r => r.Mentee)
                .Include(r => r.Mentor)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<MentorshipRequest> CreateAsync(MentorshipRequest request)
        {
            // Establecer fecha de expiración por defecto (30 días)
            if (!request.ExpiresAt.HasValue)
            {
                request.ExpiresAt = DateTime.UtcNow.AddDays(30);
            }

            _context.MentorshipRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<MentorshipRequest> UpdateAsync(MentorshipRequest request)
        {
            request.UpdatedAt = DateTime.UtcNow;
            _context.MentorshipRequests.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var request = await GetByIdAsync(id);
            if (request == null) return false;

            _context.MentorshipRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeStatusAsync(int id, RequestStatus status, string? rejectionReason = null)
        {
            var request = await GetByIdAsync(id);
            if (request == null) return false;

            request.Status = status;
            request.RejectionReason = rejectionReason;
            request.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(request);
            return true;
        }

        public async Task<bool> AssignMentorAsync(int id, int mentorId)
        {
            var request = await GetByIdAsync(id);
            if (request == null) return false;

            request.MentorId = mentorId;
            request.Status = RequestStatus.Matched;
            request.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(request);
            return true;
        }

        public async Task<bool> LinkToMentorshipAsync(int id, int mentorshipId)
        {
            var request = await GetByIdAsync(id);
            if (request == null) return false;

            request.ProposedMentorshipId = mentorshipId;
            request.Status = RequestStatus.Accepted;
            request.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(request);
            return true;
        }

        public async Task<int> CountApplicationsAsync(int requestId)
        {
            return await _context.MentorApplications
                .CountAsync(a => a.RequestId == requestId);
        }

        public async Task<bool> HasUserAppliedAsync(int requestId, int mentorId)
        {
            return await _context.MentorApplications
                .AnyAsync(a => a.RequestId == requestId && a.MentorId == mentorId);
        }

        public async Task<bool> ExpireRequestAsync(int id)
        {
            var request = await GetByIdAsync(id);
            if (request == null) return false;

            request.Status = RequestStatus.Expired;
            request.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(request);
            return true;
        }
    }
}