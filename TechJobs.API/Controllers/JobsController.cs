using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechJobs.Application.Interfaces.Services;
using TechJobs.Application.Mappers;
using TechJobs.Domain.Entities;

namespace TechJobs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    public JobsController(IJobService jobService) => _jobService = jobService;

    // Public
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetApproved()
    {
        var jobs = await _jobService.GetApprovedJobsAsync();
        var result = jobs.Select(j => j.ToDto());
        return Ok(result);
    }

    public record CreateJobDto(string Title, string? Role, string? Description, string? Location, int EmployerId, int? MinExperienceYears, List<int> TechStackIds);

    // Employer only
    [Authorize(Roles = "Employer")]
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

        // return the DTO back (optional includes not needed for creation response)
        return CreatedAtAction(nameof(GetApproved), new { id = created.Id }, new { created.Id, created.Title });
    }

    // Public search
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? techStack, [FromQuery] string? location, [FromQuery] int? minExp, [FromQuery] string? role)
    {
        var results = await _jobService.SearchAsync(techStack, location, minExp, role);
        var dto = results.Select(j => j.ToDto());
        return Ok(dto);
    }

    // Admin only
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var ok = await _jobService.ApproveJobAsync(id);
        return ok ? NoContent() : NotFound();
    }

}
