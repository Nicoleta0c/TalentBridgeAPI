using Microsoft.EntityFrameworkCore;
using TalentBridge.API.DTOs.MentorshipDTOs;
using TalentBridge.API.Interfaces.IMentorship;
using TalentBridge.Application.Interfaces.IUser;
using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;

namespace TalentBridge.API.Services.MentorshipServices
{
    public class MentorshipService : IMentorshipService
    {
        private readonly IMentorshipRepository _mentorshipRepository;
        private readonly IMentorshipSessionRepository _sessionRepository;
        private readonly ISessionAttendanceRepository _attendanceRepository;
        private readonly IMentorshipMilestoneRepository _milestoneRepository;
        private readonly IMentorshipResourceRepository _resourceRepository;
        private readonly IMentorshipRequestRepository _requestRepository;
        private readonly IMentorApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;

        public MentorshipService(
            IMentorshipRepository mentorshipRepository,
            IMentorshipSessionRepository sessionRepository,
            ISessionAttendanceRepository attendanceRepository,
            IMentorshipMilestoneRepository milestoneRepository,
            IMentorshipResourceRepository resourceRepository,
            IMentorshipRequestRepository requestRepository,
            IMentorApplicationRepository applicationRepository,
            IUserRepository userRepository)
        {
            _mentorshipRepository = mentorshipRepository;
            _sessionRepository = sessionRepository;
            _attendanceRepository = attendanceRepository;
            _milestoneRepository = milestoneRepository;
            _resourceRepository = resourceRepository;
            _requestRepository = requestRepository;
            _applicationRepository = applicationRepository;
            _userRepository = userRepository;
        }

        #region Mentorship CRUD
        public async Task<MentorshipDto?> GetMentorshipByIdAsync(int id, int currentUserId)
        {
            var mentorship = await _mentorshipRepository.GetByIdAsync(id);
            if (mentorship == null) return null;

            // Verificar permisos
            if (mentorship.MentorId != currentUserId && mentorship.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes acceso a esta mentoría");
            }

            return MapToMentorshipDto(mentorship);
        }

        public async Task<MentorshipDetailDto?> GetMentorshipDetailByIdAsync(int id, int currentUserId)
        {
            var mentorship = await _mentorshipRepository.GetByIdWithDetailsAsync(id);
            if (mentorship == null) return null;

            // Verificar permisos
            if (mentorship.MentorId != currentUserId && mentorship.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes acceso a esta mentoría");
            }

            var upcomingSessions = await _sessionRepository.GetUpcomingSessionsAsync(id);
            var recentMilestones = await _milestoneRepository.GetByMentorshipIdAsync(id);
            var recentResources = await _resourceRepository.GetByMentorshipIdAsync(id);

            var dto = new MentorshipDetailDto
            {
                Id = mentorship.Id,
                MentorId = mentorship.MentorId,
                MentorName = mentorship.Mentor.FullName,
                MentorEmail = mentorship.Mentor.Email,
                MenteeId = mentorship.MenteeId,
                MenteeName = mentorship.Mentee.FullName,
                MenteeEmail = mentorship.Mentee.Email,
                Title = mentorship.Title,
                Description = mentorship.Description,
                Category = mentorship.Category,
                Tags = mentorship.Tags,
                Status = mentorship.Status,
                StartDate = mentorship.StartDate,
                EndDate = mentorship.EndDate,
                DurationWeeks = mentorship.DurationWeeks,
                Goals = mentorship.Goals,
                Expectations = mentorship.Expectations,
                SessionsPerWeek = mentorship.SessionsPerWeek,
                SessionDurationMinutes = mentorship.SessionDurationMinutes,
                MeetingMethod = mentorship.MeetingMethod,
                MeetingLink = mentorship.MeetingLink,
                MentorFeedback = mentorship.MentorFeedback,
                MenteeFeedback = mentorship.MenteeFeedback,
                MentorRating = mentorship.MentorRating,
                MenteeRating = mentorship.MenteeRating,
                CreatedAt = mentorship.CreatedAt,
                TotalSessions = mentorship.Sessions?.Count ?? 0,
                CompletedSessions = mentorship.Sessions?.Count(s => s.Status == SessionStatus.Completed) ?? 0,
                TotalMilestones = mentorship.Milestones?.Count ?? 0,
                CompletedMilestones = mentorship.Milestones?.Count(m => m.Status == MilestoneStatus.Completed) ?? 0,
                UpcomingSessions = upcomingSessions.Select(MapToSessionDto).Take(5).ToList(),
                RecentMilestones = recentMilestones.Select(MapToMilestoneDto).Take(5).ToList(),
                RecentResources = recentResources.Select(MapToResourceDto).Take(5).ToList()
            };

            return dto;
        }

        public async Task<IEnumerable<MentorshipDto>> GetUserMentorshipsAsync(int userId)
        {
            var mentorships = await _mentorshipRepository.GetByUserIdAsync(userId);
            return mentorships.Select(MapToMentorshipDto);
        }

        public async Task<IEnumerable<MentorshipDto>> GetActiveMentorshipsAsync()
        {
            var mentorships = await _mentorshipRepository.GetActiveMentorshipsAsync();
            return mentorships.Select(MapToMentorshipDto);
        }

