using Inventory.Data.Models;
using Inventory.Web.Common;
using Inventory.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Web.Logic
{
    public interface IAssetLogic
    {
        Task<IEnumerable<PagedListVM>> GetAssetList(Parameters parameters);

        Task<IEnumerable<AssetVM>> GetAssets(string assetType, string status);

        Task<IEnumerable<AssetVM>> GetAssets(int? assetTypeId, string status = null, bool activeOnly = false);

        Task<AssetBulkVM> GetBulkAssets(string assets, string assetType, int LoginUserLocId = 0, bool isAdmin = false);

        Task<AssetBulkVM> UpdateBulkAssets(AssetBulkVM vm, string assetType);

        Task<object> GetAssetCounts();

        Task CreateAssets(AssetVM vm, int? quantity);

        Task<AssetVM> GetAsset(int Id, string assetType);

        Task SaveAsset(AssetVM vm);

        Task DeleteAsset(AssetVM vm);

        Task<IEnumerable<AssetVM>> GetConnectedAssets(int AssetId);

        Task<AssetVM> GetConnectedAsset(int Id);

        Task<IEnumerable<AssetVM>> GetComputerTypeAssets(int? connectedAssetId, bool onlyActive = true, bool hasSerial = true, int locationId = 0);

        Task<AssetTypeVM> GetAssetType(int assetTypeId);

        Task<AssetTypeVM> GetAssetType(string assetTypeName);

        Task<IEnumerable<AssetCategoryVM>> GetAssetCategories();

        Task<IEnumerable<AssetCategoryVM>> GetAssetCategoriesByType(int? assetTypeId);

        Task<IEnumerable<AssetStatusVM>> GetAssetStatuses(string assetType, string licenseType);

        Task<AssetStatusVM> GetAssetStatus(int statusId);

        Task<IEnumerable<LicenseTypeVM>> GetLicenseTypes();

        Task<LicenseTypeVM> GetLicenseType(int licenseTypeId);

        Task<string> CheckSerials(List<string> serials, int manuId);

        Task<bool> CheckSerial(string serial, int manuId);

        Task<string> CheckLicenses(List<string> licenses, string licenseType);

        Task<bool> CheckAssetTag(string assetTag);

        Task<bool> CheckMacAddress(string macAddress);

        Task<bool> CheckDrawer(string drawer);

        Task<bool> CheckIPAddress(string ipaddress);

        Task<string> GetAssetLicenseType(int? invoiceItemId);

        Task RemoveAssignedAsset(int assetId, int? userId);

        Task UpdateConnectedAssets(Asset asset);

        Task RemoveConnectedAssets(int assetId, int? userId);
    }
}