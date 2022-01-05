using Microsoft.EntityFrameworkCore;

namespace Domain.Relational;

public class RelationalRepositoryContext : DbContext
{
    public RelationalRepositoryContext(DbContextOptions<RelationalRepositoryContext> options)
        : base(options)
    {
    }

    public DbSet<Group> Groups { get; set; }
    public DbSet<User> Users { get; set; }
}
