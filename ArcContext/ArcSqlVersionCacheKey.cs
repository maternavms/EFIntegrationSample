using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEfMultipleSqlVersions.ArcContext
{
    public class ArcSqlVersionCacheKey : ModelCacheKey
    {
        private readonly Version sqlVersion;

        public ArcSqlVersionCacheKey(IArcAgentDbContext context, Version sqlVersion)
            :base((DbContext)context, false)
        {
            this.sqlVersion = sqlVersion;
        }

        /// <summary>
        /// Do we want to match major/minor, or just major, or the whole?
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected override bool Equals(ModelCacheKey other)
        {
            var arcKey = (ArcSqlVersionCacheKey)other;
            return sqlVersion.Major == arcKey.sqlVersion.Major
                && sqlVersion.Minor == sqlVersion.Minor;
        }
    }
}
