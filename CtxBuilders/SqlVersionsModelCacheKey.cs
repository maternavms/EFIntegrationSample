using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TestEfMultipleSqlVersions.DbCtx;

namespace TestEfMultipleSqlVersions.CtxBuilders
{
    /// <summary>
    /// This is our custom model caching based on SQLServer version
    /// </summary>
    public class SqlVersionsModelCacheKey : ModelCacheKey
    {
        SqlServerVersion? _sqlVersion;
        public SqlVersionsModelCacheKey(DbContext context) : base(context)
        {
            _sqlVersion = (context as SysDdContext)?.SqlVersion;
        }

        protected override bool Equals(ModelCacheKey other)
        {
            return base.Equals(other) && (other as SqlVersionsModelCacheKey)?._sqlVersion == _sqlVersion;
        }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode() * 397;
            if (_sqlVersion != null)
            {
                hashCode ^= _sqlVersion.GetHashCode();
            }

            return hashCode;
        }
    }
}
