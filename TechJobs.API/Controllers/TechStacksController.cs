using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechJobs.Application.Interfaces;
using TechJobs.Application.Interfaces.Repositories;
using TechJobs.Domain.Entities;

namespace TechJobs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TechStacksController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public TechStacksController(IUnitOfWork uow) => _uow = uow;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stacks = await _uow.TechStacks.GetAllAsync();
        return Ok(stacks.Select(s => new { s.Id, s.Name }));
    }
}
