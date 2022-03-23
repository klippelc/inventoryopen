using System;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Web.ViewModels
{
    public class AuditLogVM
    {
        public int Id { get; set; }

        public int PrimaryKeyId { get; set; }

        [MaxLength(255)]
        public string TableName { get; set; }

        [MaxLength(255)]
        public string ColumnName { get; set; }

        [MaxLength(255)]
        public string OperationName { get; set; }

        public string OperationAndDate { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string ChangeSummary { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? DateModified { get; set; }

        [MaxLength(255)]
        public string ModifiedByName { get; set; }
    }
}