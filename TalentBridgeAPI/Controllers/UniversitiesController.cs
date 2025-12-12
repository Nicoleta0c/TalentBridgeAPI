using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TalentBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversitiesController : ControllerBase
    {
        private readonly IUniversityService _universityService;
        private readonly ILogger<UniversitiesController> _logger;

        public UniversitiesController(
            IUniversityService universityService,
            ILogger<UniversitiesController> logger)
        {
            _universityService = universityService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las universidades
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UniversityDto>>> GetAll()
        {
            try
            {
                var universities = await _universityService.GetAllAsync();
                return Ok(universities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las universidades");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene solo las universidades activas
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UniversityDto>>> GetActiveUniversities()
        {
            try
            {
                var universities = await _universityService.GetActiveUniversitiesAsync();
                return Ok(universities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener universidades activas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene una universidad por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UniversityDto>> GetById(int id)
        {
            try
            {
                var university = await _universityService.GetByIdAsync(id);

                if (university == null)
                {
                    return NotFound(new { message = $"Universidad con ID {id} no encontrada" });
                }

                return Ok(university);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener universidad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene detalles completos de una universidad (con carreras y comunidades)
        /// </summary>
        [HttpGet("{id}/details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UniversityDetailDto>> GetDetailById(int id)
        {
            try
            {
                var university = await _universityService.GetDetailByIdAsync(id);

                if (university == null)
                {
                    return NotFound(new { message = $"Universidad con ID {id} no encontrada" });
                }

                return Ok(university);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de universidad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea una nueva universidad
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UniversityDto>> Create([FromBody] CreateUniversityDto dto)
        {
            try
            {
                var university = await _universityService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = university.Id }, university);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear universidad");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza una universidad existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UniversityDto>> Update(int id, [FromBody] UpdateUniversityDto dto)
        {
            try
            {
                var university = await _universityService.UpdateAsync(id, dto);
                return Ok(university);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar universidad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina una universidad
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _universityService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar universidad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Verifica una universidad
        /// </summary>
        [HttpPatch("{id}/verify")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Verify(int id)
        {
            try
            {
                await _universityService.VerifyUniversityAsync(id);
                return Ok(new { message = "Universidad verificada exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar universidad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene todas las carreras de una universidad
        /// </summary>
        [HttpGet("{id}/careers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<UniversityCareerDto>>> GetCareersByUniversity(int id)
        {
            try
            {
                var careers = await _universityService.GetCareersByUniversityAsync(id);
                return Ok(careers);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener carreras de universidad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea una nueva carrera para una universidad
        /// </summary>
        [HttpPost("careers")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UniversityCareerDto>> CreateCareer([FromBody] CreateCareerDto dto)
        {
            try
            {
                var career = await _universityService.CreateCareerAsync(dto);
                return CreatedAtAction(nameof(GetCareersByUniversity), new { id = dto.UniversityId }, career);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear carrera");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}