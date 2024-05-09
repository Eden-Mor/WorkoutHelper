using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkoutHelper.Shared.Models;
using WorkoutHelperAPI.Data;
using WorkoutHelperAPI.Helper;
using Encryption = BCrypt.Net.BCrypt;
using WorkoutHelper.Shared.Enums;
using WorkoutHelper.Shared.JWT;

namespace WorkoutHelperAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private IConfiguration _config;

    private readonly AppDBContext _context;

    public LoginController(IConfiguration config, AppDBContext context)
    {
        _config = config;
        _context = context;
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginData loginRequest)
    {
        var user = _context.Users.SingleOrDefault(x => x.Username!.ToLower() == loginRequest.Username!.ToLower());

        if (user == null || !Encryption.EnhancedVerify(loginRequest.PasswordHash, user.PasswordHash, BCrypt.Net.HashType.SHA384) || !user.Id.HasValue)
            return NotFound();

        var token = GenerateToken(user.Id.Value, user.Role);
        return Ok(token);
    }

    [HttpPost("Register")]
    public IActionResult Register([FromBody] LoginData loginRequest)
    {
       if (_context.Users.Any(x=> x.Username!.ToLower() == loginRequest.Username!.ToLower() || string.IsNullOrEmpty(loginRequest.PasswordHash) || loginRequest.PasswordHash.Length < 7))
            return BadRequest();

        var salt = Encryption.GenerateSalt(13);
        DatabaseLoginData userData = new()
        {
            Username = loginRequest.Username!.ToLower(),
            PasswordHash = Encryption.HashPassword(loginRequest.PasswordHash, salt, enhancedEntropy: true),
            Salt = salt
        };

        var user = _context.Users.Add(userData);
        _context.SaveChanges();

        var id = user.Entity.Id;

        if (!id.HasValue)
            return NotFound();

        var token = GenerateToken(id.Value);
        return Ok(token);
    }

    protected string GenerateToken(int id, RolesEnum role = RolesEnum.LoggedInUser)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DockerSecretHelper.ReadSecret("jwt-key")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, id.ToString()),
            new(JwtCustomClaims.Role, role.ToString()),
        ];

        var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, expires: DateTime.Now.AddMinutes(120), signingCredentials: credentials));
        return token;
    }
}
