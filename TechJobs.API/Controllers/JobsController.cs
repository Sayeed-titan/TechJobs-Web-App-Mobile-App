using Microsoft.AspNetCore.Mvc;
using TechJobs.Application.Interfaces.Services;
using TechJobs.Domain.Entities;

namespace TechJobs.API.Controllers;

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
    public async Task<IActionResult> GetApproved()
    {
        var jobs = await _jobService.GetApprovedJobsAsync();
        return Ok(jobs);
    }

    public record CreateJobDto(string Title, string? Role, string? Description, string? Location, int EmployerId, int? MinExperienceYears, List<int> TechStackIds);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobDto dto)
    {
        var job = new Job
        {
            Title = dto.Title,
            Role = dto.Role,
            Description = dto.Description,
            Location = dto.Location,
            EmployerId = dto.EmployerId,
            MinExperienceYears = dto.MinExperienceYears
        };

        var created = await _jobService.CreateJobAsync(job, dto.TechStackIds ?? new List<int>());
        return CreatedAtAction(nameof(GetApproved), new { id = created.Id }, created);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? techStack, [FromQuery] string? location, [FromQuery] int? minExp, [FromQuery] string? role)
    {
        var results = await _jobService.SearchAsync(techStack, location, minExp, role);
        return Ok(results);
    }
}
