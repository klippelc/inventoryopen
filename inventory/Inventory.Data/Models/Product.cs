using Inventory.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(150)]
        public string DisplayName { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        public int? ManuId { get; set; }

        [ForeignKey("ManuId")]
        public Manufacturer Manufacturer { get; set; }

        [DefaultValue(1)]
        public int? AssetTypeId { get; set; }

        public AssetType AssetType { get; set; }

        [DefaultValue(1)]
        public int? AssetCategoryId { get; set; }

        public AssetCategory AssetCategory { get; set; }

        [DefaultValue(true)]
        public bool Active { get; set; }

        [DefaultValue("false")]
        public bool IsDeleted { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}