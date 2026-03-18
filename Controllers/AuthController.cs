using BCrypt.Net;
using Disputes.Api.Application.Auth;
using Disputes.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disputes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public AuthController(AppDbContext db, JwtTokenGenerator jwtTokenGenerator)
    {
        _db = db;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Email and password are required.");

        var user = await _db.Users.FirstOrDefaultAsync(
            x => x.Email == request.Email,
            ct);

        if (user is null)
            return Unauthorized("Invalid email or password.");

        var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!passwordValid)
            return Unauthorized("Invalid email or password.");

        var token = _jwtTokenGenerator.Generate(user);

        return Ok(new LoginResponse(
            token,
            new UserDto(user.Id, user.Email, user.DisplayName, user.Role)
        ));
    }
}