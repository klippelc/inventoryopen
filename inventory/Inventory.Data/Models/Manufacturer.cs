using Inventory.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Models
{
    public class Manufacturer : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(150)]
        public string DisplayName { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [MaxLength(150)]
        public string SupportUrl { get; set; }

        [MaxLength(30)]
        public string SupportPhone { get; set; }

        [MaxLength(200)]
        public string SupportEmail { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        [DefaultValue(true)]
        public bool Active { get; set; }

        [DefaultValue("false")]
        public bool IsDeleted { get; set; }

        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}