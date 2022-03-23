using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int PrimaryKeyId { get; set; }

        [MaxLength(255)]
        public string TableName { get; set; }

        [MaxLength(255)]
        public string ColumnName { get; set; }

        [MaxLength(255)]
        public string OperationName { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string OldValue { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string NewValue { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? DateModified { get; set; }

        [MaxLength(255)]
        public string ModifiedByName { get; set; }
    }
}