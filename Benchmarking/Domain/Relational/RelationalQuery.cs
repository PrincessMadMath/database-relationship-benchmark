using Microsoft.EntityFrameworkCore;

namespace Domain.Relational;

public static class RelationalQuery
{
    public static Task<List<Group>> GetTenantGroupsWithEagerLoading(RelationalRepositoryContext relationalRepository,
        Guid tenantId)
    {
        return relationalRepository.Groups.Where(x => x.TenantId == tenantId).Include(a => a.Owners).ToListAsync();
    }
}
