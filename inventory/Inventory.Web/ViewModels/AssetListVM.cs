
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Web.ViewModels
{
    public class AssetListVM
    {
        public int Id { get; set; }

        public string AssetIds { get; set; }

        [RegularExpression("^[a-zA-Z0-9' '_/&.-]*$", ErrorMessage = "Only AlphaNumeric, and '&-/.' allowed")]
        public string Name { get; set; }

        public string OriginalName { get; set; }

        public string Description { get; set; }

        public string OriginalDescription { get; set; }

        public int? AssetTypeId { get; set; }

        [Display(Name = "Asset Type")]
        public AssetTypeVM AssetType { get; set; }

        public int? OriginalAssetTypeId { get; set; }

        [Display(Name = "Asset Type")]
        public string AssetTypeName { get; set; }

        public bool? Shared { get; set; }

        public bool? IsOwner { get; set; }

        public int? UserId { get; set; }

        public UserVM User { get; set; }

        public string UserFullName { get; set; }

        [Display(Name = "Item Count")]
        public int? ItemCount { get; set; }

        public string PreviousUrl { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public IEnumerable<AssetTypeVM> AssetTypes { get; set; }
    }
}