
using Microsoft.EntityFrameworkCore;

namespace TestEfMultipleSqlVersions.ArcContext
{
    public interface IArcAgentDbContext
    {
        string? ConnectionString { get; }
        string? InstanceName { get; }

        IQueryable<T> EntitySet<T>() where T : class;
        Version GetSqlVersion();
        IArcAgentDbContext Instance(string instanceName);
    }
}