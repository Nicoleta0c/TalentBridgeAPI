using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentBridge.API.DTOs.CommunityDTOs;
using TalentBridge.API.Interfaces.ICommunity;

namespace TalentBridge.API.Controllers
{
    [ApiController]
    [Route("api/communities/{communityId}/posts")]
    public class CommunityPostsController : ControllerBase
    {
        private readonly ICommunityPostService _postService;
        private readonly ILogger<CommunityPostsController> _logger;

        public CommunityPostsController(
            ICommunityPostService postService,
            ILogger<CommunityPostsController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        /// <summary>
        /// Obtiene todos los posts de una comunidad
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CommunityPostDto>>> GetCommunityPosts(int communityId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var posts = await _postService.GetByCommunityIdAsync(communityId, userId);
                return Ok(posts);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener posts de comunidad {CommunityId}", communityId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un post por ID
        /// </summary>
        [HttpGet("{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommunityPostDto>> GetPost(int communityId, int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var post = await _postService.GetByIdAsync(postId, userId);

                if (post == null)
                {
                    return NotFound(new { message = $"Post con ID {postId} no encontrado" });
                }

                return Ok(post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener post {PostId} de comunidad {CommunityId}", postId, communityId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene detalles completos de un post
        /// </summary>
        [HttpGet("{postId}/details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDetailDto>> GetPostDetails(int communityId, int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var post = await _postService.GetDetailByIdAsync(postId, userId);

                if (post == null)
                {
                    return NotFound(new { message = $"Post con ID {postId} no encontrado" });
                }

                return Ok(post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de post {PostId}", postId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea un nuevo post en la comunidad
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommunityPostDto>> CreatePost(int communityId, [FromBody] CreatePostDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                // Asegurar que el post pertenece a la comunidad especificada
                dto.CommunityId = communityId;

                var post = await _postService.CreateAsync(dto, userId.Value);
                return CreatedAtAction(nameof(GetPost), new { communityId, postId = post.Id }, post);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear post en comunidad {CommunityId}", communityId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza un post existente
        /// </summary>
        [HttpPut("{postId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CommunityPostDto>> UpdatePost(int communityId, int postId, [FromBody] UpdatePostDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var post = await _postService.UpdateAsync(postId, dto, userId.Value);
                return Ok(post);
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
                _logger.LogError(ex, "Error al actualizar post {PostId}", postId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina un post
        /// </summary>
        [HttpDelete("{postId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeletePost(int communityId, int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                await _postService.DeleteAsync(postId, userId.Value);
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
                _logger.LogError(ex, "Error al eliminar post {PostId}", postId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Fija/desfija un post
        /// </summary>
        [HttpPost("{postId}/toggle-pin")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> TogglePin(int communityId, int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var isPinned = await _postService.TogglePinAsync(postId, userId.Value);
                return Ok(new { message = isPinned ? "Post fijado" : "Post desfijado", isPinned });
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
                _logger.LogError(ex, "Error al fijar/desfijar post {PostId}", postId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Bloquea/desbloquea un post
        /// </summary>
        [HttpPost("{postId}/toggle-lock")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ToggleLock(int communityId, int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var isLocked = await _postService.ToggleLockAsync(postId, userId.Value);
                return Ok(new { message = isLocked ? "Post bloqueado" : "Post desbloqueado", isLocked });
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
                _logger.LogError(ex, "Error al bloquear/desbloquear post {PostId}", postId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Da like a un post
        /// </summary>
        [HttpPost("{postId}/like")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LikePost(int communityId, int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var liked = await _postService.LikePostAsync(postId, userId.Value);
                return Ok(new
                {
                    message = liked ? "Like agregado" : "Ya has dado like a este post",
                    liked
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al dar like a post {PostId}", postId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Remueve like de un post
        /// </summary>
        [HttpDelete("{postId}/like")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UnlikePost(int communityId, int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var unliked = await _postService.UnlikePostAsync(postId, userId.Value);
                return Ok(new
                {
                    message = unliked ? "Like removido" : "No habías dado like a este post",
                    unliked
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover like de post {PostId}", postId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}