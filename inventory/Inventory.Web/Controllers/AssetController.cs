using Inventory.Web.Common;
using Inventory.Web.Logic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Inventory.Web.Controllers
{
    public class AssetController : AssetBaseMvcController
    {
        //Constructor
        public AssetController(IAssetLogic assetLogic, ILocationLogic locationLogic, IBuildingLogic buildingLogic, IRoomLogic roomLogic, 
            IManuLogic manuLogic, IProductLogic productLogic, ISupplierLogic supplierLogic, IUserLogic userLogic)
        {
            AssetLogic = assetLogic;
            LocationLogic = locationLogic;
            BuildingLogic = buildingLogic;
            RoomLogic = roomLogic;
            ManuLogic = manuLogic;
            ProductLogic = productLogic;
            SupplierLogic = supplierLogic;
            UserLogic = userLogic;
        }

        public async Task<JsonResult> Suppliers()
        {
            var vmList = await SupplierLogic.GetSuppliers();

            return Json(vmList);
        }

        public async Task<JsonResult> ComputerTypeDevices(int? connectedAssetId)
        {
            var vmList = await AssetLogic.GetComputerTypeAssets(connectedAssetId);
            var vmComputerTypeList = vmList.Select(x => new { x.Id, x.Name });

            return Json(vmComputerTypeList, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> CheckDrawer(string drawer, string originalDrawer)
        {
            bool itemExist = false;
            if (drawer != originalDrawer)
            {
                itemExist = await AssetLogic.CheckDrawer(drawer);
            }

            return Json(!itemExist, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> CheckAssetTag(string assettag, string originalAssettag)
        {
            bool itemExist = false;
            if (assettag != originalAssettag)
            {
                itemExist = await AssetLogic.CheckAssetTag(assettag);
            }

            return Json(!itemExist, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> CheckSerial(string serial, string originalSerial, int manuId)
        {
            bool itemExist = false;
            if (serial != originalSerial)
            {
                itemExist = await AssetLogic.CheckSerial(serial, manuId);
            }

            return Json(!itemExist, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> CheckIPAddress(string ipaddress, string originalIPAddress)
        {
            bool itemExist = false;
            if (ipaddress != originalIPAddress)
            {
                itemExist = await AssetLogic.CheckIPAddress(ipaddress);
            }

            return Json(!itemExist, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> CheckMacAddress(string macaddress, string originalMacaddress)
        {
            bool itemExist = false;
            if (macaddress != originalMacaddress)
            {
                itemExist = await AssetLogic.CheckMacAddress(macaddress);
            }

            return Json(!itemExist, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> AssetCategories(int? assetTypeId)
        {
            var vmList = await AssetLogic.GetAssets(assetTypeId);

            var vmCatList = vmList.GroupBy(x => new { x.InvoiceItem?.AssetCategory?.Id, x.InvoiceItem?.AssetCategory?.Name }).Select(x => new { x.Key.Id, x.Key.Name });

            return Json(vmCatList);
        }

        public async Task<JsonResult> LocationAliasCheck(int? locId, string alias)
        {
            var aliasCheck = await LocationLogic.AliasExist(locId, alias);
            return Json(aliasCheck, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetUserDefaultLocation(int? userId)
        {
            var location = await UserLogic.GetUserDefaultLocation(userId ?? 0);
            return Json(location, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> ConnectedAsset(int? assetId)
        {
            var vmAsset = await AssetLogic.GetConnectedAsset(assetId ?? 0);

            return Json(vmAsset, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> AssetAssignedUsers(int? assetTypeId)
        {
            var vmList = await AssetLogic.GetAssets(assetTypeId);

            var vmCatList = vmList.GroupBy(x => new { x.AssignedUser?.Id, x.AssignedUser?.Name }).Select(x => new { x.Key.Id, x.Key.Name });

            return Json(vmCatList);
        }

        public async Task<JsonResult> AssetAssignedAssets(int? assetTypeId)
        {
            var vmList = await AssetLogic.GetAssets(assetTypeId);

            var vmCatList = vmList.GroupBy(x => new { x.AssignedAsset?.Id, x.AssignedAsset?.Serial }).Select(x => new { x.Key.Id, x.Key.Serial });

            return Json(vmCatList);
        }

        public async Task<JsonResult> AssetLocations(int? assetTypeId)
        {
            var vmList = await AssetLogic.GetAssets(assetTypeId);

            var vmCatList = vmList.GroupBy(x => new { x.Location?.Id, x.Location?.Name }).Select(x => new { x.Key.Id, x.Key.Name });

            return Json(vmCatList);
        }

        public async Task<JsonResult> AssetAssignedLocations(int? assetTypeId)
        {
            var vmList = await AssetLogic.GetAssets(assetTypeId);

            var vmCatList = vmList.GroupBy(x => new { x.AssignedAsset?.Location?.Id, x.AssignedAsset?.Location?.Name }).Select(x => new { x.Key.Id, x.Key.Name });

            return Json(vmCatList);
        }

        public async Task<JsonResult> Categories(int? assetTypeId)
        {
            var vmList = await AssetLogic.GetAssetCategoriesByType(assetTypeId);

            return Json(vmList);
        }

        public async Task<JsonResult> Manufacturers(int? assetTypeId, int? assetCategoryId)
        {
            var vmList = await ManuLogic.GetManufacturers(assetTypeId, assetCategoryId);

            return Json(vmList);
        }

        public async Task<JsonResult> Products(int? manuId, int? assetTypeId, int? assetCategoryId)
        {
            var vmList = await ProductLogic.GetProducts(manuId, assetTypeId, assetCategoryId);

            return Json(vmList);
        }

        public async Task<JsonResult> Buildings(int? locationId)
        {
            var vmList = await BuildingLogic.GetBuildings(locationId);

            return Json(vmList);
        }

        public async Task<JsonResult> Rooms(int? buildingId)
        {
            var vmList = await RoomLogic.GetRooms(buildingId);

            return Json(vmList);
        }
    }
}