using Microsoft.EntityFrameworkCore;
using TalentBridge.Infrastructure.Data;

public class UniversityRepository : IUniversityRepository
{
    private readonly ApplicationDbContext _context;

    public UniversityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<University?> GetByIdAsync(int id)
    {
        return await _context.Universities
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<University?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Universities
            .Include(u => u.Careers)
            .Include(u => u.Communities)
            .Include(u => u.Students)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<University>> GetAllAsync()
    {
        return await _context.Universities
            .Include(u => u.Students)
            .Include(u => u.Communities)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<University>> GetActiveUniversitiesAsync()
    {
        return await _context.Universities
            .Where(u => u.IsActive)
            .Include(u => u.Students)
            .Include(u => u.Communities)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<University?> GetByAcronymAsync(string acronym)
    {
        return await _context.Universities
            .FirstOrDefaultAsync(u => u.Acronym.ToLower() == acronym.ToLower());
    }

    public async Task<University> CreateAsync(University university)
    {
        _context.Universities.Add(university);
        await _context.SaveChangesAsync();
        return university;
    }

    public async Task<University> UpdateAsync(University university)
    {
        university.UpdatedAt = DateTime.UtcNow;
        _context.Universities.Update(university);
        await _context.SaveChangesAsync();
        return university;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var university = await GetByIdAsync(id);
        if (university == null) return false;

        _context.Universities.Remove(university);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Universities.AnyAsync(u => u.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Universities
            .AnyAsync(u => u.Name.ToLower() == name.ToLower());
    }

    public async Task<int> GetStudentCountAsync(int universityId)
    {
        return await _context.Users
            .CountAsync(u => u.UniversityId == universityId);
    }

    public async Task<int> GetCommunityCountAsync(int universityId)
    {
        return await _context.Communities
            .CountAsync(c => c.UniversityId == universityId);
    }
}