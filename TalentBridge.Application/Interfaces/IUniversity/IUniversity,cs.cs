public interface IUniversityRepository
{
    Task<University?> GetByIdAsync(int id);
    Task<University?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<University>> GetAllAsync();
    Task<IEnumerable<University>> GetActiveUniversitiesAsync();
    Task<University?> GetByAcronymAsync(string acronym);
    Task<University> CreateAsync(University university);
    Task<University> UpdateAsync(University university);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
    Task<int> GetStudentCountAsync(int universityId);
    Task<int> GetCommunityCountAsync(int universityId);
}

// IUniversityCareerRepository.cs
public interface IUniversityCareerRepository
{
    Task<UniversityCareer?> GetByIdAsync(int id);
    Task<IEnumerable<UniversityCareer>> GetByUniversityIdAsync(int universityId);
    Task<UniversityCareer> CreateAsync(UniversityCareer career);
    Task<UniversityCareer> UpdateAsync(UniversityCareer career);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

// IUniversityService.cs
public interface IUniversityService
{
    Task<UniversityDto?> GetByIdAsync(int id);
    Task<UniversityDetailDto?> GetDetailByIdAsync(int id);
    Task<IEnumerable<UniversityDto>> GetAllAsync();
    Task<IEnumerable<UniversityDto>> GetActiveUniversitiesAsync();
    Task<UniversityDto> CreateAsync(CreateUniversityDto dto);
    Task<UniversityDto> UpdateAsync(int id, UpdateUniversityDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> VerifyUniversityAsync(int id);
    Task<IEnumerable<UniversityCareerDto>> GetCareersByUniversityAsync(int universityId);
    Task<UniversityCareerDto> CreateCareerAsync(CreateCareerDto dto);
}