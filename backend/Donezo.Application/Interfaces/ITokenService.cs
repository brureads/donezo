using Donezo.Domain.Entities;

namespace Donezo.Application.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}