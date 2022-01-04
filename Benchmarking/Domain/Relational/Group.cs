namespace Domain.Relational;

public class Group
{
    public Guid TenantId { get; set; }

    public Guid GroupId { get; set; }

    public string Name { get; set; }

    public ICollection<User> Owners { get; set; }
}
