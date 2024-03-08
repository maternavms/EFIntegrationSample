using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEfMultipleSqlVersions.CtxBuilders;
using TestEfMultipleSqlVersions.Entities;

namespace TestEfMultipleSqlVersions.DbCtx
{
    public class SysDdContext : DbContext
    {
        private string connString;
        public SqlServerVersion SqlVersion { get; private set; }


        public DbSet<AvailabilityGroup> AvailabilityGroups { get; set; }
        public DbSet<DmHadrAvailabilityGroupState> DmHadrAvailabilityGroupStates { get; set; }

        public SysDdContext(SqlServerVersion sqlVersion, string connectionString)
        {
            SqlVersion = sqlVersion;
            connString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //NOTE: This logic will be xtracted to a context builder in a real implementation and correct builder will be injected here by context factory
            switch(SqlVersion)
            {
                case SqlServerVersion.Sql2022:
                case SqlServerVersion.Sql2019:
                    modelBuilder.BuildContextForSql2022();
                    break;
                case SqlServerVersion.Sql2017:
                    modelBuilder.BuildContextForSql2017(); 
                    break;
                case SqlServerVersion.Sql2016:
                    modelBuilder.BuildContextForSql2016();
                    break;
                default:
                    throw new InvalidOperationException($"Sql server {SqlVersion} is not supported");
            }
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(connString, opt =>
                {
                    opt.EnableRetryOnFailure();
                })
                //NOTE: This is the part where we are setting our own model caching logic
                .ReplaceService<IModelCacheKeyFactory, SqlVersionsModelCacheKeyFactory>();

            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
