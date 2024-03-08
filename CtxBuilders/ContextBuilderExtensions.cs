using Microsoft.EntityFrameworkCore;
using TestEfMultipleSqlVersions.Entities;

namespace TestEfMultipleSqlVersions.CtxBuilders
{
    /// <summary>
    /// SQL Version specific builders. Columns that are not in all SQL versions
    /// must be mapped manually. In this case those columns are: 
    /// is_contained - present in SQL2019+
    /// cluster_type - present in SQL2017+
    /// required_synchronized_secondaries_to_commit" - present in SQL2017+
    /// </summary>
    internal static class ContextBuilderExtensions
    {
        public static void BuildContextForSql2022(this ModelBuilder modelBuilder)
        {
            //In this case all 3 columns are present, so we map them
            modelBuilder.Entity<AvailabilityGroup>(entity => {
                entity
                .Property(p => p.ClusterType)
                .HasColumnName("cluster_type")
                .HasColumnType("tinyint")
                .IsRequired(false);

                entity
                .Property(p => p.RequiredSynchronizedSecondariesToCommit)
                .HasColumnName("required_synchronized_secondaries_to_commit")
                .HasColumnType("int")
                .IsRequired(false);

                entity
                .Property(p => p.IsContained)
                .HasColumnName("is_contained")
                .HasColumnType("bit")
                .IsRequired(false);
                //This is 'relation' mapping between AvailabilityGroup and DmHadrAvailabilityGroupState. So that we can use Include statement to hydrate related object when querying data
                entity
                .HasOne(a => a.AvailabilityGroupState)
                .WithOne(h => h.AvailabilityGroup)
                .HasForeignKey<DmHadrAvailabilityGroupState>(f => f.GroupId)
                .IsRequired(false);
            });

            modelBuilder.Entity<DmHadrAvailabilityGroupState>(entity => { });

        }

        public static void BuildContextForSql2017(this ModelBuilder modelBuilder)
        {
            // For 2017 only cluster_type and required_synchronized_secondaries_to_commit are present so we map those
            // and ignore is_contained
            modelBuilder.Entity<AvailabilityGroup>(entity => {
                entity
                .Property(p => p.ClusterType)
                .HasColumnName("cluster_type")
                .HasColumnType("tinyint")
                .IsRequired(false);

                entity
                .Property(p => p.RequiredSynchronizedSecondariesToCommit)
                .HasColumnName("required_synchronized_secondaries_to_commit")
                .HasColumnType("int")
                .IsRequired(false);

                entity
                .Ignore(a => a.IsContained)
                .HasOne(a => a.AvailabilityGroupState)
                .WithOne(h => h.AvailabilityGroup)
                .HasForeignKey<DmHadrAvailabilityGroupState>(f => f.GroupId)
                .IsRequired(false);
            });

            modelBuilder.Entity<DmHadrAvailabilityGroupState>(entity => { });
        }

        public static void BuildContextForSql2016(this ModelBuilder modelBuilder)
        {
            // For 2016 none of the columns is present, so we have to ignore all three
            modelBuilder.Entity<AvailabilityGroup>(entity => {
                entity
                .Ignore(a => a.RequiredSynchronizedSecondariesToCommit)
                .Ignore(a => a.ClusterType)
                .Ignore(a => a.IsContained)
                .HasOne(a => a.AvailabilityGroupState)
                .WithOne(h => h.AvailabilityGroup)
                .HasForeignKey<DmHadrAvailabilityGroupState>(f => f.GroupId)
                .IsRequired(false);
            });

            modelBuilder.Entity<DmHadrAvailabilityGroupState>(entity => { });
        }
    }
}

/*
 *  //Only in SQL2017+
        //[Column("required_synchronized_secondaries_to_commit")]
        public int RequiredSynchronizedSecondariesToCommit { get; set; }

        //Only in SQL2017+
        //[Column("cluster_type")]
        public int ClusterType { get; set; }

        //Only in SQL2019+
        //[Column("is_contained")]
        public bool IsContained { get; set; }
 */