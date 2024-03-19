using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEfMultipleSqlVersions.ArcContext
{
    public class ArcModelCacheKeyFactory : IModelCacheKeyFactory
    {
        private readonly IMemoryCache keyCache;

        public ArcModelCacheKeyFactory(IMemoryCache keyCache)
        {
            this.keyCache = keyCache;
        }

        public object Create(DbContext context, bool designTime)
        {
            var arcContext = context as IArcAgentDbContext ?? throw new ArgumentException($"Provided context of type {context.GetType().Name} does not implement {nameof(IArcAgentDbContext)}.", nameof(context));
            var instance = arcContext.InstanceName;
            var key = keyCache.Get<ArcSqlVersionCacheKey>($"{instance}_context_cache_key");
            if (key == null)
            {
                var sqlVersion = arcContext.GetSqlVersion();
                key = new ArcSqlVersionCacheKey(arcContext, sqlVersion);
                keyCache.Set($"{instance}_context_key", key, new MemoryCacheEntryOptions {Size=1024});
            }
            return key;
        }
    }
}
