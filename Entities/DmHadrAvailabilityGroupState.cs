using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEfMultipleSqlVersions.Entities
{
    [Table("dm_hadr_availability_group_states", Schema = "sys")]
    public class DmHadrAvailabilityGroupState
    {
        public static string ViewName => "dm_hadr_availability_group_states";

        [Key]
        [Column("group_id")]
        public Guid GroupId { get; set; }

        [Column("primary_replica")]
        public string? PrimaryReplica { get; set; }

        [Column("primary_recovery_health")]
        public byte? PrimaryRecoveryHealth { get; set; }

        [Column("primary_recovery_health_desc")]
        public string? PrimaryRecoveryHealthDescription { get; set; }

        [Column("secondary_recovery_health")]
        public byte? SecondaryRecoveryHealth { get; set; }

        [Column("secondary_recovery_health_desc")]
        public string? SecondaryRecoveryHealthDescription { get; set; }

        [Column("synchronization_health")]
        public byte? SynchronizationHealth { get; set; }

        [Column("synchronization_health_desc")]
        public string? SynchronizationHealthDescription { get; set; }

        public AvailabilityGroup AvailabilityGroup { get; set; }
    }
}
