using Microsoft.EntityFrameworkCore;
using ShopQueue.Domain.Entities;

namespace ShopQueue.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Queue> Queues => Set<Queue>();
    public DbSet<QueueEntry> QueueEntries => Set<QueueEntry>();
    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<User> Users => Set<User>();
}