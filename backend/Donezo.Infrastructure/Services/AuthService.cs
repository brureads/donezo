using Donezo.Application.DTOs.Auth;
using Donezo.Application.Interfaces;
using Donezo.Domain.Entities;
using Donezo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Donezo.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly DonezoDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public AuthService(
        DonezoDbContext context,
        IPasswordService passwordService,
        ITokenService tokenService)
    {
        _context = context;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await _context.Users
            .AnyAsync(u => u.Email == email);

        if (exists)
            throw new InvalidOperationException(
                "E-mail já cadastrado.");

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = _passwordService
                .HashPassword(request.Password)
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return CreateResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user is null ||
            !_passwordService.VerifyPassword(
                request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException(
                "E-mail ou senha inválidos.");
        }

        return CreateResponse(user);
    }

    private AuthResponse CreateResponse(User user)
    {
        var (token, expiresAt) =
            _tokenService.GenerateToken(user);

        return new AuthResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Token = token,
            ExpiresAt = expiresAt
        };
    }
}