namespace Benchmark.Seed;

public class SeedInfo
{
    public SeedInfo(Guid tenantId, int groupCount, int ownerCount, int linkCount)
    {
        this.TenantId = tenantId;
        this.GroupCount = groupCount;
        this.OwnerCount = ownerCount;
        this.LinkCount = linkCount;
    }

    public Guid TenantId { get; }

    public int GroupCount { get; }

    public int OwnerCount { get; }

    public int LinkCount { get; }

    public override string ToString()
    {
        return $"{this.GroupCount} g | {this.LinkCount} o | {this.LinkCount} l";
    }
}
