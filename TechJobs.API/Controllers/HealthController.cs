using Microsoft.AspNetCore.Mvc;

namespace TechJobs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok", tsUtc = DateTime.UtcNow });
}
