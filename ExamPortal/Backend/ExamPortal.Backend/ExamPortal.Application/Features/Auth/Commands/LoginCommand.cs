using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExamPortal.Application.DTOs.User;
using ExamPortal.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ExamPortal.Application.Features.Auth.Commands;

public record LoginCommand(LoginRequestDto Payload) : IRequest<AuthResponseDto>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IExamPortalDbContext _context;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(IExamPortalDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate User
        var user = await _context.Users
           .AsNoTracking()
           .FirstOrDefaultAsync(u => u.Email == request.Payload.Email, cancellationToken);

        // NOTE: In production, verify the hashed password here using BCrypt or Argon2.
        // For this dummy step, we are doing a direct string match.
        if (user == null || user.PasswordHash != request.Payload.Password)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        // 2. Generate Claims (Including Multi-Tenant ID)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("TenantId", user.TenantId.ToString())
        };

        // 3. Create the JWT
        var secretKey = _configuration.GetSection("JwtSettings:Secret").Value;
        var issuer = _configuration.GetSection("JwtSettings:Issuer").Value;
        var audience = _configuration.GetSection("JwtSettings:Audience").Value;
        var expiryMinutesStr = _configuration.GetSection("JwtSettings:ExpiryMinutes").Value;

        var expiryMinutes = Convert.ToDouble(expiryMinutesStr);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto(tokenString, user.Id, user.Role);
    }
}