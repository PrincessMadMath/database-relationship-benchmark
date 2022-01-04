using System.Diagnostics;
using Domain.Relational;

namespace Benchmark;

public static class Seeder
{
    public const int UserCount = 100;
    public const int OwnerCount = 3;

    public static List<SeedInfo> Seeds = new()
    {
        new(10, Guid.Parse("f28fa208-5bf9-481c-b65a-38b1fab56b3a")),
        new(100, Guid.Parse("76482310-c21b-4496-b1ab-170cb7fbb657")),
        new(1000, Guid.Parse("bab74279-bc45-4814-a4a4-0ea71ba654f4")),
        new(10000, Guid.Parse("c48d7e55-eddb-4b79-84df-79ae5f3e2259"))
        // new SeedInfo(100000, Guid.Parse("436af825-d2d5-4eb9-9086-daf5e9488cd9")),
    };

    public static async Task Seed(RepositoryContext repositoryContext)
    {
        var sw = new Stopwatch();

        foreach (var seed in Seeds)
        {
            sw.Restart();
            await repositoryContext.Groups.AddRangeAsync(
                FakeDataGenerator.GenerateGroups(seed.TenantId, seed.GroupCount, UserCount, OwnerCount));
            sw.Stop();
            Console.WriteLine($"Create {seed.GroupCount} groups in {sw.ElapsedMilliseconds}ms");
        }

        sw.Restart();
        await repositoryContext.SaveChangesAsync();
        sw.Stop();
        Console.WriteLine($"Saves changes in {sw.ElapsedMilliseconds}ms");
    }

    public class SeedInfo
    {
        public SeedInfo(int groupCount, Guid tenantId)
        {
            this.TenantId = tenantId;
            this.GroupCount = groupCount;
        }

        public Guid TenantId { get; }

        public int GroupCount { get; }

        public override string ToString()
        {
            return $"{this.GroupCount} groups";
        }
    }
}
