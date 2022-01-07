using System.Diagnostics;
using Domain.Document;
using Domain.Relational;
using MongoDB.Driver;

namespace Benchmark.Seed;

public static class Seeder
{
    public const int DefaultUserCount = 500;
    public const int DefaultOwnerCount = 3;
    public const int DefaultLinkCount = 1000;

    public static List<SeedInfo> Seeds = new()
    {
        new(Guid.Parse("f28fa208-5bf9-481c-b65a-38b1fab56b3a"), 10, DefaultUserCount, DefaultOwnerCount, DefaultLinkCount),
        new(Guid.Parse("76482310-c21b-4496-b1ab-170cb7fbb657"), 100, DefaultUserCount, DefaultOwnerCount, DefaultLinkCount),
        new(Guid.Parse("bab74279-bc45-4814-a4a4-0ea71ba654f4"), 1000, DefaultUserCount, DefaultOwnerCount, DefaultLinkCount),
        new SeedInfo(Guid.Parse("c48d7e55-eddb-4b79-84df-79ae5f3e2259"), 10000, DefaultUserCount, DefaultOwnerCount, 10),
        new SeedInfo(Guid.Parse("c48d7e55-eddb-4b79-84df-79ae5f3e2259"), 10000, DefaultUserCount, DefaultOwnerCount, 1000),
        //new SeedInfo(Guid.Parse("436af825-d2d5-4eb9-9086-daf5e9488cd9"), 100000, DefaultUserCount, DefaultOwnerCount, DefaultLinkCount),
    };

    public static async Task Seed(
        RelationalRepositoryContext relationalRepositoryContext,
        MongoRepository mongoRepository)
    {
        var sw = new Stopwatch();

        foreach (var seed in Seeds)
        {
            sw.Restart();

            var (groups, users) =
                FakeDataGenerator.GenerateGroups(seed);

            // Parallalize
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

        // Chunk
        foreach (var group in groups)
        {
            var linksInsertModel = group.Links.Select(link =>
                new InsertOneModel<LinkDocument>(new LinkDocument { LinkId = link.LinkId, Url = link.Url, GroupId = group.GroupId}));

            await mongoRepository.LinkCollection.BulkWriteAsync(linksInsertModel);
        }
    }
}
