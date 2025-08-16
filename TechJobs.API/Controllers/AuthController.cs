using Microsoft.AspNetCore.Mvc;
using TechJobs.Application.DTOs;
using TechJobs.Application.Interfaces.Services;

namespace TechJobs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        try
        {
            var res = await _auth.RegisterAsync(req);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var res = await _auth.LoginAsync(req);
        if (res is null) return Unauthorized(new { error = "Invalid credentials" });
        return Ok(res);
    }
}
