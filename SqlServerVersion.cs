using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEfMultipleSqlVersions
{
    /// <summary>
    /// 
    /// </summary>
    public enum SqlServerVersion
    {
        [Description("SQL Server 2022")]
        Sql2022 = 16,

        [Description("SQL Server 2019")]
        Sql2019 = 15,

        [Description("SQL Server 2017")]
        Sql2017 = 14,

        [Description("SQL Server 2016")]
        Sql2016 = 13,

        [Description("SQL Server 2014")]
        Sql2014 = 12,

        [Description("SQL Server 2012")]
        Sql2012 = 11
    }
}
