using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Donezo.Application.Interfaces;
using Donezo.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Donezo.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly string _secretKey;

    public TokenService(string secretKey)
    {
        if (Encoding.UTF8.GetByteCount(secretKey) < 32)
            throw new ArgumentException(
                "JWT key must have at least 32 bytes.");

        _secretKey = secretKey;
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddHours(2);

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_secretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "Donezo",
            audience: "DonezoClient",
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt);
    }
}