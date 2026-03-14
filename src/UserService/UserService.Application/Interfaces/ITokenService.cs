using UserService.Domain.Entities;

namespace UserService.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<bool> IsTokenRevokedAsync(string jti, CancellationToken cancellationToken = default);
}
