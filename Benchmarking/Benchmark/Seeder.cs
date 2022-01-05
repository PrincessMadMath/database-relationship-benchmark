using System.Diagnostics;
using Domain.Document;
using Domain.Relational;
using MongoDB.Driver;

namespace Benchmark;

public static class Seeder
{
    public const int UserCount = 5000;
    public const int OwnerCount = 3;

    public static List<SeedInfo> Seeds = new()
    {
        new(10, Guid.Parse("f28fa208-5bf9-481c-b65a-38b1fab56b3a")),
        new(100, Guid.Parse("76482310-c21b-4496-b1ab-170cb7fbb657")),
        new(1000, Guid.Parse("bab74279-bc45-4814-a4a4-0ea71ba654f4")),
        new SeedInfo(10000, Guid.Parse("c48d7e55-eddb-4b79-84df-79ae5f3e2259"))
        // new SeedInfo(100000, Guid.Parse("436af825-d2d5-4eb9-9086-daf5e9488cd9")),
    };

    public static async Task Seed(RelationalRepositoryContext relationalRepositoryContext,
        MongoRepository mongoRepository)
    {
        var sw = new Stopwatch();

        foreach (var seed in Seeds)
        {
            sw.Restart();

            var (groups, users) =
                FakeDataGenerator.GenerateGroups(seed.TenantId, seed.GroupCount, UserCount, OwnerCount);
            await SeedRelational(relationalRepositoryContext, groups);
            await SeedDocument(mongoRepository, groups, users);

            sw.Stop();
            Console.WriteLine($"Create {seed.GroupCount} groups in {sw.ElapsedMilliseconds}ms");
        }
    }

    private static async Task SeedRelational(RelationalRepositoryContext relationalRepositoryContext,
        List<Group> groups)
    {
        await relationalRepositoryContext.Groups.AddRangeAsync(groups);
        await relationalRepositoryContext.SaveChangesAsync();
    }

    private static async Task SeedDocument(MongoRepository mongoRepository, List<Group> groups, List<User> users)
    {
        var groupsInsertModel = groups.Select(group =>
        {
            return new InsertOneModel<GroupDocument>(new GroupDocument
            {
                TenantId = group.TenantId,
                GroupId = group.GroupId,
                Name = group.Name,
                OwnersId = group.Owners.Select(x => x.UserId).ToList()
            });
        });

        await mongoRepository.GroupCollection.BulkWriteAsync(groupsInsertModel);

        var usersInsertModel = users.Select(user =>
            new InsertOneModel<UserDocument>(new UserDocument { UserId = user.UserId, Name = user.Name }));

        await mongoRepository.UserCollection.BulkWriteAsync(usersInsertModel);
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
