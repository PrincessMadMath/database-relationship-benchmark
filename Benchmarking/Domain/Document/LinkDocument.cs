using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Domain.Document;

public class LinkDocument
{
    [BsonId] public ObjectId Id { get; set; }

    public Guid LinkId { get; set; }

    // Parent-reference
    public Guid GroupId { get; set; }

    public string Url { get; set; }

    public static IEnumerable<CreateIndexModel<LinkDocument>> GetIndexes()
    {
        yield return new CreateIndexModel<LinkDocument>(
            Builders<LinkDocument>.IndexKeys.Ascending(d => d.LinkId),
            new CreateIndexOptions { Unique = true });

        yield return new CreateIndexModel<LinkDocument>(
            Builders<LinkDocument>.IndexKeys.Ascending(d => d.GroupId));
    }
}
