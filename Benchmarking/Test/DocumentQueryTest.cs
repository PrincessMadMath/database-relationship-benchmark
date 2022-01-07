using System;
using System.Linq;
using System.Threading.Tasks;
using Benchmark;
using Benchmark.Seed;
using Domain.Document;
using Xunit;

namespace Test;

public class DocumentQueryTest
{
    private readonly MongoRepository _mongoRepository;

    private static readonly Guid TenantWith100Groups = Guid.Parse("76482310-c21b-4496-b1ab-170cb7fbb657");

    public DocumentQueryTest()
    {
        this._mongoRepository = new MongoRepository();
    }

    [Fact]
    public async Task ChannelTest()
    {
        var groups = await DocumentQuery.GetTenantWithChannel(this._mongoRepository, TenantWith100Groups, 4);

        Assert.Equal(100, groups.Count);
        Assert.Equal(Seeder.DefaultOwnerCount, groups.First().Owners.Count);
        Assert.Equal(Seeder.DefaultLinkCount, groups.First().Links.Count);
    }

    [Fact]
    public async Task LookupTest()
    {
        var groups = await DocumentQuery.GetTenantWithLookup(this._mongoRepository, TenantWith100Groups);

        Assert.Equal(100, groups.Count);
        Assert.Equal(Seeder.DefaultOwnerCount, groups.First().Owners.Count);
        Assert.Equal(Seeder.DefaultLinkCount, groups.First().Links.Count);
    }

    [Fact]
    public async Task MongoViewTest()
    {
        var groups = await DocumentQuery.GetTenantOnView(this._mongoRepository, TenantWith100Groups);

        Assert.Equal(100, groups.Count);
        Assert.Equal(Seeder.DefaultOwnerCount, groups.First().Owners.Count);
        Assert.Equal(Seeder.DefaultLinkCount, groups.First().Links.Count);
    }

    [Fact]
    public async Task MongoMaterializedViewTest()
    {
        var groups = await DocumentQuery.GetTenantOnViewMaterialized(this._mongoRepository, TenantWith100Groups);

        Assert.Equal(100, groups.Count);
        Assert.Equal(Seeder.DefaultOwnerCount, groups.First().Owners.Count);
        Assert.Equal(Seeder.DefaultLinkCount, groups.First().Links.Count);
    }
}
