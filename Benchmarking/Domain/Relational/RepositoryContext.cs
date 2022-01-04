using Microsoft.EntityFrameworkCore;

namespace Domain.Relational;

public class RepositoryContext : DbContext
{
    public RepositoryContext(DbContextOptions<RepositoryContext> options)
        : base(options)
    {
    }

    public DbSet<Group> Groups { get; set; }
    public DbSet<User> Users { get; set; }
}
