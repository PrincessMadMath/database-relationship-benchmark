﻿using Domain.Relational;
using Faker;

namespace Benchmark;

public static class FakeDataGenerator
{
    public static List<Group> GenerateGroups(Guid tenantId, int groupCount, int userCount, int ownerCount)
    {
        var random = new Random();

        var users = Enumerable.Range(0, userCount).Select(x => new User
        {
            UserId = Guid.NewGuid(), Name = Name.FullName()
        });

        var groups = Enumerable.Range(0, groupCount).Select(x =>
        {
            var owners = users.Skip(random.Next(userCount - ownerCount)).Take(ownerCount).ToList();

            return new Group { TenantId = tenantId, GroupId = Guid.NewGuid(), Owners = owners };
        });

        return groups.ToList();
    }
}
