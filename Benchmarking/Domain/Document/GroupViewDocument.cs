using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Document;

public class GroupViewDocument
{
    [BsonId]
    public ObjectId Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid GroupId { get; set; }

    public string Name { get; set; }

    public ICollection<Guid> OwnersId { get; set; }

    public ICollection<UserDocument> Owners { get; set; }

    public ICollection<LinkDocument> Links { get; set; }
}
