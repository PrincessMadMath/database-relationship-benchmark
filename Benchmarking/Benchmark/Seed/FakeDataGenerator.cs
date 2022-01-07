using System.Diagnostics;
using Domain.Relational;

namespace Benchmark.Seed;

public static class FakeDataGenerator
{
    public static (List<Group> Groups, List<User> Users) Generate(
        Guid tenantId,
        int groupCount,
        int ownerCount,
        int linkCount)
    {
        var sw = Stopwatch.StartNew();
        var (groups, users) = DoGenerate(tenantId, groupCount, ownerCount, linkCount);

        Console.WriteLine($"Generate {groupCount} groups with {linkCount} links in {sw.ElapsedMilliseconds}ms");

        return (groups, users);
    }

    private static (List<Group> Groups, List<User> Users) DoGenerate(
        Guid tenantId,
        int groupCount,
        int ownerCount,
        int linkCount)
    {
        var users = new List<User>();

        var groups = Enumerable.Range(0, groupCount).Select(x =>
        {
            var owners = Enumerable.Range(0, ownerCount).Select(_ => new User { UserId = Guid.NewGuid(), Name = "Test Name" })
                .ToList();

            users.AddRange(owners);

            var links = Enumerable.Range(0, linkCount).Select(x => new Link
            {
                LinkId = Guid.NewGuid(), Url = "www.localhost.com"
            }).ToList();


            return new Group
            {
                TenantId = tenantId,
                Name = "Company",
                GroupId = Guid.NewGuid(),
                Owners = owners,
                Links = links
            };
        });

        return (groups.ToList(), users);
    }
}
