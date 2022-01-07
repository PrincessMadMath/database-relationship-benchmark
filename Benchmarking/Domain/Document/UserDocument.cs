using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Domain.Document;

public class UserDocument
{
    [BsonId] public ObjectId Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; }

    public static IEnumerable<CreateIndexModel<UserDocument>> GetIndexes()
    {
        yield return new CreateIndexModel<UserDocument>(
            Builders<UserDocument>.IndexKeys.Ascending(d => d.UserId),
            new CreateIndexOptions { Unique = true });
    }
}
