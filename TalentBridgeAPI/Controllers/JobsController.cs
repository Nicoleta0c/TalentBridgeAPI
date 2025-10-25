using Microsoft.AspNetCore.Mvc;
using TalentBridge.Application.DTOs;
using TalentBridge.Application.Interfaces;

namespace TalentBridge.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return Ok(jobs);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveJobs()
        {
            var jobs = await _jobService.GetActiveJobsAsync();
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            return job != null ? Ok(job) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobDto createJobDto)
        {
            try
            {
                var job = await _jobService.CreateJobAsync(createJobDto);
                return CreatedAtAction(nameof(GetJobById), new { id = job.Id }, job);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] CreateJobDto updateJobDto)
        {
            var job = await _jobService.UpdateJobAsync(id, updateJobDto);
            return job != null ? Ok(job) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var result = await _jobService.DeleteJobAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyToJob([FromBody] ApplyToJobRequest request)
        {
            try
            {
                // TODO: Obtener userId del token JWT (por ahora lo pasamos en el request)
                var application = await _jobService.ApplyToJobAsync(request.UserId, request.ApplyDto);
                return Ok(application);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("applications/user/{userId}")]
        public async Task<IActionResult> GetUserApplications(int userId)
        {
            var applications = await _jobService.GetUserApplicationsAsync(userId);
            return Ok(applications);
        }
    }

    public record ApplyToJobRequest(int UserId, ApplyToJobDto ApplyDto);
}