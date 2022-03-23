using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Inventory.Web.ViewModels
{
    public class AssetVM
    {
        public int Id { get; set; }

        public int? InvoiceItemId { get; set; }

        public InvoiceItemVM InvoiceItem { get; set; }

        [RegularExpression("^[a-zA-Z0-9-]*$", ErrorMessage = "Only AlphaNumeric, and '-' allowed")]
        public string Name { get; set; }

        public string OriginalName { get; set; }

        public string Description { get; set; }

        public string OriginalDescription { get; set; }

        [RegularExpression("^[0-9]{1,4}$", ErrorMessage = "Drawer must be numeric")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Drawer must be 4 digits")]
        [Remote(action: "CheckDrawer", controller: "Asset", AdditionalFields = "OriginalDrawer", ErrorMessage = "Drawer not available")]
        [Display(Name = "Drawer Number")]
        public string Drawer { get; set; }

        public string OriginalDrawer { get; set; }

        [RegularExpression("[0-9]{4}([0-9]{2})?", ErrorMessage = "Must be Numeric, and 4 or 6 Chars")]
        [MinLength(4, ErrorMessage = "Asset Tag must be 4 or 6 digits")]
        [MaxLength(6, ErrorMessage = "Asset Tag must be 4 or 6 digits")]
        [Remote(action: "CheckAssetTag", controller: "Asset", AdditionalFields = "OriginalAssetTag", ErrorMessage = "Asset Tag not available")]
        [Display(Name = "Asset Tag")]
        public string AssetTag { get; set; }

        public string OriginalAssetTag { get; set; }

        [Remote(action: "CheckSerial", controller: "Asset", AdditionalFields = "OriginalSerial, ManuId", ErrorMessage = "Serial not available")]
        public string Serial { get; set; }

        public string OriginalSerial { get; set; }

        public string SerialandAssignedUser { get; set; }

        [Display(Name = "License Key")]
        public string LicenseKeyMulti { get; set; }

        public string OriginalLicenseKeyMulti { get; set; }

        [Display(Name = "Mac Address")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "MAC Format: xx:yy:zz:11:22:33")]
        [RegularExpression("^([0-9a-fA-F]{2}(?:[:-]?[0-9a-fA-F]{2}){5})$", ErrorMessage = "MAC Format: xx:yy:zz:11:22:33")]
        [Remote(action: "CheckMacAddress", controller: "Asset", AdditionalFields = "OriginalMacAddress", ErrorMessage = "Mac Address not available")]
        public string MacAddress { get; set; }

        public string OriginalMacAddress { get; set; }

        [RegularExpression(@"\b(?:(?:2(?:[0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9])\.){3}(?:(?:2([0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9]))\b", ErrorMessage = "IP Address 0-255.0-255.0-255.0-255")]
        [Display(Name = "IP Address")]
        [Remote(action: "CheckIPAddress", controller: "Asset", AdditionalFields = "OriginalIPAddress", ErrorMessage = "IP Address not available")]
        public string IPAddress { get; set; }

        public string OriginalIPAddress { get; set; }

        public int? StatusId { get; set; }

        public AssetStatusVM Status { get; set; }

        public int? OrignialStatusId { get; set; }

        [Display(Name = "Surplus Number")]
        public string SNFnumber { get; set; }

        [Display(Name = "Surplus Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? SurplusDate { get; set; }

        [Display(Name = "Date Received")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateReceived { get; set; }

        [Display(Name = "Date Received")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DefaultDateReceived { get; set; }

        [Display(Name = "Last Login Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? LastLoginDate { get; set; }

        [Display(Name = "Last Boot-Up Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? LastBootDate { get; set; }

        public bool? showCheckBox { get; set; }

        public int? LocationId { get; set; }

        public int? OriginalLocationId { get; set; }

        public LocationVM Location { get; set; }

        public int? BuildingId { get; set; }

        public int? OriginalBuildingId { get; set; }

        public BuildingVM Building { get; set; }

        public int? RoomId { get; set; }

        public int? OriginalRoomId { get; set; }

        public RoomVM Room { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public int? AssignedAssetId { get; set; }

        [Display(Name = "Assigned Asset")]
        public AssetVM AssignedAsset { get; set; }

        public int? OriginalAssignedAssetId { get; set; }

        public int? AssignedUserId { get; set; }

        [Display(Name = "Assigned User")]
        public UserVM AssignedUser { get; set; }

        public int? OriginalAssignedUserId { get; set; }

        public DateTime? AssignedDate { get; set; }
        //
        public int? ConnectedAssetId { get; set; }

        [Display(Name = "Connected Asset")]
        public AssetVM ConnectedAsset { get; set; }

        public int? OriginalConnectedAssetId { get; set; }
        //

        public bool Display { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }

        ///
        [Display(Name = "PO Number")]
        public string PONumber { get; set; }

        [Display(Name = "Invoice Number")]
        public int? InvoiceNumber { get; set; }

        [Display(Name = "Item Number")]
        public string InvoiceAndInvoiceItem { get; set; }

        [Display(Name = "Supplier")]
        public string SupplierName { get; set; }

        [Display(Name = "Purchase Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? PurchaseDate { get; set; }

        [Display(Name = "Unit Price")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal? UnitPrice { get; set; }

        [Display(Name = "Item Number")]
        public int? InvoiceItemNumber { get; set; }

        public int? ManuId { get; set; }

        public string ManufacturerName { get; set; }

        public int? ProductId { get; set; }

        public string ProductName { get; set; }

        [Display(Name = "Asset Type")]
        public string AssetTypeName { get; set; }

        [Display(Name = "Asset Category")]
        public string AssetCategoryName { get; set; }

        [Display(Name = "Asset Status")]
        public string AssetStatusName { get; set; }

        [Display(Name = "Asset Status")]
        public string AssetStatusGroup { get; set; }

        [Display(Name = "License Key")]
        public string LicenseKeyDisplay { get; set; }

        [Display(Name = "License Key")]
        public string LicenseKeySingle { get; set; }

        public string OriginalLicenseKeySingle { get; set; }

        [Display(Name = "License Type")]
        public string LicenseTypeName { get; set; }

        [Display(Name = "End of Warranty")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = "Location")]
        public string FullLocation { get; set; }

        [Display(Name = "Location")]
        public string LocationCode { get; set; }

        [Display(Name = "Assigned User")]
        public string AssignedUserName { get; set; }

        public string AssetNameOrSerial { get; set; }

        public string AssetNameOrSerialAndUser { get; set; }

        public string PreviousUrl { get; set; }

        [Display(Name = "Connected Assets")]
        public List<string> ConnectedAssetNames { get; set; }

        public string ConnectedAssetSerials { get; set; }

        public string ConnectedAssetNameAndAssetCategory { get; set; }

        [Display(Name = "Confirmed Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? AssetConfirmedDate { get; set; }

        public bool? HasItBeen1Year { get; set; }

        public int? AssetConfirmedById { get; set; }

        public bool? AbleToConfirm { get; set; }

        [Display(Name = "Confirmed By")]
        public string AssetConfirmedByName { get; set; }

        public UserAssetConfirmationVM UserAssetConfirmation { get; set; }

        ///

        public IEnumerable<InvoiceItemVM> InvoiceItems { get; set; }
        public IEnumerable<AssetStatusVM> Statuses { get; set; }
        public IEnumerable<LocationVM> Locations { get; set; }
        public IEnumerable<BuildingVM> Buildings { get; set; }
        public IEnumerable<RoomVM> Rooms { get; set; }
        public IEnumerable<AssetVM> Assets { get; set; }
        public IEnumerable<AssetVM> ConnectedAssets { get; set; }
        public IEnumerable<AssetVM> ComputerTypeAssets { get; set; }
        public IEnumerable<UserVM> Users { get; set; }
        public IEnumerable<AuditLogVM> AuditLogs { get; set; }

    }
}