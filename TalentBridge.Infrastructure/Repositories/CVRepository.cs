using Microsoft.EntityFrameworkCore;
using TalentBridge.Application.Interfaces;
using TalentBridge.Domain.Entities;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories
{
    public class CVRepository : ICVRepository
    {
        private readonly ApplicationDbContext _context;

        public CVRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CV?> GetByIdAsync(int id)
        {
            return await _context.CVs
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CV>> GetByUserIdAsync(int userId)
        {
            return await _context.CVs
                .Include(c => c.User)
                .Where(c => c.UserId == userId && c.IsActive)
                .OrderByDescending(c => c.UploadDate)
                .ToListAsync();
        }

        public async Task<CV?> GetActiveCVByUserIdAsync(int userId)
        {
            return await _context.CVs
                .Include(c => c.User)
                .Where(c => c.UserId == userId && c.IsActive)
                .OrderByDescending(c => c.UploadDate)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(CV cv)
        {
            await _context.CVs.AddAsync(cv);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CV cv)
        {
            cv.UpdatedAt = DateTime.UtcNow;
            _context.CVs.Update(cv);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CV cv)
        {
            cv.IsActive = false;
            await UpdateAsync(cv);
        }
    }
}