using Microsoft.EntityFrameworkCore;
using TestEfMultipleSqlVersions.DbCtx;

namespace TestEfMultipleSqlVersions
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serverName = "YourServerGoesHere";

            var dbConnStr2022 = $"Data Source={serverName};Initial Catalog=master;Integrated Security=True;Trust Server Certificate=True";
            var dbConnStr2016 = $"Data Source={serverName};Initial Catalog=master;Integrated Security=True;Trust Server Certificate=True";

            //var ctx2017 = new SysDdContext(SqlServerVersion.Sql2017, dbConnStr2017);
            //var testData17 = ctx2017.AvailabilityGroups.Include(a => a.AvailabilityGroupState).ToList();

            var ctx2016 = new SysDdContext(SqlServerVersion.Sql2016, dbConnStr2016);
            var testData16 = ctx2016.AvailabilityGroups.Include(a => a.AvailabilityGroupState).ToList();

            var ctx2022 = new SysDdContext(SqlServerVersion.Sql2022, dbConnStr2022);
            var testData22 = ctx2022.AvailabilityGroups.Include(a => a.AvailabilityGroupState).ToList();
        }
    }
}
