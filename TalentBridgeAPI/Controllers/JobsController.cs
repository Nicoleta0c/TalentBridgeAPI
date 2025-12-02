using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentBridge.Application.DTOs;
using TalentBridge.Application.DTOs.JobDTOs;
using TalentBridge.Application.Interfaces;
using TalentBridge.Application.Validations;

namespace TalentBridge.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IValidator<CreateJobDto> _createJobValidator;
        private readonly IValidator<UpdateJobDto> _updateJobValidator;

        public JobsController(
            IJobService jobService,
            IValidator<CreateJobDto> createJobValidator,
            IValidator<UpdateJobDto> updateJobValidator)
        {
            _jobService = jobService;
            _createJobValidator = createJobValidator;
            _updateJobValidator = updateJobValidator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllJobs([FromQuery] bool activeOnly = true)
        {
            try
            {
                var jobs = await _jobService.GetAllJobsAsync(activeOnly);
                return Ok(new
                {
                    success = true,
                    data = jobs,
                    count = jobs.Count(),
                    filters = new { activeOnly }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving jobs",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetJobById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid job ID. ID must be greater than 0"
                });
            }

            try
            {
                var job = await _jobService.GetJobByIdAsync(id);
                return job != null ? Ok(new { success = true, data = job }) :
                       NotFound(new { success = false, message = "Job not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving the job",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobDto createJobDto)
        {
            var validationResult = await _createJobValidator.ValidateAsync(createJobDto);
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
                var job = await _jobService.CreateJobAsync(createJobDto);
                return CreatedAtAction(nameof(GetJobById), new { id = job.Id },
                    new { success = true, data = job, message = "Job created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while creating the job",
                    error = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] UpdateJobDto updateJobDto)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid job ID. ID must be greater than 0"
                });
            }

            var validationResult = await _updateJobValidator.ValidateAsync(updateJobDto);
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
                var result = await _jobService.UpdateJobAsync(id, updateJobDto);
                return result ? Ok(new { success = true, message = "Job updated successfully" }) :
                       NotFound(new { success = false, message = "Job not found or you don't have permission to update it" });
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
                    message = "An error occurred while updating the job",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid job ID. ID must be greater than 0"
                });
            }

            try
            {
                var result = await _jobService.DeleteJobAsync(id);
                return result ? Ok(new { success = true, message = "Job deleted successfully" }) :
                       NotFound(new { success = false, message = "Job not found" });
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
                    message = "An error occurred while deleting the job",
                    error = ex.Message
                });
            }
        }
    }
}