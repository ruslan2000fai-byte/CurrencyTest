using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppReadDbContext _readContext;
    private readonly AppWriteDbContext _writeContext;

    public UserRepository(AppReadDbContext readContext, AppWriteDbContext writeContext)
    {
        _readContext = readContext;
        _writeContext = writeContext;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _readContext.Users.FindAsync(new object[] { id }, ct);

    public async Task<User?> GetByNameAsync(string name, CancellationToken ct = default)
        => await _readContext.Users.FirstOrDefaultAsync(u => u.Name == name, ct);

    public async Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        _writeContext.Users.Add(user);
        await _writeContext.SaveChangesAsync(ct);
        return user;
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        => await _readContext.Users.AnyAsync(u => u.Name == name, ct);
}
