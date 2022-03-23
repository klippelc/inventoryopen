using Inventory.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class Invoice : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string PONumber { get; set; }

        public int? SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public decimal? TotalPrice { get; set; }

        [DefaultValue("false")]
        public bool IsDeleted { get; set; }

        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}