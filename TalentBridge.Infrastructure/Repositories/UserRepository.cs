using Microsoft.EntityFrameworkCore;
using TalentBridge.Application.Interfaces;
using TalentBridge.Application.Interfaces.IUser;
using TalentBridge.Domain.Entities;
using TalentBridge.Infrastructure.Data;

namespace TalentBridge.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new User
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PasswordHash = u.PasswordHash,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    UniversityId = u.UniversityId,
                    CareerCareerId = u.CareerCareerId,
                    StudentId = u.StudentId,
                    ExpectedGraduationYear = u.ExpectedGraduationYear,
                    CurrentSemester = u.CurrentSemester,
                    IsMentor = u.IsMentor,
                    IsMentee = u.IsMentee,
                    MentorTitle = u.MentorTitle,
                    MentorBio = u.MentorBio,
                    YearsOfExperience = u.YearsOfExperience,
                    HourlyRate = u.HourlyRate,
                    AvailableForMentorship = u.AvailableForMentorship,
                    MaxMentees = u.MaxMentees,
                    MentorAverageRating = u.MentorAverageRating,
                    TotalMentorshipSessions = u.TotalMentorshipSessions,
                    TotalMenteesHelped = u.TotalMenteesHelped,
                    PreferredMeetingMethod = u.PreferredMeetingMethod,
                    Timezone = u.Timezone
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Where(u => u.Email == email)
                .Select(u => new User
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PasswordHash = u.PasswordHash,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    UniversityId = u.UniversityId,
                    CareerCareerId = u.CareerCareerId,
                    StudentId = u.StudentId,
                    ExpectedGraduationYear = u.ExpectedGraduationYear,
                    CurrentSemester = u.CurrentSemester,
                    IsMentor = u.IsMentor,
                    IsMentee = u.IsMentee,
                    MentorTitle = u.MentorTitle,
                    MentorBio = u.MentorBio,
                    YearsOfExperience = u.YearsOfExperience,
                    HourlyRate = u.HourlyRate,
                    AvailableForMentorship = u.AvailableForMentorship,
                    MaxMentees = u.MaxMentees,
                    MentorAverageRating = u.MentorAverageRating,
                    TotalMentorshipSessions = u.TotalMentorshipSessions,
                    TotalMenteesHelped = u.TotalMenteesHelped,
                    PreferredMeetingMethod = u.PreferredMeetingMethod,
                    Timezone = u.Timezone
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            // Usa Select() para traer SOLO las columnas seguras
            // Esto evita que EF intente deserializar propiedades JSON corruptas
            return await _context.Users
                .Where(u => u.IsActive)
                .Select(u => new User
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PasswordHash = u.PasswordHash,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    UniversityId = u.UniversityId,
                    CareerCareerId = u.CareerCareerId,
                    StudentId = u.StudentId,
                    ExpectedGraduationYear = u.ExpectedGraduationYear,
                    CurrentSemester = u.CurrentSemester,
                    IsMentor = u.IsMentor,
                    IsMentee = u.IsMentee,
                    MentorTitle = u.MentorTitle,
                    MentorBio = u.MentorBio,
                    YearsOfExperience = u.YearsOfExperience,
                    HourlyRate = u.HourlyRate,
                    AvailableForMentorship = u.AvailableForMentorship,
                    MaxMentees = u.MaxMentees,
                    MentorAverageRating = u.MentorAverageRating,
                    TotalMentorshipSessions = u.TotalMentorshipSessions,
                    TotalMenteesHelped = u.TotalMenteesHelped,
                    PreferredMeetingMethod = u.PreferredMeetingMethod,
                    Timezone = u.Timezone
                    // NO incluyas: MentorExpertise, MentorIndustries, PreferredMentorshipCategories, AvailabilitySchedule
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            user.IsActive = false;
            await UpdateAsync(user);
        }
    }
}