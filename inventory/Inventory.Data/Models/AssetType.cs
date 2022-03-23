using Inventory.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Models
{
    public class AssetType : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string IconCss { get; set; }

        [DefaultValue("true")]
        public bool Active { get; set; }

        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }
        public virtual ICollection<AssetCategory> AssetCategories { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<AssetStatusType> AssetStatusTypes { get; set; }
        public virtual ICollection<AssetList> AssetLists { get; set; }
    }
}