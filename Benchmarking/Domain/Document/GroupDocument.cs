namespace Domain.Document;

public class GroupDocument
{
    public Guid TenantId { get; set; }

    public Guid GroupId { get; set; }

    public string Name { get; set; }
}
