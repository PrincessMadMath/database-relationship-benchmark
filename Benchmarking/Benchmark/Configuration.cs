using Domain.Relational;
using Microsoft.EntityFrameworkCore;

namespace Benchmark;

public static class Configuration
{
    public static DbContextOptions<RelationalRepositoryContext> PostgresOptions =
        new DbContextOptionsBuilder<RelationalRepositoryContext>()
            .UseNpgsql("Host=localhost:5432;Database=benchmark;Username=dbuser;Password=admin1234")
            .Options;
}
