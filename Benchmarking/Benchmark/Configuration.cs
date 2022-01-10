using Domain.Document;
using Domain.Relational;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Benchmark;

public static class Configuration
{
    public static DbContextOptions<RelationalRepositoryContext> PostgresOptions =
        new DbContextOptionsBuilder<RelationalRepositoryContext>()
            .UseNpgsql("Host=localhost:5432;Database=benchmark;Username=dbuser;Password=admin1234")
            .Options;

    public static async Task ResetDatabase(RelationalRepositoryContext relationalRepository,
        MongoRepository mongoRepository)
    {
        relationalRepository.Database.EnsureDeleted();
        relationalRepository.Database.EnsureCreated();

        await mongoRepository.MongoDatabase.DropCollectionAsync(MongoRepository.GroupCollectionName);
        await mongoRepository.MongoDatabase.DropCollectionAsync(MongoRepository.UserCollectionName);
        await mongoRepository.MongoDatabase.DropCollectionAsync(MongoRepository.LinkCollectionName);
        await mongoRepository.MongoDatabase.DropCollectionAsync(MongoRepository.GroupViewCollectionName);
        await mongoRepository.MongoDatabase.DropCollectionAsync(MongoRepository.GroupViewMaterializedCollectionName);

        await mongoRepository.GroupCollection.Indexes.CreateManyAsync(GroupDocument.GetIndexes());
        await mongoRepository.UserCollection.Indexes.CreateManyAsync(UserDocument.GetIndexes());
        await mongoRepository.LinkCollection.Indexes.CreateManyAsync(LinkDocument.GetIndexes());

        // TODO: Remove duplicate of aggregation
        await mongoRepository.MongoDatabase.CreateViewAsync(
            MongoRepository.GroupViewCollectionName,
            MongoRepository.GroupCollectionName,
            new EmptyPipelineDefinition<GroupDocument>()
                .Lookup<GroupDocument, GroupDocument, UserDocument, DocumentQuery.GroupWithUser>(
                    mongoRepository.UserCollection, groupDocument => groupDocument.OwnersId,
                    userDocument => userDocument.UserId, x => x.Owners)
                .Lookup<GroupDocument, DocumentQuery.GroupWithUser, LinkDocument, GroupViewDocument>(
                    mongoRepository.LinkCollection, groupDocument => groupDocument.GroupId,
                    linkDocument => linkDocument.GroupId, x => x.Links)
        );
    }
    
    public static async Task ResetDatabase(MongoRepository mongoRepository)
    {
        await mongoRepository.MongoDatabase.DropCollectionAsync(MongoRepository.GroupCollectionName);
        await mongoRepository.MongoDatabase.DropCollectionAsync(MongoRepository.UserCollectionName);
        await mongoRepository.MongoDatabase.DropCollectionAsync(MongoRepository.LinkCollectionName);
        await mongoRepository.MongoDatabase.DropCollectionAsync(MongoRepository.GroupViewCollectionName);
        await mongoRepository.MongoDatabase.DropCollectionAsync(MongoRepository.GroupViewMaterializedCollectionName);

        await mongoRepository.GroupCollection.Indexes.CreateManyAsync(GroupDocument.GetIndexes());
        await mongoRepository.UserCollection.Indexes.CreateManyAsync(UserDocument.GetIndexes());
        await mongoRepository.LinkCollection.Indexes.CreateManyAsync(LinkDocument.GetIndexes());

        // TODO: Remove duplicate of aggregation
        await mongoRepository.MongoDatabase.CreateViewAsync(
            MongoRepository.GroupViewCollectionName,
            MongoRepository.GroupCollectionName,
            new EmptyPipelineDefinition<GroupDocument>()
                .Lookup<GroupDocument, GroupDocument, UserDocument, DocumentQuery.GroupWithUser>(
                    mongoRepository.UserCollection, groupDocument => groupDocument.OwnersId,
                    userDocument => userDocument.UserId, x => x.Owners)
                .Lookup<GroupDocument, DocumentQuery.GroupWithUser, LinkDocument, GroupViewDocument>(
                    mongoRepository.LinkCollection, groupDocument => groupDocument.GroupId,
                    linkDocument => linkDocument.GroupId, x => x.Links)
        );
    }

    public static async Task CreateMaterializedView(MongoRepository mongoRepository)
    {
        // Create on demand materialized view to simulate denormalized
        await mongoRepository.AggregateAsyncEnumerable(
            mongoRepository.GroupCollection,
            new EmptyPipelineDefinition<GroupDocument>()
                .Lookup<GroupDocument, GroupDocument, UserDocument, DocumentQuery.GroupWithUser>(
                    mongoRepository.UserCollection, groupDocument => groupDocument.OwnersId,
                    userDocument => userDocument.UserId, x => x.Owners)
                .Lookup<GroupDocument, DocumentQuery.GroupWithUser, LinkDocument, GroupViewDocument>(
                    mongoRepository.LinkCollection, groupDocument => groupDocument.GroupId,
                    linkDocument => linkDocument.GroupId, x => x.Links)
                .Merge(mongoRepository.GroupViewMaterializedCollection,
                    new MergeStageOptions<GroupViewDocument> { WhenMatched = MergeStageWhenMatched.Replace })
        ).ToListAsync();
    }
}
