using Domain.Relational;
using Faker;

namespace Benchmark.Seed;

public static class FakeDataGenerator
{
    public static (List<Group> Groups, List<User> Users) GenerateGroups(SeedInfo seedInfo)
    {
        var random = new Random();

        var users = Enumerable.Range(0, seedInfo.UserCount).Select(x => new User
        {
            UserId = Guid.NewGuid(), Name = Name.FullName()
        }).ToList();

        var groups = Enumerable.Range(0, seedInfo.GroupCount).Select(x =>
        {
            var owners = users.Skip(random.Next( seedInfo.UserCount - seedInfo.OwnerCount)).Take(seedInfo.OwnerCount).ToList();

            var links = Enumerable.Range(0, seedInfo.LinkCount).Select(x => new Link
            {
                LinkId = Guid.NewGuid(), Url = Internet.Url()
            }).ToList();


            return new Group { TenantId = seedInfo.TenantId, Name = Company.Name(), GroupId = Guid.NewGuid(), Owners = owners, Links = links};
        });

        return (groups.ToList(), users);
    }

    public static (List<Group> Groups, List<User> Users) GenerateWithConstString(SeedInfo seedInfo)
    {
        var random = new Random();

        var users = Enumerable.Range(0, seedInfo.UserCount).Select(x => new User
        {
            UserId = Guid.NewGuid(), Name = "Test Name"
        }).ToList();

        var groups = Enumerable.Range(0, seedInfo.GroupCount).Select(x =>
        {
            var owners = users.Skip(random.Next( seedInfo.UserCount - seedInfo.OwnerCount)).Take(seedInfo.OwnerCount).ToList();

            var links = Enumerable.Range(0, seedInfo.LinkCount).Select(x => new Link
            {
                LinkId = Guid.NewGuid(), Url = "www.localhost.com"
            }).ToList();


            return new Group { TenantId = seedInfo.TenantId, Name = "Company", GroupId = Guid.NewGuid(), Owners = owners, Links = links};
        });

        return (groups.ToList(), users);
    }

    public static (List<Group> Groups, List<User> Users) GenerateWithConstStringAndSameOwners(SeedInfo seedInfo)
    {
        var random = new Random();

        var users = Enumerable.Range(0, seedInfo.UserCount).Select(x => new User
        {
            UserId = Guid.NewGuid(), Name = "Test Name"
        }).ToList();

        var groups = Enumerable.Range(0, seedInfo.GroupCount).Select(x =>
        {
            var owners = users.Take(seedInfo.OwnerCount).ToList();

            var links = Enumerable.Range(0, seedInfo.LinkCount).Select(x => new Link
            {
                LinkId = Guid.NewGuid(), Url = "www.localhost.com"
            }).ToList();


            return new Group { TenantId = seedInfo.TenantId, Name = "Company", GroupId = Guid.NewGuid(), Owners = owners, Links = links};
        });

        return (groups.ToList(), users);
    }
}
