using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Domain.Document;

public class GroupDocument
{
    [BsonId] public ObjectId Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid GroupId { get; set; }

    public string Name { get; set; }

    public ICollection<Guid> OwnersId { get; set; }

    public static IEnumerable<CreateIndexModel<GroupDocument>> GetIndexes()
    {
        yield return new CreateIndexModel<GroupDocument>(
            Builders<GroupDocument>.IndexKeys.Combine(
                Builders<GroupDocument>.IndexKeys.Ascending(d => d.TenantId),
                Builders<GroupDocument>.IndexKeys.Ascending(d => d.GroupId)),
            new CreateIndexOptions { Unique = true });
    }
}
