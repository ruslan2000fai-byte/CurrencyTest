using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Users.FindAsync(new object[] { id }, ct);

    public async Task<User?> GetByNameAsync(string name, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.Name == name, ct);

    public async Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);
        return user;
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        => await _context.Users.AnyAsync(u => u.Name == name, ct);
}
