using System.Collections.Immutable;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Domain.Document;

public class MongoRepository
{
    public const string GroupCollectionName = "Groups";
    public const string UserCollectionName = "Users";
    public const string LinkCollectionName = "Links";
    public const string GroupViewCollectionName = "GroupsView";
    public const string GroupViewMaterializedCollectionName = "GroupsViewMaterialized";

    public readonly IMongoDatabase MongoDatabase;

    public MongoRepository()
    {
        // TODO: setup elsewhere
        var mongoSetting = MongoClientSettings.FromConnectionString("mongodb://localhost:27017");
        mongoSetting.GuidRepresentation = GuidRepresentation.Standard;

        // Not working with filter...
        // BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        var mongoClient = new MongoClient(mongoSetting);

        this.MongoDatabase = mongoClient.GetDatabase("default");

        this.GroupCollection = this.MongoDatabase.GetCollection<GroupDocument>(GroupCollectionName);
        this.UserCollection = this.MongoDatabase.GetCollection<UserDocument>(UserCollectionName);
        this.LinkCollection = this.MongoDatabase.GetCollection<LinkDocument>(LinkCollectionName);
        this.GroupViewCollection = this.MongoDatabase.GetCollection<GroupViewDocument>(GroupViewCollectionName);
        this.GroupViewMaterializedCollection =
            this.MongoDatabase.GetCollection<GroupViewDocument>(GroupViewMaterializedCollectionName);
    }

    public IMongoCollection<GroupDocument> GroupCollection { get; set; }

    public IMongoCollection<UserDocument> UserCollection { get; set; }

    public IMongoCollection<LinkDocument> LinkCollection { get; set; }

    public IMongoCollection<GroupViewDocument> GroupViewCollection { get; set; }

    public IMongoCollection<GroupViewDocument> GroupViewMaterializedCollection { get; set; }

    public IAsyncEnumerable<TDocument> FindAsyncEnumerable<TDocument>(IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument> filter)
    {
        return AsyncEnumerable.Create(
                token =>
                {
                    IAsyncCursor<TDocument> cursor = null;

                    async ValueTask<bool> MoveNextAsync()
                    {
                        cursor ??= await collection.FindAsync(filter, null, token);

                        return await cursor.MoveNextAsync(token);
                    }

                    return AsyncEnumerator.Create(
                        MoveNextAsync,
                        () => cursor?.Current ?? ImmutableList<TDocument>.Empty,
                        () =>
                        {
                            cursor?.Dispose();
                            return default;
                        });
                })
            .SelectMany(x => x.ToAsyncEnumerable());
    }

    public IAsyncEnumerable<TResult> AggregateAsyncEnumerable<TDocument, TResult>(
        IMongoCollection<TDocument> collection, PipelineDefinition<TDocument, TResult> pipeline)
    {
        return AsyncEnumerable.Create(
                token =>
                {
                    IAsyncCursor<TResult> cursor = null;

                    async ValueTask<bool> MoveNextAsync()
                    {
                        cursor ??= await collection.AggregateAsync(pipeline, null, token);

                        return await cursor.MoveNextAsync(token);
                    }

                    return AsyncEnumerator.Create(
                        MoveNextAsync,
                        () => cursor?.Current ?? ImmutableList<TResult>.Empty,
                        () =>
                        {
                            cursor?.Dispose();
                            return default;
                        });
                })
            .SelectMany(x => x.ToAsyncEnumerable());
    }
}
