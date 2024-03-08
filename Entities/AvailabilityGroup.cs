using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEfMultipleSqlVersions.Entities
{
    [Table("availability_groups", Schema = "sys")]
    public class AvailabilityGroup
    {
        [Column("name")]
        public string? Name { get; set; }

        [Column("group_id")]
        [Key]
        public Guid GroupId { get; set; }

        [Column("resource_id")]
        public string? ResourceId { get; set; }

        [Column("resource_group_id")]
        public string? ResourceGroupId { get; set; }

        [Column("failure_condition_level")]
        public int? FailureConditionLevel { get; set; }

        [Column("health_check_timeout")]
        public int? HealthCheckTimeoutMs { get; set; }

        [Column("automated_backup_preference")]
        public byte? AutomatedBackupPreference { get; set; }

        [Column("automated_backup_preference_desc")]
        public string? AutomatedBackupPreferenceDesc { get; set; }

        [Column("version")]
        public short? Version { get; set; }

        [Column("basic_features")]
        public bool? BasicFeatures { get; set; }

        [Column("dtc_support")]
        public bool? DtcSupport { get; set; }

        [Column("db_failover")]
        public bool? DbFailover { get; set; }

        [Column("is_distributed")]
        public bool? IsDistributed { get; set; }

        [Column("cluster_type_desc")]
        public string? ClusterTypeDesc { get; set; }

        [Column("sequence_number")]
        public long? SequenceNumber { get; set; }

        //Only in SQL2017+
        //[Column("required_synchronized_secondaries_to_commit")]
        public int? RequiredSynchronizedSecondariesToCommit { get; set; }

        //Only in SQL2017+
        //[Column("cluster_type")]
        public byte? ClusterType { get; set; }

        //Only in SQL2019+
        //[Column("is_contained")]
        public bool? IsContained { get; set; }

        public DmHadrAvailabilityGroupState AvailabilityGroupState { get; set; }
    }
}
