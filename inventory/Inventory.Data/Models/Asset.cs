using Inventory.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class Asset : BaseEntity
    {
        public int Id { get; set; }

        public int? InvoiceItemId { get; set; }

        [ForeignKey("InvoiceItemId")]
        public InvoiceItem InvoiceItem { get; set; }

        public string AssetTag { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [MaxLength(150)]
        public string Drawer { get; set; }

        [MaxLength(150)]
        public string Serial { get; set; }

        [MaxLength(150)]
        public string LicenseKeyMulti { get; set; }

        [MaxLength(255)]
        public string MacAddress { get; set; }

        [MaxLength(50)]
        public string IPAddress { get; set; }

        public int? StatusId { get; set; }

        [ForeignKey("StatusId")]
        public AssetStatus Status { get; set; }

        [MaxLength(15)]
        public string SNFnumber { get; set; }

        public DateTime? SurplusDate { get; set; }

        public DateTime? DateReceived { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LastBootDate { get; set; }

        public int? LocationId { get; set; }

        [ForeignKey("LocationId")]
        public Location Location { get; set; }

        public int? BuildingId { get; set; }

        [ForeignKey("BuildingId")]
        public Building Building { get; set; }

        public int? RoomId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        public int? AssignedUserId { get; set; }

        [ForeignKey("AssignedUserId")]
        public User AssignedUser { get; set; }

        public int? AssignedAssetId { get; set; }

        [ForeignKey("AssignedAssetId")]
        public Asset AssignedAsset { get; set; }

        public DateTime? AssignedDate { get; set; }

        // 
        public int? ConnectedAssetId { get; set; }

        [ForeignKey("ConnectedAssetId")]
        public Asset ConnectedAsset { get; set; }
        //
        
        [MaxLength(500)]
        public string Notes { get; set; }

        [DefaultValue("true")]
        public bool Display { get; set; }

        [DefaultValue("false")]
        public bool IsDeleted { get; set; }

        public virtual ICollection<Asset> AssignedAssets { get; set; }
        public virtual ICollection<Asset> ConnectedAssets { get; set; }
        public virtual ICollection<AssetUserList> AssetUserLists { get; set; }
        public virtual ICollection<UserAssetConfirmation> UserAssetConfirmations { get; set; }
    }
}