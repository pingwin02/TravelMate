using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TravelMateAuthService.Services;

namespace TravelMateAuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration config, IUserService userService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Credentials credentials)
    {
        var guid = await userService.CheckLoginCredentials(credentials);
        if (guid == null) return Unauthorized();

        var token = GenerateJwtToken(credentials.Username, (Guid)guid);

        return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Credentials credentials)
    {
        try
        {
            await userService.RegisterUser(credentials);
            return Ok();
        }
        catch (DuplicateNameException)
        {
            return Conflict("Username already exists");
        }
    }

    private string GenerateJwtToken(string username, Guid guid)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, guid.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create the token
        var token = new JwtSecurityToken(
            config["Jwt:Issuer"],
            config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class Credentials
{
    public string Username { get; set; }
    public string Password { get; set; }
}