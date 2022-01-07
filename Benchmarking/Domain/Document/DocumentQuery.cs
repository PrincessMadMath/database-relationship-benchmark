using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Open.ChannelExtensions;

namespace Domain.Document;

public class DocumentQuery
{
    public static async Task<List<GroupDto>> GetTenantForeach(MongoRepository repository,
        Guid tenantId)
    {
        var groupFilter = Builders<GroupDocument>.Filter.Eq(d => d.TenantId, tenantId);
        var groupsDocument =
            await repository.FindAsyncEnumerable(repository.GroupCollection, groupFilter).ToListAsync();

        var groupsDto = new List<GroupDto>();

        foreach (var groupDocument in groupsDocument)
        {
            var userFilter = Builders<UserDocument>.Filter.In(d => d.UserId, groupDocument.OwnersId);
            var owners = await repository.FindAsyncEnumerable(repository.UserCollection, userFilter).ToListAsync();

            var linksFilter = Builders<LinkDocument>.Filter.Eq(d => d.GroupId, groupDocument.GroupId);
            var links = await repository.FindAsyncEnumerable(repository.LinkCollection, linksFilter)
                .ToListAsync();

            groupsDto.Add(new GroupDto
            {
                TenantId = groupDocument.TenantId,
                GroupId = groupDocument.GroupId,
                Name = groupDocument.Name,
                Owners = owners,
                Links = links
            });
        }

        return groupsDto;
    }

    public static async Task<List<GroupDto>> GetTenantWithChannel(MongoRepository repository,
        Guid tenantId, int maxConcurrency)
    {
        var filter = Builders<GroupDocument>.Filter.Eq(d => d.TenantId, tenantId);

        var groups = await repository.FindAsyncEnumerable(
                repository.GroupCollection,
                filter)
            .ToChannel()
            .PipeAsync(
                maxConcurrency,
                async document =>
                {
                    var userFilter = Builders<UserDocument>.Filter.In(d => d.UserId, document.OwnersId);
                    var owners = await repository.FindAsyncEnumerable(repository.UserCollection, userFilter)
                        .ToListAsync();

                    var linksFilter = Builders<LinkDocument>.Filter.Eq(d => d.GroupId, document.GroupId);
                    var links = await repository.FindAsyncEnumerable(repository.LinkCollection, linksFilter)
                        .ToListAsync();

                    return new GroupDto
                    {
                        TenantId = document.TenantId,
                        GroupId = document.GroupId,
                        Name = document.Name,
                        Owners = owners,
                        Links = links
                    };
                }
            ).ToListAsync();

        return groups;
    }

    public static async Task<List<GroupViewDocument>> GetTenantWithLookup(MongoRepository repository,
        Guid tenantId)
    {
        var pipelineDefinition = new EmptyPipelineDefinition<GroupDocument>()
            .Match(Builders<GroupDocument>.Filter.Eq(d => d.TenantId, tenantId))
            .Lookup<GroupDocument, GroupDocument, UserDocument, GroupWithUser>(repository.UserCollection,
                groupDocument => groupDocument.OwnersId,
                userDocument => userDocument.UserId, x => x.Owners)
            .Lookup<GroupDocument, GroupWithUser, LinkDocument, GroupViewDocument>(repository.LinkCollection,
                groupDocument => groupDocument.GroupId,
                linkDocument => linkDocument.GroupId, x => x.Links);

        var groups = await repository.AggregateAsyncEnumerable(repository.GroupCollection, pipelineDefinition)
            .ToListAsync();

        return groups;
    }

    public static async Task<List<GroupViewDocument>> GetTenantOnView(MongoRepository repository,
        Guid tenantId)
    {
        var filter = Builders<GroupViewDocument>.Filter.Eq(d => d.TenantId, tenantId);

        var groups = await repository.FindAsyncEnumerable(repository.GroupViewCollection, filter).ToListAsync();

        return groups;
    }

    public static async Task<List<GroupViewDocument>> GetTenantOnViewMaterialized(MongoRepository repository,
        Guid tenantId)
    {
        var filter = Builders<GroupViewDocument>.Filter.Eq(d => d.TenantId, tenantId);

        var groups = await repository.FindAsyncEnumerable(repository.GroupViewMaterializedCollection, filter)
            .ToListAsync();

        return groups;
    }

    // TODO: Cleanup
    public class GroupDto
    {
        [BsonId] public ObjectId Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid GroupId { get; set; }

        public string Name { get; set; }

        public ICollection<Guid> OwnersId { get; set; }

        public ICollection<UserDocument> Owners { get; set; }

        public ICollection<LinkDocument> Links { get; set; }
    }

    public class GroupWithUser
    {
        [BsonId] public ObjectId Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid GroupId { get; set; }

        public string Name { get; set; }

        public ICollection<Guid> OwnersId { get; set; }

        public ICollection<UserDocument> Owners { get; set; }
    }
}