        public async Task<MentorshipDto> CreateMentorshipAsync(CreateMentorshipDto dto, int currentUserId)
        {
            // Verificar que el mentor existe
            var mentor = await _userRepository.GetByIdAsync(dto.MentorId);
            if (mentor == null || !mentor.IsMentor || !mentor.AvailableForMentorship)
            {
                throw new InvalidOperationException("El mentor seleccionado no está disponible");
            }

            // Verificar que el mentor no ha alcanzado su límite de mentees
            var activeMentorships = await _mentorshipRepository.CountActiveMentorshipsAsync(dto.MentorId);
            if (activeMentorships >= mentor.MaxMentees)
            {
                throw new InvalidOperationException("El mentor ha alcanzado su límite de mentees");
            }

            // Verificar que el mentee existe
            var mentee = await _userRepository.GetByIdAsync(dto.MenteeId);
            if (mentee == null)
            {
                throw new KeyNotFoundException($"Usuario con ID {dto.MenteeId} no encontrado");
            }

            // Solo el mentee puede solicitar mentoría
            if (currentUserId != dto.MenteeId)
            {
                throw new UnauthorizedAccessException("Solo puedes solicitar mentorías para ti mismo");
            }

            var mentorship = new Mentorship
            {
                MentorId = dto.MentorId,
                MenteeId = dto.MenteeId,
                Title = dto.Title,
                Description = dto.Description,
                Goals = dto.Goals,
                Expectations = dto.Expectations,
                Category = dto.Category,
                Tags = dto.Tags,
                Status = MentorshipStatus.Requested,
                DurationWeeks = dto.DurationWeeks,
                SessionsPerWeek = dto.SessionsPerWeek,
                SessionDurationMinutes = dto.SessionDurationMinutes,
                MeetingMethod = dto.MeetingMethod,
                MeetingLink = dto.MeetingLink,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _mentorshipRepository.CreateAsync(mentorship);
            return MapToMentorshipDto(created);
        }

        public async Task<MentorshipDto> UpdateMentorshipAsync(int id, UpdateMentorshipDto dto, int currentUserId)
        {
            var mentorship = await _mentorshipRepository.GetByIdAsync(id);
            if (mentorship == null)
            {
                throw new KeyNotFoundException($"Mentoría con ID {id} no encontrada");
            }

            // Verificar permisos: solo mentor o mentee pueden actualizar
            if (mentorship.MentorId != currentUserId && mentorship.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes permisos para actualizar esta mentoría");
            }

            // Solo se puede actualizar si está en estado Pending o Active
            if (mentorship.Status != MentorshipStatus.Pending && mentorship.Status != MentorshipStatus.Active)
            {
                throw new InvalidOperationException("No se puede actualizar una mentoría en este estado");
            }

            // Actualizar campos
            if (!string.IsNullOrEmpty(dto.Title))
                mentorship.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                mentorship.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.Goals))
                mentorship.Goals = dto.Goals;

            if (!string.IsNullOrEmpty(dto.Expectations))
                mentorship.Expectations = dto.Expectations;

            if (dto.DurationWeeks.HasValue)
                mentorship.DurationWeeks = dto.DurationWeeks.Value;

            if (dto.SessionsPerWeek.HasValue)
                mentorship.SessionsPerWeek = dto.SessionsPerWeek.Value;

            if (dto.SessionDurationMinutes.HasValue)
                mentorship.SessionDurationMinutes = dto.SessionDurationMinutes.Value;

            if (dto.MeetingMethod.HasValue)
                mentorship.MeetingMethod = dto.MeetingMethod.Value;

            if (dto.MeetingLink != null)
                mentorship.MeetingLink = dto.MeetingLink;

            if (dto.Status.HasValue)
                mentorship.Status = dto.Status.Value;

            var updated = await _mentorshipRepository.UpdateAsync(mentorship);
            return MapToMentorshipDto(updated);
        }

        public async Task<bool> DeleteMentorshipAsync(int id, int currentUserId)
        {
            var mentorship = await _mentorshipRepository.GetByIdAsync(id);
            if (mentorship == null)
            {
                throw new KeyNotFoundException($"Mentoría con ID {id} no encontrada");
            }

            // Solo el mentor o mentee pueden eliminar (en estado Requested o Pending)
            if (mentorship.MentorId != currentUserId && mentorship.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes permisos para eliminar esta mentoría");
            }

            if (mentorship.Status != MentorshipStatus.Requested && mentorship.Status != MentorshipStatus.Pending)
            {
                throw new InvalidOperationException("Solo se pueden eliminar mentorías en estado Pendiente");
            }

            return await _mentorshipRepository.DeleteAsync(id);
        }

        public async Task<bool> CompleteMentorshipAsync(int id, int currentUserId)
        {
            var mentorship = await _mentorshipRepository.GetByIdAsync(id);
            if (mentorship == null)
            {
                throw new KeyNotFoundException($"Mentoría con ID {id} no encontrada");
            }

            // Solo el mentor puede marcar como completada
            if (mentorship.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el mentor puede completar la mentoría");
            }

            if (mentorship.Status != MentorshipStatus.Active)
            {
                throw new InvalidOperationException("Solo se pueden completar mentorías activas");
            }

            return await _mentorshipRepository.CompleteMentorshipAsync(id);
        }

        public async Task<bool> CancelMentorshipAsync(int id, int currentUserId, string reason)
        {
            var mentorship = await _mentorshipRepository.GetByIdAsync(id);
            if (mentorship == null)
            {
                throw new KeyNotFoundException($"Mentoría con ID {id} no encontrada");
            }

            // Solo mentor o mentee pueden cancelar
            if (mentorship.MentorId != currentUserId && mentorship.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes permisos para cancelar esta mentoría");
            }

            return await _mentorshipRepository.CancelMentorshipAsync(id, reason);
        }
        #endregion

        #region Sessions
        public async Task<IEnumerable<MentorshipSessionDto>> GetMentorshipSessionsAsync(int mentorshipId, int currentUserId)
        {
            var mentorship = await _mentorshipRepository.GetByIdAsync(mentorshipId);
            if (mentorship == null)
            {
                throw new KeyNotFoundException($"Mentoría con ID {mentorshipId} no encontrada");
            }

            // Verificar permisos
            if (mentorship.MentorId != currentUserId && mentorship.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes acceso a las sesiones de esta mentoría");
            }

            var sessions = await _sessionRepository.GetByMentorshipIdAsync(mentorshipId);
            return sessions.Select(MapToSessionDto);
        }

