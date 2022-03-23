using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Web.ViewModels
{
    public class InvoiceItemVM
    {
        public int Id { get; set; }

        public int? InvoiceId { get; set; }

        public int? OriginalInvoiceId { get; set; }

        [Display(Name = "Purchase Order")]
        public InvoiceVM Invoice { get; set; }

        [Display(Name = "Item Number")]
        public int? InvoiceItemNumber { get; set; }

        public int? AssetTypeId { get; set; }

        [Display(Name = "Asset Type")]
        public AssetTypeVM AssetType { get; set; }

        public int? AssetCategoryId { get; set; }

        public int? OriginalAssetCategoryId { get; set; }

        public string OriginalAssetCategoryName { get; set; }

        [Display(Name = "Asset Category")]
        public AssetCategoryVM AssetCategory { get; set; }

        public int? ManuId { get; set; }

        public int? OriginalManuId { get; set; }

        public ManufacturerVM Manufacturer { get; set; }

        public int? ProductId { get; set; }

        public int? OriginalProductId { get; set; }

        public ProductVM Product { get; set; }

        public int? LicenseTypeId { get; set; }

        public int? LicenseTypeIdOriginal { get; set; }

        [Display(Name = "License Type")]
        public LicenseTypeVM LicenseType { get; set; }

        public int? Quantity { get; set; }

        public int? QuantityOriginal { get; set; }

        public int? QuantityDisplay { get; set; }

        public int? Assigned { get; set; }

        public int? InActive { get; set; }

        public int? Active { get; set; }

        public int? Shared { get; set; }

        public int? Available { get; set; }

        [Display(Name = "Unit Price")]
        [RegularExpression("^\\d{0,8}(\\.\\d{2,2})?$", ErrorMessage = "The Unit Price must include two decimals.")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal? UnitPrice { get; set; }

        [Display(Name = "Expiration Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ExpirationDate { get; set; }


        //Default Values
        [Display(Name = "Date Received")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateReceived { get; set; }

        [Display(Name = "Serials")]
        [DataType(DataType.MultilineText)]
        public string Serial { get; set; }

        [Display(Name = "License Key")]
        public string LicenseKeySingle { get; set; }

        public string OriginalLicenseKeySingle { get; set; }

        [Display(Name = "License Keys")]
        [DataType(DataType.MultilineText)]
        public string LicenseKeyMulti { get; set; }

        public int? AssetLocationId { get; set; }

        public LocationVM AssetLocation { get; set; }

        public int? AssetBuildingId { get; set; }

        public BuildingVM AssetBuilding { get; set; }

        public int? AssetRoomId { get; set; }

        public BuildingVM AssetRoom { get; set; }

        public int? AssignedAssetId { get; set; }

        [Display(Name = "Assigned Asset")]
        public AssetVM AssignedAsset { get; set; }

        public int? AssignedUserId { get; set; }

        [Display(Name = "Assigned User")]
        public UserVM AssignedUser { get; set; }

        [Display(Name = "Supplier")]
        public string SupplierName { get; set; }

        //////////////////////////////////

        [DataType(DataType.MultilineText)]
        public string Specifications { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }

        //
        [Display(Name = "Asset Type")]
        public string AssetTypeName { get; set; }

        [Display(Name = "License Type")]
        public string LicenseTypeName { get; set; }

        public string InvoiceNumberAndItemNumber { get; set; }

        public string PreviousUrl { get; set; }

        public IEnumerable<InvoiceVM> Invoices { get; set; }
        public IEnumerable<SupplierVM> Suppliers { get; set; }
        public IEnumerable<AssetTypeVM> AssetTypes { get; set; }
        public IEnumerable<LicenseTypeVM> LicenseTypes { get; set; }
        public IEnumerable<AssetCategoryVM> AssetCategories { get; set; }
        public IEnumerable<ManufacturerVM> Manufacturers { get; set; }
        public IEnumerable<ProductVM> Products { get; set; }
        public IEnumerable<LocationVM> Locations { get; set; }
        public IEnumerable<BuildingVM> Buildings { get; set; }
        public IEnumerable<RoomVM> Rooms { get; set; }
        public IEnumerable<UserVM> Users { get; set; }
        public IEnumerable<AssetVM> Assets { get; set; }
    }
}