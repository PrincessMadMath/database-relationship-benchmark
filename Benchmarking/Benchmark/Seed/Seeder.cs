using System.Diagnostics;
using Domain.Document;
using Domain.Relational;
using MongoDB.Driver;

namespace Benchmark.Seed;

public static class Seeder
{
    public const int DefaultOwnerCount = 5;
    public const int DefaultLinkCount = 100;

    public static List<SeedInfo> Seeds = new()
    {
        new SeedInfo(Guid.Parse("f28fa208-5bf9-481c-b65a-38b1fab56b3a"), 10, DefaultOwnerCount, DefaultLinkCount),
        new SeedInfo(Guid.Parse("76482310-c21b-4496-b1ab-170cb7fbb657"), 100, DefaultOwnerCount, DefaultLinkCount),
        new SeedInfo(Guid.Parse("bab74279-bc45-4814-a4a4-0ea71ba654f4"), 1000, DefaultOwnerCount, DefaultLinkCount),
        new SeedInfo(Guid.Parse("d7b2bb60-782e-4405-b44c-1960896f98e9"), 1000, DefaultOwnerCount, 1000),
        new SeedInfo(Guid.Parse("c48d7e55-eddb-4b79-84df-79ae5f3e2259"), 10000, DefaultOwnerCount, DefaultLinkCount),
        // new SeedInfo(Guid.Parse("436af825-d2d5-4eb9-9086-daf5e9488cd9"), 100000, DefaultOwnerCount, DefaultLinkCount)
    };

    public static async Task Seed(
        RelationalRepositoryContext relationalRepositoryContext,
        MongoRepository mongoRepository)
    {
        var sw = Stopwatch.StartNew();

        // await Parallel.ForEachAsync(Seeds, async (seed, token) =>
        //     {
        //         var (groups, users) =
        //             FakeDataGenerator.Generate(
        //                 seed.TenantId,
        //                 seed.GroupCount,
        //                 seed.OwnerCount,
        //                 seed.LinkCount,
        //                 seed.UserCount
        //             );
        //
        //
        //         await Seed(
        //             relationalRepositoryContext,
        //             mongoRepository,
        //             groups,
        //             users);
        //     }
        // );

        foreach (var seed in Seeds)
        {
            var (groups, users) =
                FakeDataGenerator.Generate(
                    seed.TenantId,
                    seed.GroupCount,
                    seed.OwnerCount,
                    seed.LinkCount
                );


            await Seed(
                relationalRepositoryContext,
                mongoRepository,
                groups,
                users);
        }

        Console.WriteLine($"Seeds all in {sw.ElapsedMilliseconds}ms");
    }

    public static async Task Seed(
        RelationalRepositoryContext relationalRepositoryContext,
        MongoRepository mongoRepository,
        List<Group> groups, List<User> users)
    {
        var sw = Stopwatch.StartNew();

        var task1 = SeedRelational(relationalRepositoryContext, groups);
        var task2 = SeedDocument(mongoRepository, groups, users);

        await Task.WhenAll(task1, task2);

        sw.Stop();
        Console.WriteLine(
            $"Seeds {groups.Count} groups with {groups.First().Links.Count} links in {sw.ElapsedMilliseconds}ms");
    }
    
    public static async Task Seed(
        MongoRepository mongoRepository,
        List<Group> groups, List<User> users)
    {
        var sw = Stopwatch.StartNew();

        await SeedDocument(mongoRepository, groups, users);

        sw.Stop();
        Console.WriteLine(
            $"Seeds {groups.Count} groups with {groups.First().Links.Count} links in {sw.ElapsedMilliseconds}ms");
    }

    private static async Task SeedRelational(RelationalRepositoryContext relationalRepositoryContext,
        List<Group> groups)
    {
        var sw = Stopwatch.StartNew();
    
        await Parallel.ForEachAsync(groups.Chunk(250), async (chunkedGroups, token) =>
        {
            await using var relationalRepository = new RelationalRepositoryContext(Configuration.PostgresOptions);
    
            await relationalRepository.Groups.AddRangeAsync(chunkedGroups);
            await relationalRepository.SaveChangesAsync(token);
        });
    
        Console.WriteLine($"    Relational: {sw.ElapsedMilliseconds}ms");
    }

    private static async Task SeedDocument(MongoRepository mongoRepository, List<Group> groups, List<User> users)
    {
        var sw = Stopwatch.StartNew();

        var g = Stopwatch.StartNew();
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
        Console.WriteLine($"    Groups: {g.ElapsedMilliseconds}ms");

        if (users.Any())
        {
            var u = Stopwatch.StartNew();
            var usersInsertModel = users.Select(user =>
                new InsertOneModel<UserDocument>(new UserDocument { UserId = user.UserId, Name = user.Name }));

            await mongoRepository.UserCollection.BulkWriteAsync(usersInsertModel);
            Console.WriteLine($"    Users: {g.ElapsedMilliseconds}ms");
        }

        var l = Stopwatch.StartNew();
        var links = new List<InsertOneModel<LinkDocument>>();

        foreach (var group in groups)
        {
            links.AddRange(group.Links.Select(link =>
                new InsertOneModel<LinkDocument>(new LinkDocument
                {
                    LinkId = link.LinkId, Url = link.Url, GroupId = group.GroupId
                })));
        }
        
        await Parallel.ForEachAsync(links.Chunk(1000), async (models, token) =>
        {
            await mongoRepository.LinkCollection.BulkWriteAsync(models, null, token);
        });
        Console.WriteLine($"    Links: {g.ElapsedMilliseconds}ms");


        Console.WriteLine($"    Document: {sw.ElapsedMilliseconds}ms");
    }
}
