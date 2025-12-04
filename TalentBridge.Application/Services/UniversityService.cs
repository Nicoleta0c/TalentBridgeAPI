namespace TalentBridge.API.Services
{
    public class UniversityService : IUniversityService
    {
        private readonly IUniversityRepository _universityRepository;
        private readonly IUniversityCareerRepository _careerRepository;

        public UniversityService(
            IUniversityRepository universityRepository,
            IUniversityCareerRepository careerRepository)
        {
            _universityRepository = universityRepository;
            _careerRepository = careerRepository;
        }

        public async Task<UniversityDto?> GetByIdAsync(int id)
        {
            var university = await _universityRepository.GetByIdAsync(id);
            if (university == null) return null;

            return await MapToUniversityDto(university);
        }

        public async Task<UniversityDetailDto?> GetDetailByIdAsync(int id)
        {
            var university = await _universityRepository.GetByIdWithDetailsAsync(id);
            if (university == null) return null;

            var dto = new UniversityDetailDto
            {
                Id = university.Id,
                Name = university.Name,
                Acronym = university.Acronym,
                Description = university.Description,
                Logo = university.Logo,
                Website = university.Website,
                Email = university.Email,
                Phone = university.Phone,
                Address = university.Address,
                City = university.City,
                Province = university.Province,
                ContactPersonName = university.ContactPersonName,
                ContactPersonEmail = university.ContactPersonEmail,
                ContactPersonPhone = university.ContactPersonPhone,
                IsActive = university.IsActive,
                IsVerified = university.IsVerified,
                TotalStudents = university.Students.Count,
                TotalCommunities = university.Communities.Count,
                CreatedAt = university.CreatedAt,
                Careers = university.Careers.Select(c => new UniversityCareerDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    Description = c.Description,
                    AreaOfStudy = c.AreaOfStudy,
                    DurationYears = c.DurationYears,
                    DegreeType = c.DegreeType,
                    TotalStudents = c.Students.Count,
                    IsActive = c.IsActive
                }).ToList()
            };

            return dto;
        }

        public async Task<IEnumerable<UniversityDto>> GetAllAsync()
        {
            var universities = await _universityRepository.GetAllAsync();
            var dtos = new List<UniversityDto>();

            foreach (var university in universities)
            {
                dtos.Add(await MapToUniversityDto(university));
            }

            return dtos;
        }

        public async Task<IEnumerable<UniversityDto>> GetActiveUniversitiesAsync()
        {
            var universities = await _universityRepository.GetActiveUniversitiesAsync();
            var dtos = new List<UniversityDto>();

            foreach (var university in universities)
            {
                dtos.Add(await MapToUniversityDto(university));
            }

            return dtos;
        }

        public async Task<UniversityDto> CreateAsync(CreateUniversityDto dto)
        {
            // Verificar si ya existe una universidad con el mismo nombre
            if (await _universityRepository.ExistsByNameAsync(dto.Name))
            {
                throw new InvalidOperationException($"Ya existe una universidad con el nombre '{dto.Name}'");
            }

            var university = new University
            {
                Name = dto.Name,
                Acronym = dto.Acronym,
                Description = dto.Description,
                Logo = dto.Logo,
                Website = dto.Website,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                Province = dto.Province,
                ContactPersonName = dto.ContactPersonName,
                ContactPersonEmail = dto.ContactPersonEmail,
                ContactPersonPhone = dto.ContactPersonPhone,
                IsActive = true,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _universityRepository.CreateAsync(university);
            return await MapToUniversityDto(created);
        }

        public async Task<UniversityDto> UpdateAsync(int id, UpdateUniversityDto dto)
        {
            var university = await _universityRepository.GetByIdAsync(id);
            if (university == null)
            {
                throw new KeyNotFoundException($"Universidad con ID {id} no encontrada");
            }

            // Actualizar solo los campos que no son nulos
            if (!string.IsNullOrEmpty(dto.Name))
                university.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Acronym))
                university.Acronym = dto.Acronym;

            if (!string.IsNullOrEmpty(dto.Description))
                university.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.Logo))
                university.Logo = dto.Logo;

            if (!string.IsNullOrEmpty(dto.Website))
                university.Website = dto.Website;

            if (!string.IsNullOrEmpty(dto.Email))
                university.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.Phone))
                university.Phone = dto.Phone;

            if (!string.IsNullOrEmpty(dto.Address))
                university.Address = dto.Address;

            if (!string.IsNullOrEmpty(dto.City))
                university.City = dto.City;

            if (!string.IsNullOrEmpty(dto.Province))
                university.Province = dto.Province;

            if (!string.IsNullOrEmpty(dto.ContactPersonName))
                university.ContactPersonName = dto.ContactPersonName;

            if (!string.IsNullOrEmpty(dto.ContactPersonEmail))
                university.ContactPersonEmail = dto.ContactPersonEmail;

            if (!string.IsNullOrEmpty(dto.ContactPersonPhone))
                university.ContactPersonPhone = dto.ContactPersonPhone;

            var updated = await _universityRepository.UpdateAsync(university);
            return await MapToUniversityDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (!await _universityRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Universidad con ID {id} no encontrada");
            }

            return await _universityRepository.DeleteAsync(id);
        }

        public async Task<bool> VerifyUniversityAsync(int id)
        {
            var university = await _universityRepository.GetByIdAsync(id);
            if (university == null)
            {
                throw new KeyNotFoundException($"Universidad con ID {id} no encontrada");
            }

            university.IsVerified = true;
            university.UpdatedAt = DateTime.UtcNow;

            await _universityRepository.UpdateAsync(university);
            return true;
        }

        public async Task<IEnumerable<UniversityCareerDto>> GetCareersByUniversityAsync(int universityId)
        {
            if (!await _universityRepository.ExistsAsync(universityId))
            {
                throw new KeyNotFoundException($"Universidad con ID {universityId} no encontrada");
            }

            var careers = await _careerRepository.GetByUniversityIdAsync(universityId);

            return careers.Select(c => new UniversityCareerDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Description = c.Description,
                AreaOfStudy = c.AreaOfStudy,
                DurationYears = c.DurationYears,
                DegreeType = c.DegreeType,
                TotalStudents = c.Students?.Count ?? 0,
                IsActive = c.IsActive
            });
        }

        public async Task<UniversityCareerDto> CreateCareerAsync(CreateCareerDto dto)
        {
            if (!await _universityRepository.ExistsAsync(dto.UniversityId))
            {
                throw new KeyNotFoundException($"Universidad con ID {dto.UniversityId} no encontrada");
            }

            var career = new UniversityCareer
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                AreaOfStudy = dto.AreaOfStudy,
                DurationYears = dto.DurationYears,
                DegreeType = dto.DegreeType,
                UniversityId = dto.UniversityId,
                IsActive = true
            };

            var created = await _careerRepository.CreateAsync(career);

            return new UniversityCareerDto
            {
                Id = created.Id,
                Name = created.Name,
                Code = created.Code,
                Description = created.Description,
                AreaOfStudy = created.AreaOfStudy,
                DurationYears = created.DurationYears,
                DegreeType = created.DegreeType,
                TotalStudents = 0,
                IsActive = created.IsActive
            };
        }

        // Método auxiliar para mapear
        private async Task<UniversityDto> MapToUniversityDto(University university)
        {
            return new UniversityDto
            {
                Id = university.Id,
                Name = university.Name,
                Acronym = university.Acronym,
                Description = university.Description,
                Logo = university.Logo,
                Website = university.Website,
                Email = university.Email,
                Phone = university.Phone,
                Address = university.Address,
                City = university.City,
                Province = university.Province,
                IsActive = university.IsActive,
                IsVerified = university.IsVerified,
                TotalStudents = await _universityRepository.GetStudentCountAsync(university.Id),
                TotalCommunities = await _universityRepository.GetCommunityCountAsync(university.Id),
                CreatedAt = university.CreatedAt
            };
        }
    }
}