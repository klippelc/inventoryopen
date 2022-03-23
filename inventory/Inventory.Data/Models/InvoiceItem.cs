using Inventory.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class InvoiceItem : BaseEntity
    {
        public int Id { get; set; }

        public int? InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }

        public int? InvoiceItemNumber { get; set; }

        public int? AssetTypeId { get; set; }

        [ForeignKey("AssetTypeId")]
        public AssetType AssetType { get; set; }

        public int? AssetCategoryId { get; set; }

        [ForeignKey("AssetCategoryId")]
        public AssetCategory AssetCategory { get; set; }

        public int? ManuId { get; set; }

        [ForeignKey("ManuId")]
        public Manufacturer Manufacturer { get; set; }

        public int? ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int? LicenseTypeId { get; set; }

        [ForeignKey("LicenseTypeId")]
        public LicenseType LicenseType { get; set; }

        public string LicenseKeySingle { get; set; }

        public decimal? UnitPrice { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [MaxLength(500)]
        public string Specifications { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        [DefaultValue("false")]
        public bool IsDeleted { get; set; }

        public virtual ICollection<Asset> Assets { get; set; }
    }
}