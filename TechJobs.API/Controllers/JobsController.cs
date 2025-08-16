using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechJobs.Application.DTOs;
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

    // Public: approved jobs with paging/sorting
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetApproved(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortDir = "desc"
    )
    {
        var paged = await _jobService.GetApprovedJobsPagedAsync(new PageRequest(page, pageSize), sortBy, sortDir);
        var dto = paged.Items.Select(j => j.ToDto()).ToList();
        var result = new PagedResult<object>(dto, paged.Page, paged.PageSize, paged.TotalCount);
        return Ok(result);
    }

    public record CreateJobDto(string Title, string? Role, string? Description, string? Location, int EmployerId, int? MinExperienceYears, List<int> TechStackIds);

    // Employer only: create job
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
        return CreatedAtAction(nameof(GetApproved), new { id = created.Id }, new { created.Id, created.Title });
    }

    // Public: search with paging/sorting
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? techStack,
        [FromQuery] string? location,
        [FromQuery] int? minExp,
        [FromQuery] string? role,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortDir = "desc"
    )
    {
        var paged = await _jobService.SearchPagedAsync(techStack, location, minExp, role, new PageRequest(page, pageSize), sortBy, sortDir);
        var dto = paged.Items.Select(j => j.ToDto()).ToList();
        var result = new PagedResult<object>(dto, paged.Page, paged.PageSize, paged.TotalCount);
        return Ok(result);
    }

    // Admin only: approve a job
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var ok = await _jobService.ApproveJobAsync(id);
        return ok ? NoContent() : NotFound();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? sortBy = "createdAt",
    [FromQuery] string? sortDir = "desc")
    {
        var all = await _jobService.GetPendingJobsAsync();
        // reuse JobService paging helpers by mapping through SearchPagedAsync-like behavior
        // quick inline: sort + page here
        bool desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
        IEnumerable<TechJobs.Domain.Entities.Job> Sort(IEnumerable<TechJobs.Domain.Entities.Job> jobs)
            => (sortBy ?? "").ToLowerInvariant() switch
            {
                "title" => desc ? jobs.OrderByDescending(j => j.Title) : jobs.OrderBy(j => j.Title),
                "location" => desc ? jobs.OrderByDescending(j => j.Location) : jobs.OrderBy(j => j.Location),
                "experience" or "minexperienceyears" =>
                    desc ? jobs.OrderByDescending(j => j.MinExperienceYears ?? int.MaxValue)
                         : jobs.OrderBy(j => j.MinExperienceYears ?? int.MaxValue),
                "employer" =>
                    desc ? jobs.OrderByDescending(j => j.Employer.FullName)
                         : jobs.OrderBy(j => j.Employer.FullName),
                _ => desc ? jobs.OrderByDescending(j => j.CreatedAtUtc)
                          : jobs.OrderBy(j => j.CreatedAtUtc),
            };
        var sorted = Sort(all);
        var total = sorted.Count();
        var items = sorted.Skip((page - 1) * Math.Clamp(pageSize, 1, 100)).Take(Math.Clamp(pageSize, 1, 100)).ToList();
        var dto = items.Select(j => j.ToDto()).ToList();
        return Ok(new { items = dto, page, pageSize, totalCount = total, totalPages = (int)Math.Ceiling((double)total / Math.Clamp(pageSize, 1, 100)) });
    }

}
