using Microsoft.EntityFrameworkCore;

namespace Domain.Relational;

public static class RelationalQuery
{
    public static Task<List<Group>> GetTenantGroupsWithEagerLoading(RepositoryContext repository, Guid tenantId)
    {
        return repository.Groups.Where(x => x.TenantId == tenantId).Include(a => a.Owners).ToListAsync();
    }
}
