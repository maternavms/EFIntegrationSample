using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TestEfMultipleSqlVersions.CtxBuilders
{
    public class SqlVersionsModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context, bool designTime) => new SqlVersionsModelCacheKey(context);
    }
}
