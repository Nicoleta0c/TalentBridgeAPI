using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentBridge.API.DTOs.MentorshipDTOs;
using TalentBridge.API.Interfaces.IMentorship;

namespace TalentBridge.API.Controllers.MentorshipControllers
{
    [ApiController]
    [Route("api/mentorships/{mentorshipId}/sessions")]
    public class MentorshipSessionsController : ControllerBase
    {
        private readonly IMentorshipService _mentorshipService;
        private readonly ILogger<MentorshipSessionsController> _logger;

        public MentorshipSessionsController(
            IMentorshipService mentorshipService,
            ILogger<MentorshipSessionsController> logger)
        {
            _mentorshipService = mentorshipService;
            _logger = logger;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        /// <summary>
        /// Obtiene todas las sesiones de una mentoría
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<MentorshipSessionDto>>> GetSessions(int mentorshipId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var sessions = await _mentorshipService.GetMentorshipSessionsAsync(mentorshipId, userId.Value);
                return Ok(sessions);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener sesiones de mentoría {MentorshipId}", mentorshipId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene una sesión por ID
        /// </summary>
        [HttpGet("{sessionId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MentorshipSessionDto>> GetSession(int mentorshipId, int sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var session = await _mentorshipService.GetSessionByIdAsync(sessionId, userId.Value);

                if (session == null)
                {
                    return NotFound(new { message = $"Sesión con ID {sessionId} no encontrada" });
                }

                return Ok(session);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener sesión {SessionId} de mentoría {MentorshipId}", sessionId, mentorshipId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea una nueva sesión
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MentorshipSessionDto>> CreateSession(int mentorshipId, [FromBody] CreateSessionDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                dto.MentorshipId = mentorshipId;
                var session = await _mentorshipService.CreateSessionAsync(dto, userId.Value);
                return CreatedAtAction(nameof(GetSession), new { mentorshipId, sessionId = session.Id }, session);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear sesión en mentoría {MentorshipId}", mentorshipId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza una sesión existente
        /// </summary>
        [HttpPut("{sessionId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<MentorshipSessionDto>> UpdateSession(int mentorshipId, int sessionId, [FromBody] UpdateSessionDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var session = await _mentorshipService.UpdateSessionAsync(sessionId, dto, userId.Value);
                return Ok(session);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar sesión {SessionId}", sessionId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina una sesión
        /// </summary>
        [HttpDelete("{sessionId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteSession(int mentorshipId, int sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                await _mentorshipService.DeleteSessionAsync(sessionId, userId.Value);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar sesión {SessionId}", sessionId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Inicia una sesión
        /// </summary>
        [HttpPost("{sessionId}/start")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> StartSession(int mentorshipId, int sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                await _mentorshipService.StartSessionAsync(sessionId, userId.Value);
                return Ok(new { message = "Sesión iniciada exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión {SessionId}", sessionId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Finaliza una sesión
        /// </summary>
        [HttpPost("{sessionId}/end")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> EndSession(int mentorshipId, int sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                await _mentorshipService.EndSessionAsync(sessionId, userId.Value);
                return Ok(new { message = "Sesión finalizada exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al finalizar sesión {SessionId}", sessionId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene las asistencias de una sesión
        /// </summary>
        [HttpGet("{sessionId}/attendances")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetSessionAttendances(int mentorshipId, int sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var attendances = await _mentorshipService.GetSessionAttendancesAsync(sessionId, userId.Value);
                return Ok(attendances);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias de sesión {SessionId}", sessionId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Marca asistencia de un usuario
        /// </summary>
        [HttpPost("{sessionId}/attendances/{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> MarkAttendance(int mentorshipId, int sessionId, int userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                await _mentorshipService.MarkAttendanceAsync(sessionId, userId, currentUserId.Value);
                return Ok(new { message = "Asistencia marcada exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar asistencia de usuario {UserId} en sesión {SessionId}", userId, sessionId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}