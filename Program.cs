using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using TestEfMultipleSqlVersions.ArcContext;
using TestEfMultipleSqlVersions.ArcEntities;
using TestEfMultipleSqlVersions.DbCtx;

namespace TestEfMultipleSqlVersions
{
    internal class Program
    {

        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddMemoryCache(opts =>
            {
                opts.ExpirationScanFrequency = System.TimeSpan.FromMinutes(1);
            });

            services.AddDbContext<IArcAgentDbContext, ArcAgentDbContext>(options =>
            {
                options.ReplaceService<IModelCacheKeyFactory, ArcModelCacheKeyFactory>();
            });

            var serviceProvider = services.BuildServiceProvider();

            var context1 = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IArcAgentDbContext>().Instance(".");
            var context2 = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IArcAgentDbContext>().Instance(".\\Sql_2014");
            var context3 = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IArcAgentDbContext>().Instance(".\\Sql_2014");

            var sql2022 = context1.EntitySet<AllViews>().First();
            var sql2014 = context2.EntitySet<AllViews>().First();
            var sql2022_2 = context1.EntitySet<AllViews>().First();

            Console.WriteLine($"Sql2022 column name = {sql2022.Name} Type = {sql2022.Type}");
            Console.WriteLine($"Sql2014 column name = {sql2014.Name} Type = {sql2014.Type}");
         }

        //static void Main(string[] args)
        //{

        //    var serverName = ".";

        //    var dbConnStr2022 = $"Data Source={serverName};Initial Catalog=master;Integrated Security=True;Trust Server Certificate=True";
        //    var dbConnStr2016 = $"Data Source={serverName};Initial Catalog=master;Integrated Security=True;Trust Server Certificate=True";

        //    //var ctx2017 = new SysDdContext(SqlServerVersion.Sql2017, dbConnStr2017);
        //    //var testData17 = ctx2017.AvailabilityGroups.Include(a => a.AvailabilityGroupState).ToList();

        //    var ctx2016 = new SysDdContext(SqlServerVersion.Sql2016, dbConnStr2016);
        //    var testData16 = ctx2016.AvailabilityGroups.Include(a => a.AvailabilityGroupState).ToList();

        //    var ctx2022 = new SysDdContext(SqlServerVersion.Sql2022, dbConnStr2022);
        //    var testData22 = ctx2022.AvailabilityGroups.Include(a => a.AvailabilityGroupState).ToList();
        //}
    }
}
