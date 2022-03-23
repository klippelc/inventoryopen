using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Web.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }

        [RegularExpression("^[a-zA-Z0-9' '_/&.-]*$", ErrorMessage = "Only AlphaNumeric, and '&-/.' allowed")]
        public string Name { get; set; }

        public string OriginalName { get; set; }

        [RegularExpression("^[a-zA-Z0-9' '_/&.-]*$", ErrorMessage = "Only AlphaNumeric, and '&-/.' allowed")]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        public string OriginalDisplayName { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

        public int? AssetTypeId { get; set; }

        public int? OriginalAssetTypeId { get; set; }

        [Display(Name = "Product Type")]
        public AssetTypeVM AssetType { get; set; }

        public int? AssetCategoryId { get; set; }

        public int? OriginalAssetCategoryId { get; set; }

        [Display(Name = "Product Category")]
        public AssetCategoryVM AssetCategory { get; set; }

        public int? ManuId { get; set; }

        public int? OriginalManuId { get; set; }

        [Display(Name = "Manufacturer")]
        public ManufacturerVM Manufacturer { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? DateAdded { get; set; }

        public DateTime? DateModified { get; set; }

        //
        [Display(Name = "Product Type")]
        public string AssetTypeName { get; set; }

        [Display(Name = "Product Category")]
        public string AssetCategoryName { get; set; }

        [Display(Name = "Manufacturer")]
        public string ManufacturerName { get; set; }

        public int? InvoiceItemCount { get; set; }

        public string PreviousUrl { get; set; }

        //

        public IEnumerable<AssetTypeVM> AssetTypes { get; set; }
        public IEnumerable<AssetCategoryVM> AssetCategories { get; set; }
        public IEnumerable<ManufacturerVM> Manufacturers { get; set; }

    }
}