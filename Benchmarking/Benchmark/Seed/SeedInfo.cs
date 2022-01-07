namespace Benchmark.Seed;

public class SeedInfo
{
    public SeedInfo(Guid tenantId, int groupCount, int userCount, int ownerCount, int linkCount)
    {
        this.TenantId = tenantId;
        this.GroupCount = groupCount;
        this.UserCount = userCount;
        this.OwnerCount = ownerCount;
        this.LinkCount = linkCount;
    }

    public Guid TenantId { get; }

    public int GroupCount { get; }

    public int UserCount { get; }

    public int OwnerCount { get; }

    public int LinkCount { get; }

    public override string ToString()
    {
        return $"{this.GroupCount} groups | {this.OwnerCount} owners |  {this.LinkCount} links";
    }
}
