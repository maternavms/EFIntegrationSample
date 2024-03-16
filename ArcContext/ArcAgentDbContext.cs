using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEfMultipleSqlVersions.CtxBuilders;
using TestEfMultipleSqlVersions.Extensions;

namespace TestEfMultipleSqlVersions.ArcContext
{
    /// <summary>
    /// 
    /// </summary>
    public class ArcAgentDbContext : DbContext, IArcAgentDbContext
    {
        public ArcAgentDbContext(DbContextOptions<ArcAgentDbContext> options): base(options)
        {            
        }
        /// <summary>
        /// Specify the instance name to connect to.
        /// </summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public IArcAgentDbContext Instance(string instanceName)
        {
            InstanceName = instanceName;
            ConnectionString = $"Server={instanceName};Database=master;Trusted_Connection=True;Trust Server Certificate=True;";
            return this;
        }
                
        public string? ConnectionString { get; private set; }

        public string? InstanceName { get; private set; }

        public IQueryable<T> EntitySet<T>() where T : class
        {
            this.Database.SetConnectionString(ConnectionString);
            return Set<T>();
        }

        public Version GetSqlVersion()
        {
            try
            {
                var sqlCommand = "select SERVERPROPERTY('productversion')";
                using var connection = new SqlConnection(ConnectionString);
                connection.Open();

                using var command = new SqlCommand(sqlCommand, connection);
                var results = command.ExecuteScalar().ToString();

                return Version.Parse(results ?? "999.999.999"); //unknown version take the highest
            }
            catch
            {
                return new Version(999, 999, 999);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var version = GetSqlVersion();
            // do the model building here
            modelBuilder.UseVersionSpecificModel(version);
        }


        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(options =>
                {
                    options.EnableRetryOnFailure(3);
                })
                .ReplaceService<IModelCacheKeyFactory, ArcModelCacheKeyFactory>();

            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
