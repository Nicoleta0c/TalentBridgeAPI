using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentBridge.API.DTOs.CommunityDTOs;
using TalentBridge.API.Interfaces.ICommunity;

namespace TalentBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommunitiesController : ControllerBase
    {
        private readonly ICommunityService _communityService;
        private readonly ILogger<CommunitiesController> _logger;

        public CommunitiesController(
            ICommunityService communityService,
            ILogger<CommunitiesController> logger)
        {
            _communityService = communityService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las comunidades
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommunityDto>>> GetAll()
        {
            try
            {
                var communities = await _communityService.GetAllAsync();
                return Ok(communities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las comunidades");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene comunidades públicas
        /// </summary>
        [HttpGet("public")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommunityDto>>> GetPublicCommunities()
        {
            try
            {
                var communities = await _communityService.GetPublicCommunitiesAsync();
                return Ok(communities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener comunidades públicas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene comunidades por universidad
        /// </summary>
        [HttpGet("university/{universityId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommunityDto>>> GetByUniversity(int universityId)
        {
            try
            {
                var communities = await _communityService.GetByUniversityIdAsync(universityId);
                return Ok(communities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener comunidades de universidad {UniversityId}", universityId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene las comunidades del usuario
        /// </summary>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommunityDto>>> GetUserCommunities(int userId)
        {
            try
            {
                var communities = await _communityService.GetUserCommunitiesAsync(userId);
                return Ok(communities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener comunidades del usuario {UserId}", userId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene una comunidad por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommunityDto>> GetById(int id)
        {
            try
            {
                var community = await _communityService.GetByIdAsync(id);

                if (community == null)
                {
                    return NotFound(new { message = $"Comunidad con ID {id} no encontrada" });
                }

                return Ok(community);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener comunidad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene detalles completos de una comunidad
        /// </summary>
        [HttpGet("{id}/details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommunityDetailDto>> GetDetailById(int id)
        {
            try
            {
                var community = await _communityService.GetDetailByIdAsync(id);

                if (community == null)
                {
                    return NotFound(new { message = $"Comunidad con ID {id} no encontrada" });
                }

                return Ok(community);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de comunidad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea una nueva comunidad
        /// 📝 Ingresa el UserId en el formulario de Swagger
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CommunityDto>> Create([FromBody] CreateCommunityDto dto)
        {
            try
            {
                _logger.LogInformation("🚀 POST /api/Communities - Crear comunidad");
                _logger.LogInformation($"📝 UserId: {dto.UserId}, Comunidad: {dto.Name}");

                // ✅ Validar que el userId sea válido
                if (dto.UserId <= 0)
                {
                    return BadRequest(new { message = "UserId debe ser mayor a 0" });
                }

                var community = await _communityService.CreateAsync(dto, dto.UserId);
                return CreatedAtAction(nameof(GetById), new { id = community.Id }, community);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Universidad o Usuario no encontrado");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear comunidad");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza una comunidad
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CommunityDto>> Update(int id, [FromBody] UpdateCommunityDto dto, [FromQuery] int userId)
        {
            try
            {
                _logger.LogInformation($"🔄 PUT /api/Communities/{id} - Usuario: {userId}");

                if (userId <= 0)
                {
                    return BadRequest(new { message = "UserId debe ser mayor a 0" });
                }

                var community = await _communityService.UpdateAsync(id, dto, userId);
                return Ok(community);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar comunidad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina una comunidad
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(int id, [FromQuery] int userId)
        {
            try
            {
                _logger.LogInformation($"🗑️ DELETE /api/Communities/{id} - Usuario: {userId}");

                if (userId <= 0)
                {
                    return BadRequest(new { message = "UserId debe ser mayor a 0" });
                }

                await _communityService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar comunidad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Unirse a una comunidad
        /// </summary>
        [HttpPost("{id}/join")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> JoinCommunity(int id, [FromQuery] int userId)
        {
            try
            {
                _logger.LogInformation($"➕ JOIN /api/Communities/{id} - Usuario: {userId}");

                if (userId <= 0)
                {
                    return BadRequest(new { message = "UserId debe ser mayor a 0" });
                }

                await _communityService.JoinCommunityAsync(id, userId);
                return Ok(new { message = "Te has unido a la comunidad exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al unirse a comunidad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Salir de una comunidad
        /// </summary>
        [HttpPost("{id}/leave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LeaveCommunity(int id, [FromQuery] int userId)
        {
            try
            {
                _logger.LogInformation($"➖ LEAVE /api/Communities/{id} - Usuario: {userId}");

                if (userId <= 0)
                {
                    return BadRequest(new { message = "UserId debe ser mayor a 0" });
                }

                await _communityService.LeaveCommunityAsync(id, userId);
                return Ok(new { message = "Has salido de la comunidad exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al salir de comunidad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene los miembros de una comunidad
        /// </summary>
        [HttpGet("{id}/members")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CommunityMemberDto>>> GetMembers(int id)
        {
            try
            {
                var members = await _communityService.GetMembersAsync(id);
                return Ok(members);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener miembros de comunidad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza el rol de un miembro
        /// </summary>
        [HttpPatch("{id}/members/role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMemberRole(int id, [FromBody] UpdateMemberRoleDto dto, [FromQuery] int userId)
        {
            try
            {
                _logger.LogInformation($"👤 UPDATE ROLE - Comunidad: {id}, Admin: {userId}, Miembro: {dto.MemberId}");

                if (userId <= 0)
                {
                    return BadRequest(new { message = "UserId debe ser mayor a 0" });
                }

                await _communityService.UpdateMemberRoleAsync(id, dto.MemberId, dto.Role, userId);
                return Ok(new { message = "Rol actualizado exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar rol de miembro en comunidad {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Remueve un miembro de la comunidad
        /// </summary>
        [HttpDelete("{id}/members/{memberId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveMember(int id, int memberId, [FromQuery] int userId)
        {
            try
            {
                _logger.LogInformation($"❌ REMOVE MEMBER - Comunidad: {id}, Admin: {userId}, Miembro: {memberId}");

                if (userId <= 0)
                {
                    return BadRequest(new { message = "UserId debe ser mayor a 0" });
                }

                await _communityService.RemoveMemberAsync(id, memberId, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover miembro {MemberId} de comunidad {Id}", memberId, id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}