using Microsoft.AspNetCore.Mvc;
using TalentBridge.Application.DTOs;
using TalentBridge.Application.Interfaces;

namespace TalentBridge.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CVsController : ControllerBase
    {
        private readonly ICVService _cvService;

        public CVsController(ICVService cvService)
        {
            _cvService = cvService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserCVs(int userId)
        {
            var cvs = await _cvService.GetUserCVsAsync(userId);
            return Ok(cvs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCVById(int id)
        {
            var cv = await _cvService.GetCVByIdAsync(id);
            return cv != null ? Ok(cv) : NotFound();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCV([FromBody] UploadCVDto uploadCVDto)
        {
            try
            {
                var cv = await _cvService.UploadCVAsync(uploadCVDto);
                return CreatedAtAction(nameof(GetCVById), new { id = cv.Id }, cv);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCV(int id, [FromBody] UploadCVDto updateCVDto)
        {
            var cv = await _cvService.UpdateCVAsync(id, updateCVDto);
            return cv != null ? Ok(cv) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCV(int id)
        {
            var result = await _cvService.DeleteCVAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeCV([FromBody] AnalyzeCVRequestDto analyzeRequest)
        {
            try
            {
                var result = await _cvService.AnalyzeCVAsync(analyzeRequest);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}