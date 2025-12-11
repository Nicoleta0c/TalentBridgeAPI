using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentBridge.Application.DTOs;
using TalentBridge.Application.DTOs.CVDTOs;
using TalentBridge.Application.Interfaces;
using TalentBridge.Application.Validations;

namespace TalentBridge.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CVsController : ControllerBase
    {
        private readonly ICVService _cvService;
        private readonly IValidator<UploadCVDto> _uploadCVValidator;
        private readonly IValidator<UpdateCVDto> _updateCVValidator;
        private readonly IValidator<AnalyzeCVRequestDto> _analyzeCVValidator;

        public CVsController(
            ICVService cvService,
            IValidator<UploadCVDto> uploadCVValidator,
            IValidator<UpdateCVDto> updateCVValidator,
            IValidator<AnalyzeCVRequestDto> analyzeCVValidator)
        {
            _cvService = cvService;
            _uploadCVValidator = uploadCVValidator;
            _updateCVValidator = updateCVValidator;
            _analyzeCVValidator = analyzeCVValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCVs([FromQuery] int? userId = null)
        {
            try
            {
                var cvs = userId.HasValue ?
                    await _cvService.GetCVsByUserIdAsync(userId.Value) :
                    await _cvService.GetAllCVsAsync();

                return Ok(new
                {
                    success = true,
                    data = cvs,
                    count = cvs.Count(),
                    filters = new { userId }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving CVs",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCVById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid CV ID. ID must be greater than 0"
                });
            }

            try
            {
                var cv = await _cvService.GetCVByIdAsync(id);
                return cv != null ? Ok(new { success = true, data = cv }) :
                       NotFound(new { success = false, message = "CV not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving the CV",
                    error = ex.Message
                });
            }
        }

        [HttpGet("user/{userId}/active")]
        public async Task<IActionResult> GetActiveCV(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid user ID. ID must be greater than 0"
                });
            }

            try
            {
                var cv = await _cvService.GetActiveCVByUserIdAsync(userId);
                return cv != null ? Ok(new { success = true, data = cv }) :
                       NotFound(new { success = false, message = "No active CV found for this user" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving the active CV",
                    error = ex.Message
                });
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCV([FromForm] UploadCVDto uploadCVDto)
        {
            var validationResult = await _uploadCVValidator.ValidateAsync(uploadCVDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            try
            {
                var cv = await _cvService.UploadCVAsync(uploadCVDto);
                return CreatedAtAction(nameof(GetCVById), new { id = cv.Id },
                    new { success = true, data = cv, message = "CV uploaded successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while uploading the CV",
                    error = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCV(int id, [FromBody] UpdateCVDto updateCVDto)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid CV ID. ID must be greater than 0"
                });
            }

            var validationResult = await _updateCVValidator.ValidateAsync(updateCVDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            try
            {
                var result = await _cvService.UpdateCVAsync(id, updateCVDto);
                return result ? Ok(new { success = true, message = "CV updated successfully" }) :
                       NotFound(new { success = false, message = "CV not found or you don't have permission to update it" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while updating the CV",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCV(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid CV ID. ID must be greater than 0"
                });
            }

            try
            {
                var result = await _cvService.DeleteCVAsync(id);
                return result ? Ok(new { success = true, message = "CV deleted successfully" }) :
                       NotFound(new { success = false, message = "CV not found" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while deleting the CV",
                    error = ex.Message
                });
            }
        }

        [HttpPost("{id}/analyze")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> AnalyzeCV(int id, [FromBody] AnalyzeCVRequestDto analyzeRequest)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid CV ID. ID must be greater than 0"
                });
            }

            // Asigna el ID de la ruta al DTO
            analyzeRequest.CVId = id;

            var validationResult = await _analyzeCVValidator.ValidateAsync(analyzeRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            try
            {
                var analysisResult = await _cvService.AnalyzeCVAsync(analyzeRequest);
                return Ok(new { success = true, data = analysisResult });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while analyzing the CV",
                    error = ex.Message
                });
            }
        }
    }
}