using Microsoft.EntityFrameworkCore;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories
{
    public class UniversityCareerRepository : IUniversityCareerRepository
    {
        private readonly ApplicationDbContext _context;

        public UniversityCareerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UniversityCareer?> GetByIdAsync(int id)
        {
            return await _context.UniversityCareers
                .Include(c => c.University)
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<UniversityCareer>> GetByUniversityIdAsync(int universityId)
        {
            return await _context.UniversityCareers
                .Where(c => c.UniversityId == universityId)
                .Include(c => c.Students)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<UniversityCareer> CreateAsync(UniversityCareer career)
        {
            _context.UniversityCareers.Add(career);
            await _context.SaveChangesAsync();
            return career;
        }

        public async Task<UniversityCareer> UpdateAsync(UniversityCareer career)
        {
            _context.UniversityCareers.Update(career);
            await _context.SaveChangesAsync();
            return career;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var career = await GetByIdAsync(id);
            if (career == null) return false;

            _context.UniversityCareers.Remove(career);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.UniversityCareers.AnyAsync(c => c.Id == id);
        }
    }
}