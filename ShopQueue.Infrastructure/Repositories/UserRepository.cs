using Microsoft.EntityFrameworkCore;
using ShopQueue.Application.Repositories;
using ShopQueue.Domain.Entities;
using ShopQueue.Infrastructure.Persistence;

namespace ShopQueue.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await db.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return db.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await db.Users.AddAsync(user, cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        db.Users.Update(user);
        return Task.CompletedTask;
    }
}