namespace Domain.Relational;

public class User
{
    public Guid UserId { get; set; }

    public string Name { get; set; }

    public ICollection<Group> OwnersOf { get; set; }
}
