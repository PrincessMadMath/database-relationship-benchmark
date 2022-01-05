using System;
using System.Linq;
using System.Threading.Tasks;
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
        Assert.Equal(3, groups.First().Owners.Count);
    }

    [Fact]
    public async Task Test1()
    {
        var groups = await DocumentQuery.GetTenantWithLookup(this._mongoRepository, TenantWith100Groups);

        Assert.Equal(100, groups.Count);
        Assert.Equal(3, groups.First().Owners.Count);
    }
}