        public async Task<MentorshipSessionDto?> GetSessionByIdAsync(int id, int currentUserId)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null) return null;

            // Verificar permisos
            if (session.Mentorship.MentorId != currentUserId && session.Mentorship.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes acceso a esta sesión");
            }

            return MapToSessionDto(session);
        }

        public async Task<MentorshipSessionDto> CreateSessionAsync(CreateSessionDto dto, int currentUserId)
        {
            var mentorship = await _mentorshipRepository.GetByIdAsync(dto.MentorshipId);
            if (mentorship == null)
            {
                throw new KeyNotFoundException($"Mentoría con ID {dto.MentorshipId} no encontrada");
            }

            // Solo el mentor puede crear sesiones
            if (mentorship.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el mentor puede crear sesiones");
            }

            if (mentorship.Status != MentorshipStatus.Active)
            {
                throw new InvalidOperationException("No se pueden crear sesiones para mentorías no activas");
            }

            // Verificar que no haya solapamiento de horarios
            var existingSessions = await _sessionRepository.GetSessionsByDateRangeAsync(
                dto.MentorshipId,
                dto.StartTime.AddMinutes(-30),
                dto.EndTime.AddMinutes(30)
            );

            if (existingSessions.Any())
            {
                throw new InvalidOperationException("Ya existe una sesión programada en ese horario");
            }

            var session = new MentorshipSession
            {
                MentorshipId = dto.MentorshipId,
                Title = dto.Title,
                Description = dto.Description,
                ScheduledDate = dto.ScheduledDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Agenda = dto.Agenda,
                MeetingMethod = dto.MeetingMethod,
                MeetingLink = dto.MeetingLink,
                SendReminder = dto.SendReminder,
                ReminderHoursBefore = dto.ReminderHoursBefore,
                Status = SessionStatus.Scheduled,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _sessionRepository.CreateAsync(session);
            return MapToSessionDto(created);
        }

        public async Task<MentorshipSessionDto> UpdateSessionAsync(int id, UpdateSessionDto dto, int currentUserId)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null)
            {
                throw new KeyNotFoundException($"Sesión con ID {id} no encontrada");
            }

            // Solo el mentor puede actualizar sesiones
            if (session.Mentorship.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el mentor puede actualizar sesiones");
            }

            // Actualizar campos
            if (!string.IsNullOrEmpty(dto.Title))
                session.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                session.Description = dto.Description;

            if (dto.ScheduledDate.HasValue)
                session.ScheduledDate = dto.ScheduledDate.Value;

            if (dto.StartTime.HasValue)
                session.StartTime = dto.StartTime.Value;

            if (dto.EndTime.HasValue)
                session.EndTime = dto.EndTime.Value;

            if (dto.Agenda != null)
                session.Agenda = dto.Agenda;

            if (dto.MeetingMethod.HasValue)
                session.MeetingMethod = dto.MeetingMethod.Value;

            if (dto.MeetingLink != null)
                session.MeetingLink = dto.MeetingLink;

            if (dto.Status.HasValue)
                session.Status = dto.Status.Value;

            if (dto.Notes != null)
                session.Notes = dto.Notes;

            if (dto.Homework != null)
                session.Homework = dto.Homework;

            var updated = await _sessionRepository.UpdateAsync(session);
            return MapToSessionDto(updated);
        }

        public async Task<bool> DeleteSessionAsync(int id, int currentUserId)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null)
            {
                throw new KeyNotFoundException($"Sesión con ID {id} no encontrada");
            }

            // Solo el mentor puede eliminar sesiones
            if (session.Mentorship.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el mentor puede eliminar sesiones");
            }

            // Solo se pueden eliminar sesiones futuras
            if (session.StartTime <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("No se pueden eliminar sesiones pasadas o en curso");
            }

            return await _sessionRepository.DeleteAsync(id);
        }

        public async Task<bool> StartSessionAsync(int id, int currentUserId)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null)
            {
                throw new KeyNotFoundException($"Sesión con ID {id} no encontrada");
            }

            // Solo el mentor puede iniciar sesiones
            if (session.Mentorship.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el mentor puede iniciar sesiones");
            }

            if (session.Status != SessionStatus.Scheduled)
            {
                throw new InvalidOperationException("Solo se pueden iniciar sesiones programadas");
            }

            if (session.StartTime > DateTime.UtcNow.AddMinutes(15))
            {
                throw new InvalidOperationException("No se puede iniciar la sesión más de 15 minutos antes");
            }

            return await _sessionRepository.StartSessionAsync(id);
        }

        public async Task<bool> EndSessionAsync(int id, int currentUserId)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null)
            {
                throw new KeyNotFoundException($"Sesión con ID {id} no encontrada");
            }

            // Solo el mentor puede finalizar sesiones
            if (session.Mentorship.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el mentor puede finalizar sesiones");
            }

            if (session.Status != SessionStatus.InProgress)
            {
                throw new InvalidOperationException("Solo se pueden finalizar sesiones en progreso");
            }

            return await _sessionRepository.EndSessionAsync(id);
        }
        #endregion

        #region Attendance
        public async Task<IEnumerable<AttendanceDto>> GetSessionAttendancesAsync(int sessionId, int currentUserId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                throw new KeyNotFoundException($"Sesión con ID {sessionId} no encontrada");
            }

            // Verificar permisos
            if (session.Mentorship.MentorId != currentUserId && session.Mentorship.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes acceso a las asistencias de esta sesión");
            }

            var attendances = await _attendanceRepository.GetBySessionIdAsync(sessionId);
            return attendances.Select(MapToAttendanceDto);
        }

        public async Task<AttendanceDto?> GetAttendanceAsync(int id, int currentUserId)
        {
            var attendance = await _attendanceRepository.GetByIdAsync(id);
            if (attendance == null) return null;

            // Verificar permisos
            if (attendance.UserId != currentUserId &&
                attendance.Session.Mentorship.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes acceso a esta asistencia");
            }

            return MapToAttendanceDto(attendance);
        }

        public async Task<AttendanceDto> UpdateAttendanceAsync(int id, UpdateAttendanceDto dto, int currentUserId)
        {
            var attendance = await _attendanceRepository.GetByIdAsync(id);
            if (attendance == null)
            {
                throw new KeyNotFoundException($"Asistencia con ID {id} no encontrada");
            }

            // Solo el usuario mismo o el mentor pueden actualizar asistencia
            if (attendance.UserId != currentUserId && attendance.Session.Mentorship.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes permisos para actualizar esta asistencia");
            }

            attendance.Status = dto.Status;
            attendance.Rating = dto.Rating;
            attendance.Feedback = dto.Feedback;
            attendance.UpdatedAt = DateTime.UtcNow;

            var updated = await _attendanceRepository.UpdateAsync(attendance);
            return MapToAttendanceDto(updated);
        }

        public async Task<bool> MarkAttendanceAsync(int sessionId, int userId, int currentUserId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                throw new KeyNotFoundException($"Sesión con ID {sessionId} no encontrada");
            }

            // Solo el mentor puede marcar asistencia de otros
            if (session.Mentorship.MentorId != currentUserId && userId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes permisos para marcar asistencia");
            }

            return await _attendanceRepository.MarkAttendanceAsync(sessionId, userId, AttendanceStatus.Attended);
        }
        #endregion

        #region Milestones
        public async Task<IEnumerable<MentorshipMilestoneDto>> GetMentorshipMilestonesAsync(int mentorshipId, int currentUserId)
        {
            var mentorship = await _mentorshipRepository.GetByIdAsync(mentorshipId);
            if (mentorship == null)
            {
                throw new KeyNotFoundException($"Mentoría con ID {mentorshipId} no encontrada");
            }

            // Verificar permisos
            if (mentorship.MentorId != currentUserId && mentorship.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes acceso a los hitos de esta mentoría");
            }

            var milestones = await _milestoneRepository.GetByMentorshipIdAsync(mentorshipId);
            return milestones.Select(MapToMilestoneDto);
        }

        public async Task<MentorshipMilestoneDto?> GetMilestoneByIdAsync(int id, int currentUserId)
        {
            var milestone = await _milestoneRepository.GetByIdAsync(id);
            if (milestone == null) return null;

            // Verificar permisos
            if (milestone.Mentorship.MentorId != currentUserId && milestone.Mentorship.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes acceso a este hito");
            }

            return MapToMilestoneDto(milestone);
        }

        public async Task<MentorshipMilestoneDto> CreateMilestoneAsync(CreateMilestoneDto dto, int currentUserId)
        {
            var mentorship = await _mentorshipRepository.GetByIdAsync(dto.MentorshipId);
            if (mentorship == null)
            {
                throw new KeyNotFoundException($"Mentoría con ID {dto.MentorshipId} no encontrada");
            }

            // Solo el mentor puede crear hitos
            if (mentorship.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el mentor puede crear hitos");
            }

            if (mentorship.Status != MentorshipStatus.Active)
            {
                throw new InvalidOperationException("No se pueden crear hitos para mentorías no activas");
            }

            var milestone = new MentorshipMilestone
            {
                MentorshipId = dto.MentorshipId,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Status = MilestoneStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _milestoneRepository.CreateAsync(milestone);
            return MapToMilestoneDto(created);
        }

        public async Task<MentorshipMilestoneDto> UpdateMilestoneAsync(int id, UpdateMilestoneDto dto, int currentUserId)
        {
            var milestone = await _milestoneRepository.GetByIdAsync(id);
            if (milestone == null)
            {
                throw new KeyNotFoundException($"Hito con ID {id} no encontrado");
            }

            // Solo el mentor puede actualizar hitos
            if (milestone.Mentorship.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el mentor puede actualizar hitos");
            }

            // Actualizar campos
            if (!string.IsNullOrEmpty(dto.Title))
                milestone.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                milestone.Description = dto.Description;

            if (dto.DueDate.HasValue)
                milestone.DueDate = dto.DueDate.Value;

            if (dto.CompletedDate.HasValue)
                milestone.CompletedDate = dto.CompletedDate;

            if (dto.Status.HasValue)
                milestone.Status = dto.Status.Value;

            if (dto.ProgressPercentage.HasValue)
                milestone.ProgressPercentage = dto.ProgressPercentage.Value;

            if (dto.Notes != null)
                milestone.Notes = dto.Notes;

            if (dto.Evidence != null)
                milestone.Evidence = dto.Evidence;

            var updated = await _milestoneRepository.UpdateAsync(milestone);
            return MapToMilestoneDto(updated);
        }

        public async Task<bool> DeleteMilestoneAsync(int id, int currentUserId)
        {
            var milestone = await _milestoneRepository.GetByIdAsync(id);
            if (milestone == null)
            {
                throw new KeyNotFoundException($"Hito con ID {id} no encontrado");
            }

            // Solo el mentor puede eliminar hitos
            if (milestone.Mentorship.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el mentor puede eliminar hitos");
            }

            return await _milestoneRepository.DeleteAsync(id);
        }

        public async Task<bool> MarkMilestoneCompleteAsync(int id, int currentUserId, string? evidence)
        {
            var milestone = await _milestoneRepository.GetByIdAsync(id);
            if (milestone == null)
            {
                throw new KeyNotFoundException($"Hito con ID {id} no encontrado");
            }

            // Solo el mentor puede marcar hitos como completados
            if (milestone.Mentorship.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el mentor puede marcar hitos como completados");
            }

            return await _milestoneRepository.MarkCompleteAsync(id, DateTime.UtcNow, evidence);
        }
        #endregion

        #region Resources
        public async Task<IEnumerable<MentorshipResourceDto>> GetMentorshipResourcesAsync(int mentorshipId, int currentUserId)
        {
            var mentorship = await _mentorshipRepository.GetByIdAsync(mentorshipId);
            if (mentorship == null)
            {
                throw new KeyNotFoundException($"Mentoría con ID {mentorshipId} no encontrada");
            }

            // Verificar permisos
            if (mentorship.MentorId != currentUserId && mentorship.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes acceso a los recursos de esta mentoría");
            }

            var resources = await _resourceRepository.GetByMentorshipIdAsync(mentorshipId);
            return resources.Select(MapToResourceDto);
        }

        public async Task<MentorshipResourceDto?> GetResourceByIdAsync(int id, int currentUserId)
        {
            var resource = await _resourceRepository.GetByIdAsync(id);
            if (resource == null) return null;

            // Verificar permisos
            if (!resource.IsPublic &&
                resource.Mentorship.MentorId != currentUserId &&
                resource.Mentorship.MenteeId != currentUserId &&
                resource.CreatedByUserId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes acceso a este recurso");
            }

            return MapToResourceDto(resource);
        }

        public async Task<MentorshipResourceDto> CreateResourceAsync(CreateResourceDto dto, int currentUserId)
        {
            var mentorship = await _mentorshipRepository.GetByIdAsync(dto.MentorshipId);
            if (mentorship == null)
            {
                throw new KeyNotFoundException($"Mentoría con ID {dto.MentorshipId} no encontrada");
            }

            // Solo mentor o mentee pueden crear recursos
            if (mentorship.MentorId != currentUserId && mentorship.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes permisos para crear recursos en esta mentoría");
            }

            var resource = new MentorshipResource
            {
                MentorshipId = dto.MentorshipId,
                SessionId = dto.SessionId,
                Title = dto.Title,
                Description = dto.Description,
                Type = dto.Type,
                Url = dto.Url,
                ThumbnailUrl = dto.ThumbnailUrl,
                IsPublic = dto.IsPublic,
                IsDownloadable = dto.IsDownloadable,
                CreatedByUserId = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _resourceRepository.CreateAsync(resource);
            return MapToResourceDto(created);
        }

        public async Task<MentorshipResourceDto> UpdateResourceAsync(int id, UpdateResourceDto dto, int currentUserId)
        {
            var resource = await _resourceRepository.GetByIdAsync(id);
            if (resource == null)
            {
                throw new KeyNotFoundException($"Recurso con ID {id} no encontrado");
            }

            // Solo el creador puede actualizar el recurso
            if (resource.CreatedByUserId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el creador puede actualizar este recurso");
            }

            // Actualizar campos
            if (!string.IsNullOrEmpty(dto.Title))
                resource.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                resource.Description = dto.Description;

            if (dto.Type.HasValue)
                resource.Type = dto.Type.Value;

            if (!string.IsNullOrEmpty(dto.Url))
                resource.Url = dto.Url;

            if (dto.ThumbnailUrl != null)
                resource.ThumbnailUrl = dto.ThumbnailUrl;

            if (dto.IsPublic.HasValue)
                resource.IsPublic = dto.IsPublic.Value;

            if (dto.IsDownloadable.HasValue)
                resource.IsDownloadable = dto.IsDownloadable.Value;

            var updated = await _resourceRepository.UpdateAsync(resource);
            return MapToResourceDto(updated);
        }

        public async Task<bool> DeleteResourceAsync(int id, int currentUserId)
        {
            var resource = await _resourceRepository.GetByIdAsync(id);
            if (resource == null)
            {
                throw new KeyNotFoundException($"Recurso con ID {id} no encontrado");
            }

            // Solo el creador o el mentor pueden eliminar recursos
            if (resource.CreatedByUserId != currentUserId && resource.Mentorship.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tienes permisos para eliminar este recurso");
            }

            return await _resourceRepository.DeleteAsync(id);
        }
        #endregion

        #region Requests & Applications
        public async Task<IEnumerable<MentorshipRequestDto>> GetMentorshipRequestsAsync(int? userId = null)
        {
            IEnumerable<MentorshipRequest> requests;

            if (userId.HasValue)
            {
                requests = await _requestRepository.GetByMenteeIdAsync(userId.Value);
            }
            else
            {
                requests = await _requestRepository.GetOpenRequestsAsync();
            }

            return requests.Select(MapToRequestDto);
        }

        public async Task<MentorshipRequestDto?> GetRequestByIdAsync(int id, int currentUserId)
        {
            var request = await _requestRepository.GetByIdWithDetailsAsync(id);
            if (request == null) return null;

            // Verificar permisos: solo mentee, mentor asignado o mentores pueden ver
            var userApplication = await _applicationRepository.GetUserApplicationAsync(id, currentUserId);
            if (request.MenteeId != currentUserId &&
                request.MentorId != currentUserId &&
                userApplication == null)
            {
                throw new UnauthorizedAccessException("No tienes acceso a esta solicitud");
            }

            return MapToRequestDto(request);
        }

        public async Task<MentorshipRequestDto> CreateRequestAsync(CreateMentorshipRequestDto dto, int currentUserId)
        {
            // Verificar que el usuario no es mentor para sí mismo
            if (dto.MentorId.HasValue && dto.MentorId.Value == currentUserId)
            {
                throw new InvalidOperationException("No puedes solicitar mentoría a ti mismo");
            }

            var request = new MentorshipRequest
            {
                MenteeId = currentUserId,
                MentorId = dto.MentorId,
                Title = dto.Title,
                Description = dto.Description,
                Goals = dto.Goals,
                Category = dto.Category,
                Tags = dto.Tags,
                PreferredDurationWeeks = dto.PreferredDurationWeeks,
                PreferredSessionsPerWeek = dto.PreferredSessionsPerWeek,
                PreferredMeetingMethod = dto.PreferredMeetingMethod,
                Status = dto.MentorId.HasValue ? RequestStatus.Pending : RequestStatus.Reviewing,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            var created = await _requestRepository.CreateAsync(request);
            return MapToRequestDto(created);
        }

        public async Task<bool> DeleteRequestAsync(int id, int currentUserId)
        {
            var request = await _requestRepository.GetByIdAsync(id);
            if (request == null)
            {
                throw new KeyNotFoundException($"Solicitud con ID {id} no encontrada");
            }

            // Solo el mentee puede eliminar su solicitud
            if (request.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo puedes eliminar tus propias solicitudes");
            }

            // Solo se pueden eliminar solicitudes pendientes
            if (request.Status != RequestStatus.Pending && request.Status != RequestStatus.Reviewing)
            {
                throw new InvalidOperationException("Solo se pueden eliminar solicitudes pendientes");
            }

            return await _requestRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MentorApplicationDto>> GetRequestApplicationsAsync(int requestId, int currentUserId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null)
            {
                throw new KeyNotFoundException($"Solicitud con ID {requestId} no encontrada");
            }

            // Solo el mentee puede ver las aplicaciones a su solicitud
            if (request.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el mentee puede ver las aplicaciones a su solicitud");
            }

            var applications = await _applicationRepository.GetByRequestIdAsync(requestId);
            return applications.Select(MapToApplicationDto);
        }

        public async Task<MentorApplicationDto> ApplyToRequestAsync(CreateMentorApplicationDto dto, int currentUserId)
        {
            var request = await _requestRepository.GetByIdAsync(dto.RequestId);
            if (request == null)
            {
                throw new KeyNotFoundException($"Solicitud con ID {dto.RequestId} no encontrada");
            }

            // Verificar que la solicitud está abierta
            if (request.Status != RequestStatus.Reviewing)
            {
                throw new InvalidOperationException("Esta solicitud no está aceptando aplicaciones");
            }

            // Verificar que el mentor existe y está disponible
            var mentor = await _userRepository.GetByIdAsync(currentUserId);
            if (mentor == null || !mentor.IsMentor || !mentor.AvailableForMentorship)
            {
                throw new InvalidOperationException("No estás habilitado como mentor o no estás disponible");
            }

            // Verificar que no ha aplicado antes
            var existingApplication = await _requestRepository.HasUserAppliedAsync(dto.RequestId, currentUserId);
            if (existingApplication != null)
            {
                throw new InvalidOperationException("Ya has aplicado a esta solicitud");
            }

            var application = new MentorApplication
            {
                RequestId = dto.RequestId,
                MentorId = currentUserId,
                Proposal = dto.Proposal,
                Experience = dto.Experience,
                WhyChooseMe = dto.WhyChooseMe,
                ProposedDurationWeeks = dto.ProposedDurationWeeks,
                ProposedSessionsPerWeek = dto.ProposedSessionsPerWeek,
                ProposedMeetingMethod = dto.ProposedMeetingMethod,
                Status = ApplicationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _applicationRepository.CreateAsync(application);
            return MapToApplicationDto(created);
        }

        public async Task<bool> WithdrawApplicationAsync(int applicationId, int currentUserId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
            {
                throw new KeyNotFoundException($"Aplicación con ID {applicationId} no encontrada");
            }

            // Solo el mentor puede retirar su aplicación
            if (application.MentorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo puedes retirar tus propias aplicaciones");
            }

            return await _applicationRepository.WithdrawApplicationAsync(applicationId);
        }

        public async Task<bool> AcceptApplicationAsync(int applicationId, int currentUserId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
            {
                throw new KeyNotFoundException($"Aplicación con ID {applicationId} no encontrada");
            }

            var request = await _requestRepository.GetByIdAsync(application.RequestId);
            if (request == null)
            {
                throw new KeyNotFoundException("Solicitud no encontrada");
            }

            // Solo el mentee puede aceptar aplicaciones
            if (request.MenteeId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el mentee puede aceptar aplicaciones");
            }

            // Cambiar estado de la aplicación
            await _applicationRepository.ChangeStatusAsync(applicationId, ApplicationStatus.Accepted);

            // Asignar mentor a la solicitud
            await _requestRepository.AssignMentorAsync(request.Id, application.MentorId);

            // Crear automáticamente la mentoría
            var mentorship = new Mentorship
            {
                MentorId = application.MentorId,
                MenteeId = request.MenteeId,
                Title = request.Title,
                Description = request.Description,
                Goals = request.Goals,
                Category = request.Category,
                Tags = request.Tags,
                DurationWeeks = application.ProposedDurationWeeks ?? request.PreferredDurationWeeks,
                SessionsPerWeek = application.ProposedSessionsPerWeek ?? request.PreferredSessionsPerWeek,
                SessionDurationMinutes = 60, // Valor por defecto
                MeetingMethod = application.ProposedMeetingMethod ?? request.PreferredMeetingMethod,
                Status = MentorshipStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var createdMentorship = await _mentorshipRepository.CreateAsync(mentorship);

            // Vincular mentoría a la solicitud
            await _requestRepository.LinkToMentorshipAsync(request.Id, createdMentorship.Id);

            return true;
        }
        #endregion

        #region Mentor Profile
        public async Task<MentorProfileDto?> GetMentorProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsMentor) return null;

            var activeMentorships = await _mentorshipRepository.CountActiveMentorshipsAsync(userId);
            var totalMentees = await _mentorshipRepository.CountTotalMenteesAsync(userId);
            var averageRating = await _mentorshipRepository.CalculateMentorRatingAsync(userId);

            return new MentorProfileDto
            {
                UserId = user.Id,
                UserName = user.FullName,
                UserEmail = user.Email,
                MentorTitle = user.MentorTitle,
                MentorBio = user.MentorBio,
                YearsOfExperience = user.YearsOfExperience,
                MentorExpertise = user.MentorExpertise,
                MentorIndustries = user.MentorIndustries,
                HourlyRate = user.HourlyRate,
                AvailableForMentorship = user.AvailableForMentorship,
                MaxMentees = user.MaxMentees,
                CurrentMenteesCount = activeMentorships,
                MentorAverageRating = averageRating,
                TotalMentorshipSessions = user.TotalMentorshipSessions,
                TotalMenteesHelped = totalMentees,
                PreferredMentorshipCategories = user.PreferredMentorshipCategories,
                PreferredMeetingMethod = user.PreferredMeetingMethod,
                Timezone = user.Timezone
            };
        }

        public async Task<MentorProfileDto> UpdateMentorProfileAsync(int userId, UpdateMentorProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");
            }

            // Actualizar campos
            if (dto.IsMentor.HasValue)
                user.IsMentor = dto.IsMentor.Value;

            if (!string.IsNullOrEmpty(dto.MentorTitle))
                user.MentorTitle = dto.MentorTitle;

            if (!string.IsNullOrEmpty(dto.MentorBio))
                user.MentorBio = dto.MentorBio;

            if (dto.YearsOfExperience.HasValue)
                user.YearsOfExperience = dto.YearsOfExperience;

            if (dto.MentorExpertise != null)
                user.MentorExpertise = dto.MentorExpertise;

            if (dto.MentorIndustries != null)
                user.MentorIndustries = dto.MentorIndustries;

            if (dto.HourlyRate.HasValue)
                user.HourlyRate = dto.HourlyRate;

            if (dto.AvailableForMentorship.HasValue)
                user.AvailableForMentorship = dto.AvailableForMentorship.Value;

            if (dto.MaxMentees.HasValue)
                user.MaxMentees = dto.MaxMentees.Value;

            if (dto.PreferredMentorshipCategories != null)
                user.PreferredMentorshipCategories = dto.PreferredMentorshipCategories;

            if (dto.PreferredMeetingMethod.HasValue)
                user.PreferredMeetingMethod = dto.PreferredMeetingMethod;

            if (!string.IsNullOrEmpty(dto.Timezone))
                user.Timezone = dto.Timezone;

            if (!string.IsNullOrEmpty(dto.AvailabilitySchedule))
                user.AvailabilitySchedule = dto.AvailabilitySchedule;

            await _userRepository.UpdateAsync(user);

            // Devolver perfil actualizado
            return await GetMentorProfileAsync(userId) ?? throw new InvalidOperationException("Error al obtener perfil actualizado");
        }

        public async Task<IEnumerable<MentorProfileDto>> SearchMentorsAsync(SearchMentorsDto dto)
        {
            // Obtener todos los usuarios y filtrar localmente
            var allUsers = await _userRepository.GetAllAsync();

            var query = allUsers.AsQueryable()
                .Where(u => u.IsMentor && u.AvailableForMentorship);

            // Aplicar filtros en memoria (esto puede ser ineficiente para grandes volúmenes de datos)
            if (!string.IsNullOrEmpty(dto.Category))
            {
                query = query.Where(u => u.PreferredMentorshipCategories.Contains(dto.Category));
            }

            if (dto.Expertise != null && dto.Expertise.Any())
            {
                query = query.Where(u => u.MentorExpertise.Any(e => dto.Expertise.Contains(e)));
            }

            if (dto.MaxHourlyRate.HasValue)
            {
                query = query.Where(u => u.HourlyRate <= dto.MaxHourlyRate.Value);
            }

            if (dto.MinYearsOfExperience.HasValue)
            {
                query = query.Where(u => u.YearsOfExperience >= dto.MinYearsOfExperience.Value);
            }

            if (dto.MinRating.HasValue)
            {
                query = query.Where(u => u.MentorAverageRating >= dto.MinRating.Value);
            }

            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                var searchTerm = dto.SearchTerm.ToLower();
                query = query.Where(u =>
                    (u.FullName != null && u.FullName.ToLower().Contains(searchTerm)) ||
                    (u.MentorTitle != null && u.MentorTitle.ToLower().Contains(searchTerm)) ||
                    (u.MentorBio != null && u.MentorBio.ToLower().Contains(searchTerm)) ||
                    (u.MentorExpertise != null && u.MentorExpertise.Any(e => e != null && e.ToLower().Contains(searchTerm))) ||
                    (u.PreferredMentorshipCategories != null && u.PreferredMentorshipCategories.Any(c => c != null && c.ToLower().Contains(searchTerm)))
                );
            }

            var mentors = query
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToList();

            var profiles = new List<MentorProfileDto>();
            foreach (var mentor in mentors)
            {
                var profile = await GetMentorProfileAsync(mentor.Id);
                if (profile != null) profiles.Add(profile);
            }

            return profiles;
        }

        public async Task<IEnumerable<MentorProfileDto>> GetRecommendedMentorsAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.PreferredMentorshipCategories == null || !user.PreferredMentorshipCategories.Any())
                return new List<MentorProfileDto>();

            // Obtener todos los usuarios y filtrar localmente
            var allUsers = await _userRepository.GetAllAsync();

            // Filtrar mentores que coincidan con las categorías preferidas del usuario
            var recommendedMentors = allUsers
                .Where(u => u.IsMentor &&
                           u.AvailableForMentorship &&
                           u.PreferredMentorshipCategories != null &&
                           user.PreferredMentorshipCategories != null &&
                           u.PreferredMentorshipCategories.Any(c =>
                               user.PreferredMentorshipCategories.Contains(c)))
                .Take(10)
                .ToList();

            var profiles = new List<MentorProfileDto>();
            foreach (var mentor in recommendedMentors)
            {
                var profile = await GetMentorProfileAsync(mentor.Id);
                if (profile != null) profiles.Add(profile);
            }

            return profiles;
        }
        #endregion

        #region Mappers
        private MentorshipDto MapToMentorshipDto(Mentorship mentorship)
        {
            return new MentorshipDto
            {
                Id = mentorship.Id,
                MentorId = mentorship.MentorId,
                MentorName = mentorship.Mentor.FullName,
                MentorEmail = mentorship.Mentor.Email,
                MenteeId = mentorship.MenteeId,
                MenteeName = mentorship.Mentee.FullName,
                MenteeEmail = mentorship.Mentee.Email,
                Title = mentorship.Title,
                Description = mentorship.Description,
                Category = mentorship.Category,
                Tags = mentorship.Tags,
                Status = mentorship.Status,
                StartDate = mentorship.StartDate,
                EndDate = mentorship.EndDate,
                DurationWeeks = mentorship.DurationWeeks,
                TotalSessions = mentorship.Sessions?.Count ?? 0,
                CompletedSessions = mentorship.Sessions?.Count(s => s.Status == SessionStatus.Completed) ?? 0,
                TotalMilestones = mentorship.Milestones?.Count ?? 0,
                CompletedMilestones = mentorship.Milestones?.Count(m => m.Status == MilestoneStatus.Completed) ?? 0,
                MentorRating = mentorship.MentorRating,
                MenteeRating = mentorship.MenteeRating,
                CreatedAt = mentorship.CreatedAt
            };
        }

        private MentorshipSessionDto MapToSessionDto(MentorshipSession session)
        {
            return new MentorshipSessionDto
            {
                Id = session.Id,
                MentorshipId = session.MentorshipId,
                Title = session.Title,
                Description = session.Description,
                ScheduledDate = session.ScheduledDate,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Status = session.Status,
                MeetingMethod = session.MeetingMethod,
                MeetingLink = session.MeetingLink,
                Notes = session.Notes,
                Homework = session.Homework,
                SendReminder = session.SendReminder,
                CreatedAt = session.CreatedAt,
                Attendances = session.Attendances?.Select(MapToAttendanceDto).ToList() ?? new()
            };
        }

        private AttendanceDto MapToAttendanceDto(SessionAttendance attendance)
        {
            return new AttendanceDto
            {
                Id = attendance.Id,
                SessionId = attendance.SessionId,
                UserId = attendance.UserId,
                UserName = attendance.User.FullName,
                UserEmail = attendance.User.Email,
                Status = attendance.Status,
                JoinedAt = attendance.JoinedAt,
                AttendanceMinutes = attendance.AttendanceMinutes,
                Rating = attendance.Rating,
                Feedback = attendance.Feedback
            };
        }

        private MentorshipMilestoneDto MapToMilestoneDto(MentorshipMilestone milestone)
        {
            return new MentorshipMilestoneDto
            {
                Id = milestone.Id,
                MentorshipId = milestone.MentorshipId,
                Title = milestone.Title,
                Description = milestone.Description,
                DueDate = milestone.DueDate,
                CompletedDate = milestone.CompletedDate,
                Status = milestone.Status,
                ProgressPercentage = milestone.ProgressPercentage,
                CreatedAt = milestone.CreatedAt
            };
        }

        private MentorshipResourceDto MapToResourceDto(MentorshipResource resource)
        {
            return new MentorshipResourceDto
            {
                Id = resource.Id,
                MentorshipId = resource.MentorshipId,
                SessionId = resource.SessionId,
                Title = resource.Title,
                Description = resource.Description,
                Type = resource.Type.ToString(),
                Url = resource.Url,
                ThumbnailUrl = resource.ThumbnailUrl,
                IsPublic = resource.IsPublic,
                IsDownloadable = resource.IsDownloadable,
                CreatedByUserId = resource.CreatedByUserId,
                CreatedByName = resource.CreatedBy.FullName,
                CreatedAt = resource.CreatedAt
            };
        }

        private MentorshipRequestDto MapToRequestDto(MentorshipRequest request)
        {
            return new MentorshipRequestDto
            {
                Id = request.Id,
                MenteeId = request.MenteeId,
                MenteeName = request.Mentee.FullName,
                MentorId = request.MentorId,
                MentorName = request.Mentor?.FullName,
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                Tags = request.Tags,
                Status = request.Status,
                TotalApplications = request.MentorApplications?.Count ?? 0,
                CreatedAt = request.CreatedAt,
                ExpiresAt = request.ExpiresAt
            };
        }

        private MentorApplicationDto MapToApplicationDto(MentorApplication application)
        {
            return new MentorApplicationDto
            {
                Id = application.Id,
                RequestId = application.RequestId,
                MentorId = application.MentorId,
                MentorName = application.Mentor.FullName,
                MentorTitle = application.Mentor.MentorTitle,
                MentorRating = application.Mentor.MentorAverageRating,
                MentorExperienceYears = application.Mentor.YearsOfExperience,
                Proposal = application.Proposal,
                Status = application.Status,
                CreatedAt = application.CreatedAt
            };
        }
        #endregion
    }
}