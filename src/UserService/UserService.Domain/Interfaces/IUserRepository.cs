using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<User?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<User> CreateAsync(User user, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
}
