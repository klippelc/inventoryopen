using System.Collections.Generic;

namespace Inventory.Web.ViewModels
{
    public class PagedListVM
    {
        public IEnumerable<InvoiceItemVM> InvoiceItems { get; set; }
        public IEnumerable<AssetVM> Assets { get; set; }
        public IEnumerable<AssetTypeVM> AssetTypes { get; set; }
        public IEnumerable<AssetCategoryVM> AssetCategories { get; set; }
        public IEnumerable<AssetStatusVM> AssetStatuses { get; set; }
        public IEnumerable<LicenseTypeVM> LicenseTypes { get; set; }
        public IEnumerable<LocationVM> Locations { get; set; }
        public IEnumerable<BuildingVM> Buildings { get; set; }
        public IEnumerable<RoomVM> Rooms { get; set; }
        public IEnumerable<ManufacturerVM> Manufacturers { get; set; }
        public IEnumerable<ProductVM> Products { get; set; }
        public IEnumerable<UserVM> UsersManaged { get; set; }
        public IEnumerable<AssetListVM> AssetList { get; set; }
        public bool ConnectedAsset { get; set; }
    }
}