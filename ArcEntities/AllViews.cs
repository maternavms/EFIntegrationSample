using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEfMultipleSqlVersions.Attributes;
using TestEfMultipleSqlVersions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TestEfMultipleSqlVersions.ArcEntities
{
    [Table("all_views", Schema = "sys")]
    public class AllViews
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("object_id")]
        [Key]
        public int ObjectId { get; set; }

        [Column("schema_id")]
        public int SchemaId { get; set; }

        [Column("type")]
        [SupportedFrom(SqlServerVersion.Sql2019)]
        [NotMapped]
        public string Type { get; set; }
    }

    public class AllViewsConfiguration : IEntityTypeConfiguration<AllViews>
    {
        public void Configure(EntityTypeBuilder<AllViews> builder)
        {
            //no need to configure anything here as the model is attribute based.
            //this would be used in cases where we do not have access to modify the model
            //it is needed now because I don't have the model builder registering entities by convention.
        }
    }
}
