using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using PagedList;
using Inventory.Data.Models;
using Inventory.Data.Services;
using Inventory.Web.Common;
using Inventory.Web.ViewModels;
using PagedList.EntityFramework;
using System.Dynamic;
using System;

namespace Inventory.Web.Logic
{
    public class AssetLogic : AssetBaseLogic, IAssetLogic
    {

        //Constructor
        public AssetLogic(InventoryDbContext db, ILocationLogic locationLogic,
            IBuildingLogic buildingLogic, IRoomLogic roomLogic, IUserLogic userLogic, IAssetListLogic assetListLogic)
        {
            Db = db;
            LocationLogic = locationLogic;
            BuildingLogic = buildingLogic;
            RoomLogic = roomLogic;
            UserLogic = userLogic;
            AssetListLogic = assetListLogic;
        }

        //Get Assets List
        public async Task<IEnumerable<PagedListVM>> GetAssetList(Parameters parameters)
        {
            var activeStatus = parameters.ActiveStatus;
            var assetType = parameters.AssetType;
            var assetId = parameters.AssetId;
            var showConnectedAssets = parameters.ShowConnectedAssets;
            var invoiceItemId = parameters.InvoiceItemId;
            var locationId = parameters.LocationId;
            var buildingId = parameters.BuildingId;
            var roomId = parameters.RoomId;
            var assetCategory = parameters.AssetCategory;
            var manuId = parameters.ManuId;
            var productId = parameters.ProductId;
            var licenseType = parameters.LicenseType;
            var searchString = parameters.SearchString;
            var LoginUserId = parameters.LoginUserId;
            var LoginUserLocId = parameters.LoginUserLocId ?? 0;
            var Roles = parameters.Roles;
            var sortOrder = parameters.SortOrder;
            var export = parameters.Export;
            var page = (export ?? false) ? 1 : parameters.CurrentPage;
            var pageSize = (export ?? false) ? 9999 : parameters.PageSize;
            var assetListId = parameters.AssetListId;

            var vmPagedList = new List<PagedListVM>();
            var vmPaged = new PagedListVM();
            var vmAssetList = new List<AssetVM>();
            var vmLocationList = new List<LocationVM>();
            var vmBuildingList = new List<BuildingVM>();
            var vmRoomList = new List<RoomVM>();
            var vmAssetCategoryList = new List<AssetCategoryVM>();
            var vmLicenseTypeList = new List<LicenseTypeVM>();
            var vmAssetStatusList = new List<AssetStatusVM>();
            var vmManufacturerList = new List<ManufacturerVM>();
            var vmProductList = new List<ProductVM>();
            var locationUserIds = new List<int>();

            var query = Db.Assets
               .Include(x => x.AssignedUser)
               .Include(x => x.AssignedAsset)
               .Include(x => x.AssignedAsset.Location)
               .Include(x => x.InvoiceItem.Invoice)
               .Include(x => x.InvoiceItem.Invoice.Supplier)
               .Include(x => x.InvoiceItem.AssetType)
               .Include(x => x.InvoiceItem.AssetCategory)
               .Include(x => x.InvoiceItem.Manufacturer)
               .Include(x => x.InvoiceItem.Product)
               .Include(x => x.InvoiceItem.LicenseType)
               .Include(x => x.Status)
               .Include(x => x.Location)
               .Include(x => x.Building)
               .Include(x => x.Room)
               .Include(x => x.AssetUserLists.Select(al => al.AssetList))
               .Include(x => x.ConnectedAsset.InvoiceItem.AssetCategory)
               .Include(x => x.ConnectedAssets.Select(ac => ac.InvoiceItem.AssetCategory))
               .Include(x => x.UserAssetConfirmations)
               .Where(x => x.IsDeleted == false && x.InvoiceItem.AssetType.Name.ToLower() == assetType.ToLower());

            if (activeStatus == 0)
            {
                query = query.Where(s => s.Status.Group == "Active");
            }
            else if (activeStatus == 99)
            {
                query = query.Where(s => s.Status.Group == "InActive");
            }
            else if (activeStatus > 0 && activeStatus < 99)
            {
                query = query.Where(s => s.Status.Id == activeStatus);
            }

            if (assetId != null && showConnectedAssets == true)
            {
                query = query.Where(s => s.Id == assetId || s.ConnectedAssetId == assetId);
            }
            else if (assetId != null)
            {
                query = query.Where(s => s.Id == assetId);
            }
            else if (showConnectedAssets == true)
            {
                query = query.Where(s => s.ConnectedAssetId != null);
            }

            if (invoiceItemId != null)
            {
                query = query.Where(s => s.InvoiceItemId == invoiceItemId);
            }

            if (!string.IsNullOrEmpty(assetCategory))
            {
                query = query.Where(s => (s.InvoiceItem.AssetCategory != null 
                                       && s.InvoiceItem.AssetCategory.Name.ToLower() == assetCategory.ToLower()));
            }

            if (manuId != null)
            {
                query = query.Where(s => s.InvoiceItem.ManuId == manuId);
            }

            if (productId != null)
            {
                query = query.Where(s => s.InvoiceItem.ProductId ==  productId);
            }

            if (locationId != null)
            {
                query = query.Where(s => s.LocationId == locationId);
            }

            if (buildingId != null)
            {
                query = query.Where(s => s.BuildingId == buildingId);
            }

            if (roomId != null)
            {
                query = query.Where(s => s.RoomId == roomId);
            }

            if (Roles.Contains("ManageParkAssets") && (!Roles.Contains("HardwareView")) && (assetType == "Hardware"))
            {
                query = query.Where(s => s.LocationId == LoginUserLocId && s.Status.Group == "Active");
            }

            if ((Roles.Contains("ManageParkAssets")) && (!Roles.Contains("SoftwareView")) && (assetType == "Software"))
            {
                var userList = await UserLogic.GetUsersByLocation(LoginUserLocId);
                locationUserIds = userList.Select(x => x.Id).ToList();

                query = query.Where(s => s.Status.Group == "Active" && ((s.AssignedAsset != null && s.AssignedAsset.Location != null && s.AssignedAsset.LocationId == LoginUserLocId)
                                         || (s.AssignedUser != null && locationUserIds.Contains(s.AssignedUser.Id))));
            }

            if ((Roles.Contains("ManageParkAssets")) && (assetType == "Software"))
            {
                var userList = await UserLogic.GetUsersByLocation(LoginUserLocId);
                locationUserIds = userList.Select(x => x.Id).ToList();
            }

            if ((Roles.Contains("ManageParkAssets")) && (!Roles.Contains("SoftwareView")) && (assetType == "Software"))
            {
                query = query.Where(s => (s.AssignedAsset != null && s.AssignedAsset.Location != null && s.AssignedAsset.LocationId == LoginUserLocId) 
                                         || s.AssignedUser != null && locationUserIds.Contains(s.AssignedUser.Id));
            }

            if (!string.IsNullOrEmpty(licenseType) && (assetType.Equals("Software")))
            {
                query = query.Where(s => (s.InvoiceItem.LicenseType != null
                                       && s.InvoiceItem.LicenseType.Name.ToLower() == licenseType.ToLower()));
            }

            if ((assetListId != null) && (((Roles.Contains("HardwareView")) && (assetType == "Hardware")) || ((Roles.Contains("SoftwareView")) && (assetType == "Software"))))
            {
                query = query.Where(s => s.AssetUserLists.Any(x => x.AssetListId == assetListId && ((x.AssetList.UserId == LoginUserId) || (x.AssetList.Shared == true))));
            }

            if (!string.IsNullOrEmpty(searchString) && (assetType.Equals("Hardware")))
            {
                var search = searchString?.Trim();
                var FullName = search?.Split(' ').ToList();
                var FirstName = FullName.Count > 0 ? FullName[0] : "";
                var LastName = FullName.Count > 1 ? FullName[1] : "";

                if (searchString.Contains("-"))
                {
                    search = searchString?.RemoveSpaces();
                    var phone = search.Length > 3 ? search.Substring(search.Length - 4) : search;

                    var searchList = search?.Split('-').ToList();
                    var invoiceNumber = searchList.Count > 0 ? searchList[0] : "";
                    var invoiceItemNumber = searchList.Count > 1 ? searchList[1] : "";
                    var serialNumber = searchList.Count > 2 ? searchList[2] : "";

                    query = query.Where(s => ((s.InvoiceItem.Invoice != null)
                                        && (s.InvoiceItem.Invoice.Id.ToString() == invoiceNumber )
                                        && (s.InvoiceItem.InvoiceItemNumber.ToString() == invoiceItemNumber)
                                        || (s.InvoiceItem.Invoice.PONumber.ToLower() == search.ToLower()))
                                        || (s.Name != null && s.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Description != null && s.Description.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Notes != null && s.Notes.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssetTag != null && s.AssetTag.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Serial != null && s.Serial.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Serial != null && s.Serial.ToString().ToLower() == serialNumber.ToLower())
                                        || (s.SNFnumber != null && s.SNFnumber.ToLower().Contains(search.ToLower()))
                                        || (s.MacAddress != null && s.MacAddress.ToString().ToLower() == search.ToLower())
                                        || (s.IPAddress != null && s.IPAddress.ToString().ToLower() == search.ToLower())
                                        || (s.AssignedUser != null && s.AssignedUser.LastName != null && s.AssignedUser.LastName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.FirstName != null && s.AssignedUser.FirstName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.Phone != null && s.AssignedUser.Phone.Trim().Length > 3 && s.AssignedUser.Phone.Substring(s.AssignedUser.Phone.Length - 4) == phone)
                                        || (s.AssignedUser != null && s.AssignedUser.Phone != null && s.AssignedUser.Phone.ToString() == search)
                                        || (s.AssignedUser != null && s.AssignedUser.MobilePhone != null && s.AssignedUser.MobilePhone.Trim().Length > 3 && s.AssignedUser.MobilePhone.Substring(s.AssignedUser.MobilePhone.Length - 4) == phone)
                                        || (s.AssignedUser != null && s.AssignedUser.MobilePhone != null && s.AssignedUser.MobilePhone.ToString() == search)
                                        || (s.Location != null && s.Location.Name != null && s.Location.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.DisplayName != null && s.Location.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.Code != null && s.Location.Code.ToLower().Contains(search.ToLower()))
                                        || (s.Building != null && s.Building.Name != null && s.Building.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Building != null && s.Building.DisplayName != null && s.Building.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.Room != null && s.Room.Name != null && s.Room.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Room != null && s.Room.DisplayName != null && s.Room.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.DisplayName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.DisplayName.ToString().ToLower().Contains(search.ToLower()))).AsQueryable();
                }
                else
                {
                    query = query.Where(s => ((s.InvoiceItem.Invoice != null)
                                        && (s.InvoiceItem.Invoice.Id.ToString() == search)
                                        && (s.InvoiceItem.InvoiceItemNumber.ToString() == search)
                                        || (s.InvoiceItem.Invoice.PONumber.ToLower() == search.ToLower()))
                                        || (s.Name != null && s.Name.ToString().ToLower() == search.ToLower())
                                        || (s.Description != null && s.Description.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Notes != null && s.Notes.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssetTag != null && s.AssetTag.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Serial != null && s.Serial.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.SNFnumber != null && s.SNFnumber.ToLower().Contains(search.ToLower()))
                                        || (s.MacAddress != null && s.MacAddress.ToString().ToLower() == search.ToLower())
                                        || (s.IPAddress != null && s.IPAddress.ToString().ToLower() == search.ToLower())
                                        || (s.Drawer != null && s.Drawer == search)
                                        || (s.AssignedUser != null && s.AssignedUser.LastName != null && s.AssignedUser.LastName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.FirstName != null && s.AssignedUser.FirstName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.FirstName != null && s.AssignedUser.FirstName.ToString().ToLower().Contains(FirstName.ToLower()) 
                                            && s.AssignedUser.LastName != null && s.AssignedUser.LastName.ToString().ToLower().Contains(LastName.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.Phone != null && s.AssignedUser.Phone.Trim().Length > 3 && s.AssignedUser.Phone.Substring(s.AssignedUser.Phone.Length - 4) == search)
                                        || (s.AssignedUser != null && s.AssignedUser.MobilePhone != null && s.AssignedUser.MobilePhone.Trim().Length > 3 && s.AssignedUser.MobilePhone.Substring(s.AssignedUser.MobilePhone.Length - 4) == search)
                                        || (s.AssignedUser != null && s.AssignedUser.UserName != null && s.AssignedUser.UserName.ToString().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.Name != null && s.InvoiceItem.Manufacturer.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.DisplayName != null && s.InvoiceItem.Manufacturer.DisplayName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.Name != null && s.InvoiceItem.Product.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.DisplayName != null && s.InvoiceItem.Product.DisplayName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Room != null && s.Room.Name != null && s.Room.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Room != null && s.Room.DisplayName != null && s.Room.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.Building != null && s.Building.Name != null && s.Building.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Building != null && s.Building.DisplayName != null && s.Building.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.Name != null && s.Location.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.DisplayName != null && s.Location.DisplayName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.Code != null && s.Location.Code.ToString().ToLower().Contains(search.ToLower())));
                }


            }
            else if (!string.IsNullOrEmpty(searchString) && (assetType.Equals("Software")))
            {
                var search = searchString?.Trim();
                var FullName = search?.Split(' ').ToList();
                var FirstName = FullName.Count > 0 ? FullName[0] : "";
                var LastName = FullName.Count > 1 ? FullName[1] : "";

                if (search.Contains("-"))
                {
                    search = search?.RemoveSpaces();
                    var phone = search.Length > 3 ? search.Replace("-", "").Substring(search.Length - 4) : search;
                    var searchList = search?.Split('-').ToList();
                    var invoiceNumber = searchList.Count > 0 ? searchList[0] : "";
                    var invoiceItemNumber = searchList.Count > 1 ? searchList[1] : "";

                    query = query.Where(s => ((s.InvoiceItem.Invoice != null)
                                        && (s.InvoiceItem.Invoice.Id.ToString() == invoiceNumber)
                                        && (s.InvoiceItem.InvoiceItemNumber.ToString() == invoiceItemNumber)
                                        || (s.InvoiceItem.Invoice.PONumber.ToLower() == search.ToLower()))
                                        || (s.InvoiceItem.LicenseType != null && s.InvoiceItem.LicenseType.Name.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.LicenseKeySingle.ToLower().Contains(search.ToLower()))
                                        || (s.LicenseKeyMulti.ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.LastName != null && s.AssignedUser.LastName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.FirstName != null && s.AssignedUser.FirstName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.Phone != null && s.AssignedUser.Phone.Trim().Length > 3 && s.AssignedUser.Phone.Substring(s.AssignedUser.Phone.Length - 4) == phone)
                                        || (s.AssignedUser != null && s.AssignedUser.Phone != null && s.AssignedUser.Phone.ToString() == search)
                                        || (s.AssignedUser != null && s.AssignedUser.MobilePhone != null && s.AssignedUser.MobilePhone.Trim().Length > 3 && s.AssignedUser.MobilePhone.Substring(s.AssignedUser.MobilePhone.Length - 4) == phone)
                                        || (s.AssignedUser != null && s.AssignedUser.MobilePhone != null && s.AssignedUser.MobilePhone.ToString() == search)
                                        || (s.Location != null && s.Location.Name != null && s.Location.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.DisplayName != null && s.Location.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.Code != null && s.Location.Code.ToLower().Contains(search.ToLower()))
                                        || (s.Building != null && s.Building.Name != null && s.Building.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Building != null && s.Building.DisplayName != null && s.Building.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.Room != null && s.Room.Name != null && s.Room.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Room != null && s.Room.DisplayName != null && s.Room.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.Name.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.Name.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.DisplayName.ToLower().Contains(search.ToLower())));
                }
                else
                {
                    query = query.Where(s => ((s.InvoiceItem.Invoice != null)
                                        && (s.InvoiceItem.Invoice.PONumber.ToLower() == search.ToLower()))
                                        || (s.InvoiceItem.LicenseType != null && s.InvoiceItem.LicenseType.Name.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.LicenseKeySingle.ToLower().Contains(search.ToLower()))
                                        || (s.LicenseKeyMulti.ToLower().Contains(search.ToLower()))
                                        || (s.AssignedAsset != null && s.AssignedAsset.AssetTag != null && s.AssignedAsset.AssetTag.ToString().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.LastName != null && s.AssignedUser.LastName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.FirstName != null && s.AssignedUser.FirstName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.FirstName != null && s.AssignedUser.FirstName.ToString().ToLower().Contains(FirstName.ToLower())
                                             && s.AssignedUser.LastName != null && s.AssignedUser.LastName.ToString().ToLower().Contains(LastName.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.Phone != null && s.AssignedUser.Phone.Trim().Length > 3 && s.AssignedUser.Phone.Substring(s.AssignedUser.Phone.Length - 4) == search)
                                        || (s.AssignedUser != null && s.AssignedUser.MobilePhone != null && s.AssignedUser.MobilePhone.Trim().Length > 3 && s.AssignedUser.MobilePhone.Substring(s.AssignedUser.MobilePhone.Length - 4) == search)
                                        || (s.AssignedUser != null && s.AssignedUser.UserName != null && s.AssignedUser.UserName.ToString().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.Name != null && s.Location.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.DisplayName != null && s.Location.DisplayName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.Code != null && s.Location.Code.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Building != null && s.Building.Name != null && s.Building.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Building != null && s.Building.DisplayName != null && s.Building.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.Room != null && s.Room.Name != null && s.Room.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Room != null && s.Room.DisplayName != null && s.Room.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.Name.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.Name.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.DisplayName.ToLower().Contains(search.ToLower())));
                }

            }

            switch (sortOrder)
            {
                case "name":
                    query = query.OrderBy(s => !string.IsNullOrEmpty(s.Name) ? s.Name : "zzzzzzzzz")
                                 .ThenBy(s => s.InvoiceItem.AssetCategory != null ? s.InvoiceItem.AssetCategory.Name : "")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "name_desc":
                    query = query.OrderByDescending(s => !string.IsNullOrEmpty(s.Name) ? s.Name : "")
                                 .ThenBy(s => s.InvoiceItem.AssetCategory != null ? s.InvoiceItem.AssetCategory.Name : "")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "drawer":
                    query = query.OrderBy(s => !string.IsNullOrEmpty(s.Drawer) ? s.Drawer : "zzzzzzzzz")
                                 .ThenBy(s => s.InvoiceItem.AssetCategory != null ? s.InvoiceItem.AssetCategory.Name : "")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "drawer_desc":
                    query = query.OrderByDescending(s => !string.IsNullOrEmpty(s.Drawer) ? s.Drawer : "")
                                 .ThenBy(s => s.InvoiceItem.AssetCategory != null ? s.InvoiceItem.AssetCategory.Name : "")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "connected":
                    query = query.OrderBy(s => s.ConnectedAssetId == null ? 0 : s.ConnectedAssetId)
                                 .ThenBy(s => s.InvoiceItem.AssetCategory != null ? s.InvoiceItem.AssetCategory.Name : "")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "connected_desc":
                    query = query.OrderBy(s => s.ConnectedAssetId == null ? 99999999 : s.ConnectedAssetId)
                                 .ThenBy(s => s.InvoiceItem.AssetCategory != null ? s.InvoiceItem.AssetCategory.Name : "")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "invoice":
                    query = query.OrderBy(s => s.InvoiceItem.Invoice.Id)
                                 .ThenBy(s => s.InvoiceItem.InvoiceItemNumber ?? 0)
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "invoice_desc":
                    query = query.OrderByDescending(s => s.InvoiceItem.Invoice.Id)
                                 .ThenByDescending(s => s.InvoiceItem.InvoiceItemNumber ?? 0)
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "snf":
                    query = query.OrderBy(s => !string.IsNullOrEmpty(s.SNFnumber) ? s.SNFnumber : "zzzzzzz")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "snf_desc":
                    query = query.OrderByDescending(s => !string.IsNullOrEmpty(s.SNFnumber) ? s.SNFnumber : "")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "manufacturer":
                    query = query.OrderBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name ?? "zzz" : s.InvoiceItem.Manufacturer.DisplayName : "zzz")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "manufacturer_desc":
                    query = query.OrderByDescending(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "product":
                    query = query.OrderBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name ?? "zzz" : s.InvoiceItem.Product.DisplayName : "zzz")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "");
                    break;

                case "product_desc":
                    query = query.OrderByDescending(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "");
                    break;

                case "category":
                    query = query.OrderBy(s => s.InvoiceItem.AssetCategory != null ? s.InvoiceItem.AssetCategory.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "category_desc":
                    query = query.OrderByDescending(s => s.InvoiceItem.AssetCategory.Name)
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "assettag":
                    query = query.OrderBy(s => s.AssetTag ?? "zzzzzzzzzzzzz")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "assettag_desc":
                    query = query.OrderByDescending(s => s.AssetTag)
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "serial":
                    query = query.OrderBy(s => s.Serial ?? "zzzzzzzzzzzzz")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "serial_desc":
                    query = query.OrderByDescending(s => s.Serial)
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "assigneduser":
                    query = query.OrderBy(s => s.AssignedUser != null && !string.IsNullOrEmpty(s.AssignedUser.LastName) ? s.AssignedUser.LastName ?? "zzz" : "zzz")
                                 .ThenBy(s => s.AssignedUser != null ? s.AssignedUser.FirstName : "")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "assigneduser_desc":
                    query = query.OrderByDescending(s => s.AssignedUser != null ? s.AssignedUser.LastName : "")
                                 .ThenBy(s => s.AssignedUser != null ? s.AssignedUser.FirstName : "")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "assignedlocation":
                    query = query.OrderBy(s => s.AssignedAsset != null && s.AssignedAsset.Location != null && !string.IsNullOrEmpty(s.AssignedAsset.Location.Code) ? s.AssignedAsset.Location.Code ?? "zzz" : "zzz")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "assignedlocation_desc":
                    query = query.OrderByDescending(s => s.AssignedAsset != null && s.AssignedAsset.Location != null && !string.IsNullOrEmpty(s.AssignedAsset.Location.Code) ? s.AssignedAsset.Location.Code : "")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "location":
                    query = query.OrderBy(s => s.Location != null && !string.IsNullOrEmpty(s.Location.Code) ? s.Location.Code ?? "zzz" : "zzz")
                                 .ThenBy(s => s.Building != null ? string.IsNullOrEmpty(s.Building.DisplayName) ? s.Building.Name ?? "zzz" : s.Building.DisplayName : "zzz")
                                 .ThenBy(s => s.Room != null ? string.IsNullOrEmpty(s.Room.DisplayName) ? s.Room.Name ?? "zzz" : s.Room.DisplayName : "zzz");
                    break;

                case "location_desc":
                    query = query.OrderByDescending(s => s.Location != null && !string.IsNullOrEmpty(s.Location.Code) ? s.Location.Code : "")
                                 .ThenBy(s => s.Building != null ? string.IsNullOrEmpty(s.Building.DisplayName)  ? s.Building.Name ?? "zzz" : s.Building.DisplayName : "zzz")
                                 .ThenBy(s => s.Room != null ? string.IsNullOrEmpty(s.Room.DisplayName) ? s.Room.Name ?? "zzz" : s.Room.DisplayName : "zzz");
                    break;

                case "building":
                    query = query.OrderBy(s => s.Building != null ? string.IsNullOrEmpty(s.Building.DisplayName) ? s.Building.Name ?? "zzz" : s.Building.DisplayName : "zzz")
                                 .ThenBy(s => s.Location != null ? string.IsNullOrEmpty(s.Location.DisplayName) ? s.Location.Name ?? "zzz" : s.Location.DisplayName : "zzz")
                                 .ThenBy(s => s.Room != null ? string.IsNullOrEmpty(s.Room.DisplayName) ? s.Room.Name ?? "zzz" : s.Room.DisplayName : "zzz");
                    break;

                case "building_desc":
                    query = query.OrderByDescending(s => s.Building != null ? string.IsNullOrEmpty(s.Building.DisplayName) ? s.Building.Name ?? "" : s.Building.DisplayName : "")
                                 .ThenBy(s => s.Location != null ? string.IsNullOrEmpty(s.Location.DisplayName) ? s.Location.Name ?? "zzz" : s.Location.DisplayName : "zzz")
                                 .ThenBy(s => s.Room != null ? string.IsNullOrEmpty(s.Room.DisplayName) ? s.Room.Name ?? "zzz" : s.Room.DisplayName : "zzz");
                    break;

                case "room":
                    query = query.OrderBy(s => s.Room != null ? string.IsNullOrEmpty(s.Room.DisplayName) ? s.Room.Name ?? "zzz" : s.Room.DisplayName : "zzz")
                                 .ThenBy(s => s.Location != null ? string.IsNullOrEmpty(s.Location.DisplayName) ? s.Location.Name ?? "zzz" : s.Location.DisplayName : "zzz")
                                 .ThenBy(s => s.Building != null ? string.IsNullOrEmpty(s.Building.DisplayName) ? s.Building.Name ?? "zzz" : s.Building.DisplayName : "zzz");
                    break;

                case "room_desc":
                    query = query.OrderByDescending(s => s.Room != null ? string.IsNullOrEmpty(s.Room.DisplayName) ? s.Room.Name : s.Room.DisplayName : "")
                                 .ThenBy(s => s.Location != null ? string.IsNullOrEmpty(s.Location.DisplayName) ? s.Location.Name ?? "zzz" : s.Location.DisplayName : "zzz")
                                 .ThenBy(s => s.Building != null ? string.IsNullOrEmpty(s.Building.DisplayName) ? s.Building.Name ?? "zzz" : s.Building.DisplayName : "zzz");
                    break;

                case "licensetype":
                    query = query.OrderBy(s => s.InvoiceItem.LicenseType != null ? s.InvoiceItem.LicenseType.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "licensetype_desc":
                    query = query.OrderByDescending(s => s.InvoiceItem.LicenseType != null ? s.InvoiceItem.LicenseType.Name : "")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "license":
                    query = query.OrderBy(s => s.LicenseKeyMulti ?? s.InvoiceItem.LicenseKeySingle ?? "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
                    break;

                case "license_desc":
                    query = query.OrderByDescending(s => s.LicenseKeyMulti ?? s.InvoiceItem.LicenseKeySingle ?? "");
                    break;

                case "assignedasset":
                    query = query.OrderBy(s => s.AssignedAsset != null ? s.AssignedAsset.Serial ?? "zzzzzzzzzz" : "zzzzzzzzzz")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "assignedasset_desc":
                    query = query.OrderByDescending(s => s.AssignedAsset != null ? s.AssignedAsset.Serial : "")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "status":
                    query = query.OrderBy(s => s.Status.Name)
                                  .ThenBy(s => s.InvoiceItem.AssetCategory.Name)
                                  .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                  .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "status_desc":
                    query = query.OrderByDescending(s => s.Status.Name)
                                 .ThenBy(s => s.InvoiceItem.AssetCategory.Name)
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                default:
                    query = query.OrderBy(s => s.InvoiceItem.AssetCategory.Name)
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;
            }

            var AssetsList = await query.ToPagedListAsync(page, pageSize);

            var InventoryItemList = await query.Where(x => x.InvoiceItem != null)
                                               .GroupBy(x => new { x.InvoiceItem.AssetCategory, x.InvoiceItem.LicenseType, x.InvoiceItem.Manufacturer, x.InvoiceItem.Product })
                                               .Select(x => new { x.Key.AssetCategory, x.Key.LicenseType, x.Key.Manufacturer, x.Key.Product }).ToListAsync();

            var CategoryList = InventoryItemList.Where(x => x.AssetCategory != null)
                                                .GroupBy(x => new { x.AssetCategory.Id, x.AssetCategory.Name })
                                                .Select(x => new { x.Key.Id, x.Key.Name }).ToList();

            var LicenseTypeList = InventoryItemList.Where(x => x.LicenseType != null)
                                                   .GroupBy(x => new { x.LicenseType.Id, x.LicenseType.Name })
                                                   .Select(x => new { x.Key.Id, x.Key.Name }).ToList();

            var ManufacturerList = InventoryItemList.Where(x => x.Manufacturer != null)
                                                    .GroupBy(x => new { x.Manufacturer.Id, Name = (x.Manufacturer.DisplayName != null && x.Manufacturer.DisplayName != "") ? x.Manufacturer.DisplayName : x.Manufacturer.Name })
                                                    .Select(x => new { x.Key.Id, x.Key.Name }).ToList(); 
            
            var ProductList = InventoryItemList.Where(x => x.Product != null)
                                               .GroupBy(x => new { x.Product.Id, Name = (x.Product.DisplayName != null && x.Product.DisplayName != "") ? x.Product.DisplayName : x.Product.Name })
                                               .Select(x => new { x.Key.Id, x.Key.Name }).ToList();

            var AssetStatusList = await query.Where(x => x.Status != null)
                                             .GroupBy(x => new { x.Status.Id, x.Status.Name, x.Status.Group, x.Status.Sequence })
                                             .Select(x => new { x.Key.Id, x.Key.Name, x.Key.Group, x.Key.Sequence }).ToListAsync();

            var LocationList = await query.Where(x => x.Location != null)
                                 .GroupBy(x => new { x.Location.Id, 
                                                     Name = (x.Location.Code != null && x.Location.Code != "") ? x.Location.Code : (x.Location.DisplayName != null && x.Location.DisplayName != "") ? x.Location.DisplayName : x.Location.Name })
                                 .Select(x => new { x.Key.Id, x.Key.Name }).ToListAsync();

            var BuildingList = await query.Where(x => x.Building != null)
                                 .GroupBy(x => new { x.Building.Id, Name = (x.Building.DisplayName != null && x.Building.DisplayName != "") ? x.Building.DisplayName : x.Building.Name })
                                 .Select(x => new { x.Key.Id, x.Key.Name }).ToListAsync();

            var RoomList = await query.Where(x => x.Room != null)
                     .GroupBy(x => new { x.Room.Id, Name = (x.Room.DisplayName != null && x.Room.DisplayName != "") ? x.Room.DisplayName : x.Room.Name })
                     .Select(x => new { x.Key.Id, x.Key.Name }).ToListAsync();

            var ConnectedAsset = await query.Where(x => x.ConnectedAsset != null).Select( x => x.ConnectedAsset).FirstOrDefaultAsync();

            var items = (IPagedList)AssetsList;

            foreach (var item in AssetsList)
            {
                var vm = new AssetVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Drawer = item.Drawer,
                    SNFnumber = item.SNFnumber,
                    SurplusDate = item.SurplusDate,

                    InvoiceItemId = item.InvoiceItemId,
                    InvoiceItem = item.InvoiceItem != null ? new InvoiceItemVM()
                    {
                        LicenseKeySingle = item.InvoiceItem.LicenseKeySingle,

                        InvoiceId = item.InvoiceItem.InvoiceId,
                        Invoice = item.InvoiceItem.Invoice != null ? new InvoiceVM()
                        {
                            Id = item.InvoiceItem.Invoice.Id,
                            PONumber = item.InvoiceItem.Invoice.PONumber,
                            
                            SupplierId = item.InvoiceItem.Invoice.SupplierId,
                            Supplier = item.InvoiceItem.Invoice.Supplier != null ? new SupplierVM()
                            {
                                Id = item.InvoiceItem.Invoice.Supplier.Id,
                                Name = item.InvoiceItem.Invoice.Supplier.Name,
                                DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Invoice.Supplier.DisplayName) ? item.InvoiceItem.Invoice.Supplier.DisplayName : item.InvoiceItem.Invoice.Supplier.Name,
                                Description = item.InvoiceItem.Invoice.Supplier.Description
                            } : new SupplierVM(),

                            PurchaseDate = item.InvoiceItem.Invoice.PurchaseDate,
                            TotalPrice = item.InvoiceItem.Invoice.TotalPrice
                        } : new InvoiceVM(),

                        InvoiceItemNumber = item.InvoiceItem.InvoiceItemNumber,

                        AssetTypeId = item.InvoiceItem.AssetTypeId,
                        AssetType = item.InvoiceItem.AssetType != null ? new AssetTypeVM()
                        {
                            Id = item.InvoiceItem.AssetType.Id,
                            Name = item.InvoiceItem.AssetType.Name,
                            Description = item.InvoiceItem.AssetType.Description,
                            IconCss = item.InvoiceItem.AssetType.IconCss
                        } : new AssetTypeVM(),

                        AssetCategoryId = item.InvoiceItem.AssetCategoryId,
                        AssetCategory = item.InvoiceItem.AssetCategory != null ? new AssetCategoryVM()
                        {
                            Id = item.InvoiceItem.AssetCategory.Id,
                            Name = item.InvoiceItem.AssetCategory.Name,
                            Description = item.InvoiceItem.AssetCategory.Description,
                            IconCss = item.InvoiceItem.AssetCategory.IconCss
                        } : new AssetCategoryVM(),

                        LicenseTypeId = item.InvoiceItem.LicenseTypeId,
                        LicenseType = item.InvoiceItem.LicenseType != null ? new LicenseTypeVM()
                        {
                            Id = item.InvoiceItem.LicenseType.Id,
                            Name = item.InvoiceItem.LicenseType.Name,
                            Description = item.InvoiceItem.LicenseType.Description,
                            IconCss = item.InvoiceItem.LicenseType.IconCss
                        } : new LicenseTypeVM(),

                        ManuId = item.InvoiceItem.ManuId,
                        Manufacturer = item.InvoiceItem.Manufacturer != null ? new ManufacturerVM()
                        {
                            Id = item.InvoiceItem.Manufacturer.Id,
                            Name = item.InvoiceItem.Manufacturer.Name,
                            DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Manufacturer.DisplayName) ? item.InvoiceItem.Manufacturer.DisplayName : item.InvoiceItem.Manufacturer.Name,
                            Description = item.InvoiceItem.Manufacturer.Description
                        } : new ManufacturerVM(),

                        ProductId = item.InvoiceItem.ProductId,
                        Product = item.InvoiceItem.Product != null ? new ProductVM()
                        {
                            Id = item.InvoiceItem.Product.Id,
                            Name = item.InvoiceItem.Product.Name,
                            DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Product.DisplayName) ? item.InvoiceItem.Product.DisplayName : item.InvoiceItem.Product.Name,
                            Description = item.InvoiceItem.Product.Description
                        } : new ProductVM(),

                        Quantity = 0, //Count of Assets 

                        Assigned = item.InvoiceItem.Assets != null ? item.InvoiceItem.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false && (x.AssignedUserId != null || x.AssignedAssetId != null)).Count() : 0,
                        Available = 0,
                        UnitPrice = item.InvoiceItem.UnitPrice,
                        ExpirationDate = item.InvoiceItem.ExpirationDate,
                        Notes = item.InvoiceItem.Notes,

                    } : new InvoiceItemVM(),

                    AssetTag = item.AssetTag,
                    Serial = item.Serial,
                    LicenseKeyMulti = item.LicenseKeyMulti,
                    MacAddress = item.MacAddress,

                    DateReceived = item.DateReceived,
                    LastLoginDate = item.LastLoginDate,
                    LastBootDate = item.LastBootDate,

                    AssetConfirmedDate = item.UserAssetConfirmations.Where(z => (z.UserId == item.AssignedUserId) && ((z.DateConfirmed > item.AssignedDate) || (item.AssignedDate == null))).Max(x => x.DateConfirmed),

                    StatusId = item.StatusId,
                    Status = item.Status != null ? new AssetStatusVM()
                    {
                        Id = item.Status.Id,
                        Name = item.Status.Name,
                        Group = item.Status.Group,
                        Sequence = item.Status.Sequence,
                        Description = item.Status.Description,
                        IconCss = item.Status.IconCss,
                        ColorCss = item.Status.ColorCss
                    } : new AssetStatusVM(),

                    LocationId = item.LocationId,
                    Location = item.Location != null ? new LocationVM()
                    {
                        Id = item.Location.Id,
                        Name = item.Location.Name,
                        DisplayName = !string.IsNullOrEmpty(item.Location.DisplayName) ? item.Location.DisplayName : item.Location.Name,
                        Code = item.Location.Code,
                        Description = item.Location.Description
                    } : new LocationVM(),

                    BuildingId = item.BuildingId,
                    Building = item.Building != null ? new BuildingVM()
                    {
                        Id = item.Building.Id,
                        Name = item.Building.Name,
                        DisplayName = !string.IsNullOrEmpty(item.Building.DisplayName) ? item.Building.DisplayName : item.Building.Name,
                        Description = item.Building.Description
                    } : new BuildingVM(),

                    RoomId = item.RoomId,
                    Room = item.Room != null ? new RoomVM()
                    {
                        Id = item.Room.Id,
                        Name =  item.Room.Name,
                        DisplayName = !string.IsNullOrEmpty(item.Room.DisplayName) ? item.Room.DisplayName : item.Room.Name,
                        Description = item.Room.Description
                    } : new RoomVM(),

                    AssignedAssetId = item.AssignedAssetId,
                    AssignedAsset = item.AssignedAsset != null ? new AssetVM()
                    {
                        Id = item.AssignedAsset.Id,
                        AssetTag = item.AssignedAsset.AssetTag,
                        Serial = item.AssignedAsset.Serial,
                        LicenseKeyMulti = item.AssignedAsset.LicenseKeyMulti,
                        MacAddress = item.AssignedAsset.MacAddress,

                        LocationId = item.AssignedAsset.LocationId,
                        Location = item.AssignedAsset.Location != null ? new LocationVM()
                        {
                            Id = item.AssignedAsset.Location.Id,
                            Name = item.AssignedAsset.Location.Name,
                            DisplayName = !string.IsNullOrEmpty(item.AssignedAsset.Location.DisplayName) ? item.AssignedAsset.Location.DisplayName : item.AssignedAsset.Location.Name,
                            Code = item.AssignedAsset.Location.Code,
                            Description = item.AssignedAsset.Location.Description
                        } : new LocationVM(),

                        InvoiceItemId = item.InvoiceItemId,
                        InvoiceItem = item.AssignedAsset.InvoiceItem != null ? new InvoiceItemVM()
                        {
                            Id = item.AssignedAsset.InvoiceItem.Id,

                            ManuId = item.InvoiceItem.ManuId,
                            Manufacturer = item.InvoiceItem.Manufacturer != null ? new ManufacturerVM()
                            {
                                Id = item.InvoiceItem.Manufacturer.Id,
                                Name = item.InvoiceItem.Manufacturer.Name,
                                DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Manufacturer.DisplayName) ? item.InvoiceItem.Manufacturer.DisplayName : item.InvoiceItem.Manufacturer.Name,
                                Description = item.InvoiceItem.Manufacturer.Description
                            } : new ManufacturerVM(),

                            ProductId = item.InvoiceItem.ProductId,
                            Product = item.InvoiceItem.Product != null ? new ProductVM()
                            {
                                Id = item.InvoiceItem.Product.Id,
                                Name = item.InvoiceItem.Product.Name,
                                DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Product.DisplayName) ? item.InvoiceItem.Product.DisplayName : item.InvoiceItem.Product.Name,
                                Description = item.InvoiceItem.Product.Description
                            } : new ProductVM(),

                        } : new InvoiceItemVM(),

                    } : new AssetVM(),

                    AssignedUserId = item.AssignedUserId,
                    AssignedUser = item.AssignedUser != null ? new UserVM()
                    {
                        UserId = item.AssignedUser.Id,
                        UserName = item.AssignedUser.UserName,
                        Name = item.AssignedUser.LastName + ", " + item.AssignedUser.FirstName,
                        LastNameFirstInitial = item.AssignedUser.LastName + ", " + item.AssignedUser?.FirstName[0] + ".",
                        LastNameFirstName = item.AssignedUser.LastName + ", " + item.AssignedUser?.FirstName,
                        Active = item.AssignedUser.Active
                    } : new UserVM(),

                    ConnectedAssetId = item.ConnectedAssetId,
                    OriginalConnectedAssetId = item.ConnectedAssetId,
                    ConnectedAsset = item.ConnectedAsset != null ? new AssetVM()
                    {
                        Id = item.ConnectedAsset.Id,
                        Name = item.ConnectedAsset.Name,
                        AssetNameOrSerial = item.ConnectedAsset.Name ?? item.ConnectedAsset.Serial ?? null,
                        AssetTag = item.ConnectedAsset.AssetTag,
                        Serial = item.ConnectedAsset.Serial,
                        LicenseKeyMulti = item.ConnectedAsset.LicenseKeyMulti,
                        MacAddress = item.ConnectedAsset.MacAddress,

                        InvoiceItem = item.ConnectedAsset.InvoiceItem != null ? new InvoiceItemVM()
                        {
                            AssetCategory = item.ConnectedAsset.InvoiceItem.AssetCategory != null ? new AssetCategoryVM()
                            {
                                Id = item.ConnectedAsset.InvoiceItem.AssetCategory.Id,
                                Name = item.ConnectedAsset.InvoiceItem.AssetCategory.Name,
                                Description = item.ConnectedAsset.InvoiceItem.AssetCategory.Description,
                                IconCss = item.ConnectedAsset.InvoiceItem.AssetCategory.IconCss
                            } : new AssetCategoryVM(),

                        } : new InvoiceItemVM(),

                    } : new AssetVM(),

                    ConnectedAssetNameAndAssetCategory = (item.ConnectedAsset != null ? (!string.IsNullOrEmpty(item?.ConnectedAsset?.Name) ? item?.ConnectedAsset?.Name : item?.ConnectedAsset?.Serial) : "")
                                                       + (item.ConnectedAsset != null ? (!string.IsNullOrEmpty(item?.ConnectedAsset?.InvoiceItem?.AssetCategory?.Name) ? " (" + item?.ConnectedAsset?.InvoiceItem?.AssetCategory?.Name + ")" : "") : ""),
                    Notes = item.Notes,
                    Display = item.Display,

                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy,
                    DateAdded = item.DateAdded,
                    DateModified = item.DateModified
                };

                var myList = new List<AssetVM>();
                var connectedSerialList = "";
                
                if (item.ConnectedAssets != null)
                {
                    foreach (var asset in item.ConnectedAssets)
                    {
                        var myAsset = asset != null ? new AssetVM()
                        {
                            Id = asset.Id,
                            Name = asset.Name,
                            AssetNameOrSerial = asset.Name ?? asset.Serial ?? null,
                            AssetTag = asset.AssetTag,
                            Serial = asset.Serial,
                            ConnectedAssetId = asset.ConnectedAssetId,

                            InvoiceItem = asset.InvoiceItem != null ? new InvoiceItemVM()
                            {
                                AssetCategory = asset.InvoiceItem.AssetCategory != null ? new AssetCategoryVM()
                                {
                                    Id = asset.InvoiceItem.AssetCategory.Id,
                                    Name = asset.InvoiceItem.AssetCategory.Name,
                                    IconCss = asset.InvoiceItem.AssetCategory.IconCss

                                } : new AssetCategoryVM(),

                            } : new InvoiceItemVM(),

                        } : new AssetVM();

                        myList.Add(myAsset);
                        connectedSerialList = (!string.IsNullOrEmpty(connectedSerialList)) ? connectedSerialList + ", " + myAsset.Serial : myAsset.Serial;
                    };

                    vm.ConnectedAssets = myList;
                    vm.ConnectedAssetSerials = connectedSerialList;
                }

                vm.showCheckBox = (((Roles.Contains("HardwareEdit")) && (assetType == "Hardware")) || (Roles.Contains("SoftwareEdit") && (assetType == "Software"))
                                  || (Roles.Contains("ManageParkAssets")) && (((assetType == "Hardware") && (vm.LocationId == LoginUserLocId) && (vm.Status.Group == "Active"))
                                  || ((assetType == "Software") && ((vm.AssignedAsset.LocationId == LoginUserLocId) || (locationUserIds.Contains(vm.AssignedUser.UserId))))));

                vm.SupplierName = vm.InvoiceItem.Invoice.Supplier.DisplayName;
                vm.ManufacturerName = vm.InvoiceItem.Manufacturer.DisplayName;
                vm.ProductName = vm.InvoiceItem.Product.DisplayName;
                vm.AssignedUserName = vm.AssignedUser.Name;
                vm.LocationCode = vm.Location.Code;
                vm.InvoiceItem.QuantityDisplay = (vm.InvoiceItem.LicenseType != null && vm.InvoiceItem.LicenseType.Name == "Site" && vm.InvoiceItem.Quantity == null) ? 10000000 : vm.InvoiceItem.Quantity ?? 0;
                vm.InvoiceItem.Available = (vm.InvoiceItem.QuantityDisplay - vm.InvoiceItem.Assigned);
                vm.InvoiceNumber = vm.InvoiceItem.Invoice.Id;
                vm.InvoiceItemNumber = vm.InvoiceItem.InvoiceItemNumber;
                vm.InvoiceAndInvoiceItem = vm.InvoiceNumber + "-" + vm.InvoiceItemNumber;
                vm.LicenseKeySingle = vm.InvoiceItem.LicenseKeySingle;
                vm.OriginalLicenseKeySingle = vm.InvoiceItem.LicenseKeySingle;
                vm.LicenseTypeName = vm.InvoiceItem.LicenseType.Name;
                vm.LicenseKeyDisplay = vm.InvoiceItem.LicenseType.Name == "Hardware-Multi" ? vm.LicenseKeyMulti : vm.LicenseKeySingle;
                vm.AssetStatusName = vm.Status.Name;
                vm.AssetStatusGroup = vm.Status.Group;

                vmAssetList.Add(vm);
            };


            var active = new AssetStatusVM()
            {
                Id = 0,
                Name = "Active",
                Sequence = 1
            };
            vmAssetStatusList.Add(active);

            var Inactive = new AssetStatusVM()
            {
                Id = 99,
                Name = "InActive",
                Sequence = 2
            };
            vmAssetStatusList.Add(Inactive);

            foreach (var item in AssetStatusList)
            {
                var vm = new AssetStatusVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Sequence = item.Sequence + 2,
                };
                vmAssetStatusList.Add(vm);
            }

            foreach (var item in LocationList)
            {
                var vm = new LocationVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                vmLocationList.Add(vm);
            }

            foreach (var item in BuildingList)
            {
                var vm = new BuildingVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                vmBuildingList.Add(vm);
            }

            foreach (var item in RoomList)
            {
                var vm = new RoomVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                vmRoomList.Add(vm);
            }

            foreach (var item in CategoryList)
            {
                var vm = new AssetCategoryVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                vmAssetCategoryList.Add(vm);
            }

            foreach (var item in ManufacturerList)
            {
                var vm = new ManufacturerVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                vmManufacturerList.Add(vm);
            }

            foreach (var item in ProductList)
            {
                var vm = new ProductVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                vmProductList.Add(vm);
            }

            if (assetType == "Software")
            {
                foreach (var item in LicenseTypeList)
                {
                    var vm = new LicenseTypeVM()
                    {
                        Id = item.Id,
                        Name = item.Name,
                    };
                    vmLicenseTypeList.Add(vm);
                }

                vmPaged.LicenseTypes = vmLicenseTypeList;
            }

            vmPaged.Assets = vmAssetList;
            vmPaged.AssetStatuses = vmAssetStatusList;
            vmPaged.ConnectedAsset = ConnectedAsset != null;
            vmPaged.AssetCategories = vmAssetCategoryList;
            vmPaged.Locations = vmLocationList;
            vmPaged.Buildings = vmBuildingList;
            vmPaged.Rooms = vmRoomList;
            vmPaged.Manufacturers = vmManufacturerList;
            vmPaged.Products = vmProductList;
            vmPaged.AssetList = new List<AssetListVM>();

            if (((Roles.Contains("HardwareView")) && (assetType == "Hardware")) || ((Roles.Contains("SoftwareView")) && (assetType == "Software")))
            {
                vmPaged.AssetList = await AssetListLogic.GetUserLists(LoginUserId, assetType, true);
            }

            vmPagedList.Add(vmPaged);

            StaticPagedList<PagedListVM> assetList = new StaticPagedList<PagedListVM>(vmPagedList, items.PageNumber, items.PageSize, items.TotalItemCount);

            return assetList;
        }

        //Get Assets
        public async Task<IEnumerable<AssetVM>> GetAssets(string assetType, string status)
        {
            var vmAssetList = new List<AssetVM>();

            var query = Db.Assets
                          .Include(x => x.InvoiceItem.AssetType)
                          .Include(x => x.InvoiceItem.AssetCategory)
                          .Include(x => x.InvoiceItem.LicenseType)
                          .Include(x => x.AssignedUser)
                          .Include(x => x.Status)
                          .Where(s => s.IsDeleted == false)
                          .AsQueryable();

            if (!string.IsNullOrEmpty(assetType))
            {
                query = query.Where(s => s.InvoiceItem != null
                                    && s.InvoiceItem.AssetType != null
                                    && s.InvoiceItem.AssetType.Name.ToLower().Trim() == assetType.ToLower().Trim())
                             .AsQueryable();
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.Status != null && s.Status.Name.ToLower().Trim() == status.ToLower().Trim()).AsQueryable();
            }

            var assetList = await query.ToListAsync();

            foreach (var item in assetList)
            {
                var vmAsset = item != null ? new AssetVM()
                {
                    Id = item.Id,
                    Serial = item.Serial,
                    Name = item.Name,
                    AssetNameOrSerial = item.Name ?? item.Serial ?? null,
                    Description = item.Description,
                    AssetTag = item.AssetTag,

                    AssignedUserId = item.AssignedUserId,
                    AssignedUser = item.AssignedUser != null ? new UserVM()
                    {
                        Name = !string.IsNullOrEmpty(item.AssignedUser.LastName) ? item.AssignedUser.LastName + ", " + item.AssignedUser.FirstName : null
                    } : new UserVM(),


                    StatusId = item.StatusId,
                    Status = item.Status != null ? new AssetStatusVM()
                    {
                        Name = item.Status.Name
                    } : new AssetStatusVM(),

                    InvoiceItem = item.InvoiceItem != null ? new InvoiceItemVM()
                    {
                        AssetTypeId = item.InvoiceItem.AssetTypeId,
                        AssetType = item.InvoiceItem.AssetType != null ? new AssetTypeVM()
                        {
                            Id = item.InvoiceItem.AssetType.Id,
                            Name = item.InvoiceItem.AssetType.Name,
                            Description = item.InvoiceItem.AssetType.Description
                        } : new AssetTypeVM(),
                        
                    } : new InvoiceItemVM(),

                    AssetCategoryName = item?.InvoiceItem?.AssetCategory.Name

                } : new AssetVM();

                vmAsset.SerialandAssignedUser = vmAsset.Serial + (vmAsset?.AssignedUser.Name != null ? " - " + vmAsset?.AssignedUser.Name : "");
                vmAssetList.Add(vmAsset);

            };
            return vmAssetList.OrderBy(x => x.Serial).ThenBy(x => x.AssetTag);
        }

        //Get Assets
        public async Task<IEnumerable<AssetVM>> GetAssets(int? assetTypeId, string status = null, bool activeOnly = false)
        {
            var vmAssetList = new List<AssetVM>();

            var query = Db.Assets
                          .Include(x => x.InvoiceItem.AssetType)
                          .Include(x => x.InvoiceItem.AssetCategory)
                          .Include(x => x.InvoiceItem.LicenseType)
                          .Include(x => x.InvoiceItem.Manufacturer)
                          .Include(x => x.InvoiceItem.Product)
                          .Include(x => x.InvoiceItem.LicenseType)
                          .Include(x => x.AssignedUser)
                          .Include(x => x.AssignedAsset)
                          .Include(x => x.AssignedAsset.Location)
                          .Include(x => x.Status)
                          .Where(s => s.IsDeleted == false)
                          .AsQueryable();

            if (assetTypeId != null)
            {
                query = query.Where(s => s.InvoiceItem != null
                                    && s.InvoiceItem.AssetTypeId == assetTypeId).AsQueryable();
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.Status != null && s.Status.Name.ToLower().Trim() == status.ToLower().Trim()).AsQueryable();
            }

            if (activeOnly)
            {
                query = query.Where(s => s.Status != null && s.Status.Name.ToLower().Trim() == "active").AsQueryable();
            }

            var assetList = await query.ToListAsync();

            foreach (var item in assetList)
            {
                var vmAsset = item != null ? new AssetVM()
                {
                    Id = item.Id,
                    Serial = item.Serial,
                    Name = item.Name,
                    AssetTag = item.AssetTag,

                    AssignedUserId = item.AssignedUserId,
                    AssignedUser = item.AssignedUser != null ? new UserVM()
                    {
                        Id = item.AssignedUser.Id,
                        Name = !string.IsNullOrEmpty(item.AssignedUser.LastName) ? item.AssignedUser.LastName + ", " + item.AssignedUser.FirstName : null
                    } : new UserVM(),

                    AssignedAssetId = item.AssignedAssetId,
                    AssignedAsset = item.AssignedAsset != null ? new AssetVM()
                    {
                        Id = item.AssignedAsset.Id,
                        AssetTag = item.AssignedAsset.AssetTag,
                        Serial = item.AssignedAsset.Serial,
                        LicenseKeyMulti = item.AssignedAsset.LicenseKeyMulti,
                        MacAddress = item.AssignedAsset.MacAddress,

                        LocationId = item.AssignedAsset.LocationId,
                        Location = item.AssignedAsset.Location != null ? new LocationVM()
                        {
                            Id = item.AssignedAsset.Location.Id,
                            Name = item.AssignedAsset.Location.Name,
                            DisplayName = !string.IsNullOrEmpty(item.AssignedAsset.Location.DisplayName) ? item.AssignedAsset.Location.DisplayName : item.AssignedAsset.Location.Name,
                            Code = item.AssignedAsset.Location.Code,
                            Description = item.AssignedAsset.Location.Description
                        } : new LocationVM()

                    } : new AssetVM(),

                    StatusId = item.StatusId,
                    Status = item.Status != null ? new AssetStatusVM()
                    {
                        Name = item.Status.Name
                    } : null,

                    InvoiceItemId = item.InvoiceItemId,
                    InvoiceItem = item.InvoiceItem != null ? new InvoiceItemVM()
                    {
                        AssetTypeId = item.InvoiceItem.AssetTypeId,
                        AssetType = item.InvoiceItem.AssetType != null ? new AssetTypeVM()
                        {
                            Id = item.InvoiceItem.AssetType.Id,
                            Name = item.InvoiceItem.AssetType.Name,
                            Description = item.InvoiceItem.AssetType.Description
                        } : new AssetTypeVM(),

                        AssetCategoryId = item.InvoiceItem.AssetCategoryId,
                        AssetCategory = item.InvoiceItem.AssetCategory != null ? new AssetCategoryVM()
                        {
                            Id = item.InvoiceItem.AssetCategory.Id,
                            Name = item.InvoiceItem.AssetCategory.Name,
                            Description = item.InvoiceItem.AssetCategory.Description
                        } : new AssetCategoryVM(),

                        LicenseTypeId = item.InvoiceItem.LicenseTypeId,
                        LicenseType = item.InvoiceItem.LicenseType != null ? new LicenseTypeVM()
                        {
                            Id = item.InvoiceItem.LicenseType.Id,
                            Name = item.InvoiceItem.LicenseType.Name,
                            Description = item.InvoiceItem.LicenseType.Description
                        } : new LicenseTypeVM(),

                        ManuId = item.InvoiceItem.ManuId,
                        Manufacturer = item.InvoiceItem.Manufacturer != null ? new ManufacturerVM()
                        {
                            Id = item.InvoiceItem.Manufacturer.Id,
                            Name = item.InvoiceItem.Manufacturer.Name,
                            DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Manufacturer.DisplayName) ? item.InvoiceItem.Manufacturer.DisplayName : item.InvoiceItem.Manufacturer.Name,
                            Description = item.InvoiceItem.Manufacturer.Description
                        } : new ManufacturerVM(),

                        ProductId = item.InvoiceItem.ProductId,
                        Product = item.InvoiceItem.Product != null ? new ProductVM()
                        {
                            Id = item.InvoiceItem.Product.Id,
                            Name = item.InvoiceItem.Product.Name,
                            DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Product.DisplayName) ? item.InvoiceItem.Product.DisplayName : item.InvoiceItem.Product.Name,
                            Description = item.InvoiceItem.Product.Description
                        } : new ProductVM()

                    } : new InvoiceItemVM()

                } : new AssetVM();

                vmAssetList.Add(vmAsset);
            };

            return vmAssetList.OrderBy(x => x.Serial).ThenBy(x => x.AssetTag);
        }

        //Get Bulk Assets
        public async Task<AssetBulkVM> GetBulkAssets(string assets, string assetType, int LoginUserLocId = 0, bool isAdmin = false)
        {
            var assetIds = assets.Split(',').Select(Int32.Parse).ToList();
            var vmAssetBulk = new AssetBulkVM();

            var assetList = await Db.Assets
                                    .Where(x => assetIds.Contains(x.Id) 
                                                && x.IsDeleted == false 
                                                && x.InvoiceItem.AssetType.Name == assetType)
                                    .Select(x => x.Id)
                                    .ToListAsync();

            vmAssetBulk.AssetIds = assetList.ListToString();
            vmAssetBulk.AssetCount = assetList.Count();
            var statuses = await GetAssetStatuses(assetType, null);
            var Locations = await LocationLogic.GetLocations();
            vmAssetBulk.Rooms = new List<RoomVM>();
            vmAssetBulk.Buildings = new List<BuildingVM>();

            if (isAdmin)
            {
                vmAssetBulk.Users = await UserLogic.GetUsers();
                vmAssetBulk.ComputerTypeAssets = await GetComputerTypeAssets(null);
                vmAssetBulk.Locations = Locations;
                vmAssetBulk.Statuses = statuses;
            }
            else
            {
                vmAssetBulk.Users = await UserLogic.GetUsersByLocation(LoginUserLocId);
                vmAssetBulk.ComputerTypeAssets = await GetComputerTypeAssets(null,true, false, LoginUserLocId);
                vmAssetBulk.Locations = Locations.Where(x => x.Id == LoginUserLocId);
                vmAssetBulk.Statuses = statuses.Where(x => x.Group == "Active").ToList();
            }

            return vmAssetBulk;
        }

        //Update Bulk Assets
        public async Task<AssetBulkVM> UpdateBulkAssets(AssetBulkVM vm, string assetType)
        {
            var assetIds = vm.AssetIds.Split(',').Select(Int32.Parse).ToList();

            var assetStatuses = await Db.AssetStatuses.Include(x => x.AssetStatusTypes).ToListAsync();
            var vmAssetStatus = assetStatuses.Where(x => x.Id == vm.StatusId).FirstOrDefault();

            var assetList = await Db.Assets
                                    .Include(x => x.InvoiceItem.LicenseType)
                                    .Include(x => x.InvoiceItem.AssetCategory)
                                    .Include(x => x.Status.AssetStatusTypes)
                                    .Include(x => x.ConnectedAssets)
                                    .Where(x => x.IsDeleted == false
                                           && x.InvoiceItem.AssetType.Name == assetType
                                           && assetIds.Contains(x.Id))
                                    .ToListAsync();

            //so when child is connected, lock: status, assigned user, location, building, room (and use the parents: status, assigned user, loc, bldg, room)
            //when parent status changed to inactive disconnect children
            //when parent status changed to shared or available, update children, (status(shared / available), and(remove) assigned user)
            //when parent status changed to assigned, update children(status and assigned user)

            if (assetType == "Hardware")
            {
                foreach (var item in assetList)
                {
                    string itemCategory = item.InvoiceItem.AssetCategory?.Name;
                    bool computerType = ComputerTypeList.Contains(itemCategory);
                    bool child = ((!computerType) && (item.ConnectedAssetId != null));
                    bool parent = ((computerType) && (item.ConnectedAssets.Any(x => x.IsDeleted == false)));
                    bool updateChildren = false;

                    if (item != null)
                    {
                        if (vm.DateReceived != null)
                        {
                            item.DateReceived = vm.DateReceived;
                            item.ModifiedBy = vm.ModifiedBy;
                        }

                        if (vm.StatusId != null)
                        {
                            if (vmAssetStatus?.Group == "InActive")
                            {
                                item.AssignedAssetId = null;
                                item.AssignedUserId = null;
                                item.ConnectedAssetId = null;
                                item.Drawer = null;
                                item.StatusId = vm.StatusId;
                                item.SurplusDate = null;
                                item.SNFnumber = null;
                                item.AssignedDate = DateTime.Now;
                                item.ModifiedBy = vm.ModifiedBy;

                                if (vmAssetStatus?.Name == "Surplus")
                                {
                                    item.SurplusDate = vm.SurplusDate;
                                    item.SNFnumber = vm.SNFnumber;
                                }

                                if (parent)
                                {
                                    await RemoveConnectedAssets(item.Id, vm.ModifiedBy);
                                }
                            }
                            else if ((vmAssetStatus?.Group == "Active") && (!child))
                            { 
                                if (vmAssetStatus?.Name == "Available")
                                {
                                    item.AssignedAssetId = null;
                                    item.AssignedUserId = null;
                                    item.StatusId = vm.StatusId;
                                    item.SurplusDate = null;
                                    item.SNFnumber = null;
                                    item.AssignedDate = null;
                                    item.ModifiedBy = vm.ModifiedBy;
                                }
                                else if ((vmAssetStatus?.Name == "Assigned") && (vm.AssignedUserId != null))
                                {
                                    item.AssignedAssetId = null;
                                    item.StatusId = vm.StatusId;
                                    item.SurplusDate = null;
                                    item.SNFnumber = null;
                                    item.AssignedDate = DateTime.Now;
                                    item.ModifiedBy = vm.ModifiedBy;
                                }
                                else if (vmAssetStatus?.Name == "Shared")
                                {
                                    item.AssignedUserId = null;
                                    item.StatusId = vm.StatusId;
                                    item.SurplusDate = null;
                                    item.SNFnumber = null;
                                    item.AssignedDate = null;
                                    item.ModifiedBy = vm.ModifiedBy;
                                }

                                if (parent)
                                {
                                    updateChildren = true;
                                }
                            }
                        }

                        if ((vm.AssignedUserId != null) && (!child) && ((vm.StatusId == null) || (vmAssetStatus?.Name == "Assigned")))
                        {
                            item.StatusId = assetStatuses.Where(x => x.Name == "Assigned").Select(x => x.Id).FirstOrDefault();
                            item.AssignedUserId = vm.AssignedUserId;
                            item.AssignedDate = DateTime.Now;
                            item.ModifiedBy = vm.ModifiedBy;

                            if (parent)
                            {
                                updateChildren = true;
                            }
                        }

                        if ((vm.LocationId != null) && ((!child) || (vmAssetStatus?.Group == "InActive")))
                        {
                            item.LocationId = vm.LocationId;
                            item.BuildingId = vm.BuildingId;
                            item.RoomId = vm.RoomId;
                            item.ModifiedBy = vm.ModifiedBy;

                            if (parent)
                            {
                                updateChildren = true;
                            }
                        }



                        if (!string.IsNullOrEmpty(vm.Notes))
                        {
                            item.Notes = (!string.IsNullOrEmpty(item.Notes) ? item.Notes?.Trim() 
                                                                              + (item.Notes?.Substring(item.Notes.Length - 1) != "." ? ". " : " ") 
                                                                              + Environment.NewLine : "") 
                                                                              + vm.Notes?.Trim();
                            item.ModifiedBy = vm.ModifiedBy;
                        }


                        if (updateChildren)
                        {
                            await UpdateConnectedAssets(item);
                        }

                    }
                }
            }
            else if (assetType == "Software")
            {
                foreach (var item in assetList)
                {
                    string licenseType = item.InvoiceItem.LicenseType?.Name;

                    if (vm.DateReceived != null)
                    {
                        item.DateReceived = vm.DateReceived;
                        item.ModifiedBy = vm.ModifiedBy;
                    }

                    if (vm.StatusId != null)
                    {
                        if (vmAssetStatus?.Name == "Available")
                        {
                            item.AssignedAssetId = null;
                            item.AssignedUserId = null;
                            item.StatusId = vm.StatusId;
                            item.AssignedDate = null; 
                            item.ModifiedBy = vm.ModifiedBy;
                        }
                        else if ((vmAssetStatus?.Name == "Assigned") && (vm.AssignedUserId != null) && (licenseType == "User"))
                        {
                            item.AssignedAssetId = null;
                            item.StatusId = vm.StatusId;
                            item.AssignedDate = DateTime.Now;
                            item.ModifiedBy = vm.ModifiedBy;
                        }
                        else if ((vmAssetStatus?.Name == "Shared") && (vm.AssignedAssetId != null) && (licenseType != "User"))
                        {
                            item.AssignedUserId = null;
                            item.StatusId = vm.StatusId;
                            item.AssignedDate = DateTime.Now;
                            item.ModifiedBy = vm.ModifiedBy;
                        }
                    }

                    if ((vm.AssignedUserId != null) && (licenseType == "User"))
                    {
                        item.StatusId = assetStatuses.Where(x => x.Name == "Assigned").Select(x => x.Id).FirstOrDefault();
                        item.AssignedUserId = vm.AssignedUserId;
                        item.AssignedDate = DateTime.Now;
                        item.ModifiedBy = vm.ModifiedBy;
                    }

                    if ((vm.AssignedAssetId != null) && (licenseType != "User"))
                    {
                        item.StatusId = assetStatuses.Where(x => x.Name == "Shared").Select(x => x.Id).FirstOrDefault();
                        item.AssignedAssetId = vm.AssignedAssetId;
                        item.AssignedDate = DateTime.Now;
                        item.ModifiedBy = vm.ModifiedBy;
                    }

                    if (!string.IsNullOrEmpty(vm.Notes))
                    {
                        item.Notes = (!string.IsNullOrEmpty(item.Notes) ? item.Notes?.Trim()
                                                                          + (item.Notes?.Substring(item.Notes.Length - 1) != "." ? ". " : " ")
                                                                          + Environment.NewLine : "")
                                                                          + vm.Notes?.Trim();
                        item.ModifiedBy = vm.ModifiedBy;
                    }
                }
            }
            
            await Db.SaveChangesAsync();

            return vm;
        }

        //Get Assets Count
        public async Task<object> GetAssetCounts()
        {
            dynamic obj = new ExpandoObject();
            obj.AssetCount = await Db.Assets
                                     .Where(x => x.IsDeleted == false 
                                            && x.Status != null && x.Status.Group == "Active" 
                                            && x.InvoiceItem != null 
                                            && x.InvoiceItem.AssetType != null)
                                     .CountAsync();

            obj.SoftwareCount = await Db.Assets
                                        .Where(x => x.IsDeleted == false
                                               && x.Status != null && x.Status.Group == "Active"
                                               && x.InvoiceItem != null 
                                               && x.InvoiceItem.AssetType != null 
                                               && x.InvoiceItem.AssetType.Name == "Software")
                                        .CountAsync();

            obj.HardwareCount = await Db.Assets
                                        .Where(x => x.IsDeleted == false
                                               && x.Status != null && x.Status.Group == "Active"
                                               && x.InvoiceItem != null 
                                               && x.InvoiceItem.AssetType != null 
                                               && x.InvoiceItem.AssetType.Name == "Hardware").CountAsync();
            return obj;
        }

        //Get Asset
        public async Task<AssetVM> GetAsset(int Id, string assetType)
        {
            var vmAuditLogs = new List<AuditLogVM>();

            var item = await Db.Assets
                                .Include(x => x.InvoiceItem.Invoice.Supplier)
                                .Include(x => x.InvoiceItem.AssetType)
                                .Include(x => x.InvoiceItem.AssetCategory)
                                .Include(x => x.InvoiceItem.Manufacturer)
                                .Include(x => x.InvoiceItem.Product)
                                .Include(x => x.InvoiceItem.LicenseType)
                                .Include(x => x.AssignedAsset)
                                .Include(x => x.AssignedAsset.Status)
                                .Include(x => x.AssignedAsset.AssignedUser)
                                .Include(x => x.AssignedUser)
                                .Include(x => x.Status)
                                .Include(x => x.Location)
                                .Include(x => x.Building)
                                .Include(x => x.Room)
                                .Include(x => x.ConnectedAsset)
                                .Include(x => x.ConnectedAsset.AssignedUser)
                                .Include(x => x.UserAssetConfirmations)
                                .Where(x => x.IsDeleted == false 
                                       && x.InvoiceItem.AssetType.Name == assetType 
                                       && x.Id == Id).FirstOrDefaultAsync();

            if (item == null)
            {
                return null;
            }

            var vm = new AssetVM()
            {
                Id = item.Id,
                InvoiceItemId = item.InvoiceItemId,
                InvoiceItem = item.InvoiceItem != null ? new InvoiceItemVM()
                {
                    InvoiceId = item.InvoiceItem.InvoiceId,
                    Invoice = item.InvoiceItem.Invoice != null ? new InvoiceVM()
                    {
                        Id = item.InvoiceItem.Invoice.Id,
                        PONumber = item.InvoiceItem.Invoice.PONumber,

                        SupplierId = item.InvoiceItem.Invoice.SupplierId,
                        Supplier = item.InvoiceItem.Invoice.Supplier != null ? new SupplierVM()
                        {
                            Id = item.InvoiceItem.Invoice.Supplier.Id,
                            Name = item.InvoiceItem.Invoice.Supplier.Name,
                            DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Invoice.Supplier.DisplayName) ? item.InvoiceItem.Invoice.Supplier.DisplayName : item.InvoiceItem.Invoice.Supplier.Name,
                            Description = item.InvoiceItem.Invoice.Supplier.Description
                        } : new SupplierVM(),

                        PurchaseDate = item.InvoiceItem.Invoice.PurchaseDate,
                        TotalPrice = item.InvoiceItem.Invoice.TotalPrice
                    } : new InvoiceVM(),

                    InvoiceItemNumber = item.InvoiceItem.InvoiceItemNumber,

                    AssetTypeId = item.InvoiceItem.AssetTypeId,
                    AssetType = item.InvoiceItem.AssetType != null ? new AssetTypeVM()
                    {
                        Id = item.InvoiceItem.AssetType.Id,
                        Name = item.InvoiceItem.AssetType.Name,
                        Description = item.InvoiceItem.AssetType.Description
                    } : new AssetTypeVM(),

                    AssetCategoryId = item.InvoiceItem.AssetCategoryId,
                    AssetCategory = item.InvoiceItem.AssetCategory != null ? new AssetCategoryVM()
                    {
                        Id = item.InvoiceItem.AssetCategory.Id,
                        Name = item.InvoiceItem.AssetCategory.Name,
                        Description = item.InvoiceItem.AssetCategory.Description
                    } : new AssetCategoryVM(),

                    LicenseTypeId = item.InvoiceItem.LicenseTypeId,
                    LicenseType = item.InvoiceItem.LicenseType != null ? new LicenseTypeVM()
                    {
                        Id = item.InvoiceItem.LicenseType.Id,
                        Name = item.InvoiceItem.LicenseType.Name,
                        Description = item.InvoiceItem.LicenseType.Description
                    } : new LicenseTypeVM(),

                    ManuId = item.InvoiceItem.ManuId,
                    Manufacturer = item.InvoiceItem.Manufacturer != null ? new ManufacturerVM()
                    {
                        Id = item.InvoiceItem.Manufacturer.Id,
                        Name = item.InvoiceItem.Manufacturer.Name,
                        DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Manufacturer.DisplayName) ? item.InvoiceItem.Manufacturer.DisplayName : item.InvoiceItem.Manufacturer.Name,
                        Description = item.InvoiceItem.Manufacturer.Description
                    } : new ManufacturerVM(),

                    ProductId = item.InvoiceItem.ProductId,
                    Product = item.InvoiceItem.Product != null ? new ProductVM()
                    {
                        Id = item.InvoiceItem.Product.Id,
                        Name = item.InvoiceItem.Product.Name,
                        DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Product.DisplayName) ? item.InvoiceItem.Product.DisplayName : item.InvoiceItem.Product.Name,
                        Description = item.InvoiceItem.Product.Description
                    } : new ProductVM(),

                    Quantity = 0, //item.InvoiceItem.Quantity,
                    Assigned = item.InvoiceItem.Assets != null ? item.InvoiceItem.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false && (x.AssignedUserId != null || x.AssignedAssetId != null)).Count() : 0,
                    Available = 0,

                    UnitPrice = item.InvoiceItem.UnitPrice,
                    ExpirationDate = item.InvoiceItem.ExpirationDate,
                    Specifications = item.InvoiceItem.Specifications,
                    Notes = item.InvoiceItem.Notes,

                } : new InvoiceItemVM(),

                Name = item.Name,
                OriginalName = item.Name,

                Drawer = item.Drawer,
                OriginalDrawer = item.Drawer,

                Description = item.Description,
                OriginalDescription = item.Description,

                AssetTag = item.AssetTag,
                OriginalAssetTag = item.AssetTag,
                
                Serial = item.Serial,
                OriginalSerial = item.Serial,
                
                LicenseKeyMulti = item.LicenseKeyMulti,
                OriginalLicenseKeyMulti = item.LicenseKeyMulti,

                IPAddress = item.IPAddress,
                OriginalIPAddress = item.IPAddress,

                MacAddress = item.MacAddress,
                OriginalMacAddress = item.MacAddress,

                SNFnumber = item.SNFnumber,
                SurplusDate = item.SurplusDate ?? DateTime.Now,

                DefaultDateReceived = item.DateReceived != null ? item.DateReceived : DateTime.Now,
                DateReceived = item.DateReceived,

                LastLoginDate = item.LastLoginDate,
                LastBootDate = item.LastBootDate,
                AssetConfirmedDate = item.UserAssetConfirmations.Where(z => (z.UserId == item.AssignedUserId) && ((z.DateConfirmed > item.AssignedDate) || (item.AssignedDate == null))).Max(x => x.DateConfirmed),

                StatusId = item.StatusId,
                Status = item.Status != null ? new AssetStatusVM()
                {
                    Id = item.Status.Id,
                    Name = item.Status.Name,
                    Group = item.Status.Group,
                    Description = item.Status.Description
                } : new AssetStatusVM(),

                OrignialStatusId = item.StatusId,

                LocationId = item.LocationId,
                OriginalLocationId = item.LocationId,
                Location = item.Location != null ? new LocationVM()
                {
                    Id = item.Location.Id,
                    Code = item.Location.Code,
                    Name = item.Location.Name,
                    DisplayName = !string.IsNullOrEmpty(item.Location.DisplayName) ? item.Location.DisplayName : item.Location.Name,
                    Description = item.Location.Description
                } : new LocationVM(),

                BuildingId = item.BuildingId,
                OriginalBuildingId = item.BuildingId,
                Building = item.Building != null ? new BuildingVM()
                {
                    Id = item.Building.Id,
                    Name = item.Building.Name,
                    DisplayName = !string.IsNullOrEmpty(item.Building.DisplayName) ? item.Building.DisplayName : item.Building.Name,
                    Description = item.Building.Description
                } : new BuildingVM(),

                RoomId = item.RoomId,
                OriginalRoomId = item.RoomId,
                Room = item.Room != null ? new RoomVM()
                {
                    Id = item.Room.Id,
                    Name = item.Room.Name,
                    DisplayName = !string.IsNullOrEmpty(item.Room.DisplayName) ? item.Room.DisplayName : item.Room.Name,
                    Description = item.Room.Description
                } : new RoomVM(),

                AssignedAssetId = item.AssignedAssetId,
                OriginalAssignedAssetId = item.AssignedAssetId,
                AssignedAsset = item.AssignedAsset != null ? new AssetVM()
                {
                    Id = item.AssignedAsset.Id,
                    Name = item.AssignedAsset.Name,
                    Description = item.AssignedAsset.Description,
                    AssetTag = item.AssignedAsset.AssetTag,
                    Serial = item.AssignedAsset.Serial,
                    LicenseKeyMulti = item.AssignedAsset.LicenseKeyMulti,
                    MacAddress = item.AssignedAsset.MacAddress,
                    AssetNameOrSerial = !string.IsNullOrEmpty(item.AssignedAsset.Name) ? item.AssignedAsset.Name : item.AssignedAsset.Serial ?? "",
                    SerialandAssignedUser = item.AssignedAsset.Serial + (item.AssignedAsset.AssignedUser != null ? " - " + item.AssignedAsset.AssignedUser.LastName + ", " + item.AssignedAsset.AssignedUser.FirstName : ""),

                    StatusId = item.AssignedAsset.StatusId,
                    Status = item.AssignedAsset.Status != null ? new AssetStatusVM()
                    {
                        Id = item.AssignedAsset.Status.Id,
                        Name = item.AssignedAsset.Status.Name,
                        Group = item.AssignedAsset.Status.Group,
                        Description = item.AssignedAsset.Status.Description
                    } : new AssetStatusVM(),

                    AssignedUserId = item.AssignedAsset.AssignedUserId,
                    AssignedUser = item.AssignedAsset.AssignedUser != null ? new UserVM()
                    {
                        Id = item.AssignedAsset.AssignedUser.Id,
                        UserId = item.AssignedAsset.AssignedUser.Id,
                        UserName = item.AssignedAsset.AssignedUser.UserName,
                        Park = item.AssignedAsset.AssignedUser.Park,
                        Title = item.AssignedAsset.AssignedUser.Title,
                        Phone = item.AssignedAsset.AssignedUser.Phone?.ToPhoneFormat(),
                        MobilePhone = item.AssignedAsset.AssignedUser.MobilePhone?.ToPhoneFormat(),
                        Name = !string.IsNullOrEmpty(item.AssignedAsset.AssignedUser.LastName) ? item.AssignedAsset.AssignedUser.LastName + ", " + item.AssignedAsset.AssignedUser.FirstName : null,
                        Active = item.AssignedAsset.AssignedUser.Active

                    } : new UserVM(),

                    InvoiceItemId = item.InvoiceItemId,
                    InvoiceItem = item.AssignedAsset.InvoiceItem != null ? new InvoiceItemVM()
                    {
                        Id = item.AssignedAsset.InvoiceItem.Id,

                        ManuId = item.InvoiceItem.ManuId,
                        Manufacturer = item.InvoiceItem.Manufacturer != null ? new ManufacturerVM()
                        {
                            Id = item.InvoiceItem.Manufacturer.Id,
                            Name = item.InvoiceItem.Manufacturer.Name,
                            DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Manufacturer.DisplayName) ? item.InvoiceItem.Manufacturer.DisplayName : item.InvoiceItem.Manufacturer.Name,
                            Description = item.InvoiceItem.Manufacturer.Description
                        } : new ManufacturerVM(),

                        ProductId = item.InvoiceItem.ProductId,
                        Product = item.InvoiceItem.Product != null ? new ProductVM()
                        {
                            Id = item.InvoiceItem.Product.Id,
                            Name = item.InvoiceItem.Product.Name,
                            DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Product.DisplayName) ? item.InvoiceItem.Product.DisplayName : item.InvoiceItem.Product.Name,
                            Description = item.InvoiceItem.Product.Description
                        } : new ProductVM(),

                    } : new InvoiceItemVM(),

                } : new AssetVM(),

                AssignedUserId = item.AssignedUserId,
                OriginalAssignedUserId = item.AssignedUserId,
                AssignedUser = item.AssignedUser != null ? new UserVM()
                {
                    Id = item.AssignedUser.Id,
                    UserId = item.AssignedUser.Id,
                    UserName = item.AssignedUser.UserName,
                    Park = item.AssignedUser.Park,
                    Title = item.AssignedUser.Title,
                    Phone = item.AssignedUser.Phone?.ToPhoneFormat(),
                    MobilePhone = item.AssignedUser.MobilePhone?.ToPhoneFormat(),
                    Name = !string.IsNullOrEmpty(item.AssignedUser.LastName) ? item.AssignedUser.LastName + ", " + item.AssignedUser.FirstName : null,
                    NameWithUserName = item.AssignedUser.LastName + ", " + item.AssignedUser.FirstName + " (" + item.AssignedUser.UserName + ")",
                    Active = item.AssignedUser.Active

                } : new UserVM(),

                ConnectedAssetId = item.ConnectedAssetId,
                OriginalConnectedAssetId = item.ConnectedAssetId,
                ConnectedAsset = item.ConnectedAsset != null ? new AssetVM()
                {
                    Id = item.ConnectedAsset.Id,
                    Name = item.ConnectedAsset.Name,
                    AssetTag = item.ConnectedAsset.AssetTag,
                    Serial = item.Serial,
                    AssetNameOrSerial = !string.IsNullOrEmpty(item.ConnectedAsset.Name) ? item.ConnectedAsset.Name : item.ConnectedAsset.Serial ?? "",
                    SerialandAssignedUser = item.ConnectedAsset.Serial + (item.ConnectedAsset.AssignedUser != null ? " - " + item.ConnectedAsset.AssignedUser.LastName + ", " + item.ConnectedAsset.AssignedUser.FirstName : ""),
                    LicenseKeyMulti = item.ConnectedAsset.LicenseKeyMulti,
                    MacAddress = item.ConnectedAsset.MacAddress,

                    AssetTypeName = item.InvoiceItem?.AssetType?.Name,

                    AssignedUserId = item.ConnectedAsset.AssignedUserId,
                    AssignedUser = item.ConnectedAsset.AssignedUser != null ? new UserVM()
                    {
                        Id = item.ConnectedAsset.AssignedUser.Id,
                        UserId = item.ConnectedAsset.AssignedUser.Id,
                        UserName = item.ConnectedAsset.AssignedUser.UserName,
                        Park = item.ConnectedAsset.AssignedUser.Park,
                        Title = item.ConnectedAsset.AssignedUser.Title,
                        Phone = item.ConnectedAsset.AssignedUser.Phone?.ToPhoneFormat(),
                        MobilePhone = item.ConnectedAsset.AssignedUser.MobilePhone?.ToPhoneFormat(),
                        Name = !string.IsNullOrEmpty(item.ConnectedAsset.AssignedUser.LastName) ? item.ConnectedAsset.AssignedUser.LastName + ", " + item.ConnectedAsset.AssignedUser.FirstName : null,
                        Active = item.ConnectedAsset.AssignedUser.Active

                    } : new UserVM()

                } : new AssetVM(),

                Notes = item.Notes,
                Display = item.Display,

                CreatedBy = item.CreatedBy,
                ModifiedBy = item.ModifiedBy,
                DateAdded = item.DateAdded,
                DateModified = item.DateModified

            };

            vm.InvoiceItem.Available = (vm.InvoiceItem.QuantityDisplay - vm.InvoiceItem.Assigned);
            vm.InvoiceItem.QuantityOriginal = vm.InvoiceItem.Quantity;
            vm.InvoiceItem.QuantityDisplay = (vm.InvoiceItem.LicenseType != null && vm.InvoiceItem.LicenseType.Name == "Site" && vm.InvoiceItem.Quantity == null) ? 10000000 : vm.InvoiceItem.Quantity ?? 0;

            vm.PONumber = vm.InvoiceItem.Invoice.PONumber;
            vm.InvoiceNumber = vm.InvoiceItem.Invoice.Id;
            vm.InvoiceItemNumber = vm.InvoiceItem.InvoiceItemNumber;
            vm.InvoiceAndInvoiceItem = vm.InvoiceNumber + "-" + vm.InvoiceItemNumber;
            vm.SupplierName = vm.InvoiceItem.Invoice.Supplier.DisplayName;
            vm.PurchaseDate = vm.InvoiceItem.Invoice.PurchaseDate;
            vm.InvoiceItemNumber = vm.InvoiceItem.InvoiceItemNumber;
            vm.ManuId = vm.InvoiceItem.ManuId;
            vm.ManufacturerName = vm.InvoiceItem.Manufacturer.DisplayName;
            vm.ProductId = vm.InvoiceItem.Product.Id;
            vm.ProductName = vm.InvoiceItem.Product.DisplayName;
            vm.SupplierName = vm.InvoiceItem.Invoice.Supplier.DisplayName;
            vm.ProductName = vm.InvoiceItem.Product.DisplayName;
            vm.AssignedUserName = vm.AssignedUser.Name;
            vm.AssetNameOrSerialAndUser = (!string.IsNullOrEmpty(vm.Name) ? vm.Name : vm.Serial ?? "") + (vm.AssignedUser != null ? " - " + vm.AssignedUser.LastName + ", " + vm.AssignedUser.FirstName : "");
            vm.AssignedAsset.SerialandAssignedUser = vm.AssignedAsset?.Serial + (vm.AssignedAsset?.AssignedUser?.Name != null ? " - " + vm.AssignedAsset?.AssignedUser?.Name : "");
            vm.AssetTypeName = vm.InvoiceItem.AssetType.Name;
            vm.AssetCategoryName = vm.InvoiceItem.AssetCategory.Name;
            vm.AssetStatusName = vm.Status.Name;
            vm.AssetStatusGroup = vm.Status.Group;
            vm.ExpirationDate = vm.InvoiceItem.ExpirationDate;
            vm.LicenseTypeName = vm.InvoiceItem.LicenseType.Name;
            vm.LicenseKeySingle = item.InvoiceItem.LicenseKeySingle;
            vm.OriginalLicenseKeySingle = item.InvoiceItem.LicenseKeySingle;
            vm.LicenseKeyDisplay = vm.InvoiceItem.LicenseType.Name == "Hardware-Multi" ? vm.LicenseKeyMulti : vm.LicenseKeySingle;
            vm.UnitPrice = vm.InvoiceItem.UnitPrice == null ? 0 : vm.InvoiceItem.UnitPrice;
            vm.FullLocation = vm.Location.DisplayName ?? "";
            vm.FullLocation += vm.Location.DisplayName != null && vm.Building.DisplayName != null ? " / " + vm.Building.DisplayName : "";
            vm.FullLocation += vm.Location.DisplayName != null && vm.Building.DisplayName != null && vm.Room.DisplayName != null ? " / " + vm.Room.DisplayName : "";
            
            vm.Statuses = await GetAssetStatuses(assetType, vm.LicenseTypeName);
            vm.Locations = await LocationLogic.GetLocations();
            vm.Buildings = await BuildingLogic.GetBuildings(vm.LocationId ?? 0);
            vm.Rooms = await RoomLogic.GetRooms(vm.BuildingId ?? 0);
            vm.Users = await UserLogic.GetUsers();
            vm.Assets = await GetComputerTypeAssets(null);
            vm.ConnectedAssets = await GetConnectedAssets(vm.Id);
            vm.ConnectedAssetNames = vm.ConnectedAssets?.Select(x => x.ConnectedAssetNameAndAssetCategory).ToList();

            ///

            var auditList = await Db.AuditLogs.Where(x => (x.TableName == "Assets" && x.PrimaryKeyId == vm.Id)
                                                           || (x.TableName == "InvoiceItems" && x.PrimaryKeyId == vm.InvoiceItemId)).ToListAsync();
            foreach (var audit in auditList)
            {
                var vmAudit = new AuditLogVM()
                {
                    Id = audit.Id,
                    OldValue = audit.OldValue,
                    NewValue = audit.NewValue,
                    TableName = audit.TableName,
                    OperationName = audit.OperationName,
                    OperationAndDate = audit.DateModified?.ToString("MMM dd, yyyy") + " (" + audit.OperationName + ")",
                    DateModified = audit.DateModified,
                    ChangeSummary = ("(" + audit.ColumnName + ") - ") 
                                    + (!string.IsNullOrEmpty(audit.OldValue) ? audit.OldValue : "Null") 
                                    + " - "
                                    + (!string.IsNullOrEmpty(audit.NewValue) ? audit.NewValue : "Null")
                                    + " - ("
                                    + audit.ModifiedByName
                                    + ")"
                };
                vmAuditLogs.Add(vmAudit);
            }

            vm.AuditLogs = vmAuditLogs;

            return vm;

        }

        //Create Assets
        public async Task CreateAssets(AssetVM vm, int? quantity)
        {
            var status = Db.AssetStatuses.AsQueryable();

            if (vm.AssignedUserId != null && vm.AssignedAssetId == null)
            {
                status = status.Where(x => x.Name == "Assigned");
            }
            else if (vm.AssignedUserId == null && vm.AssignedAssetId != null)
            {
                status = status.Where(x => x.Name == "Shared");
            }
            else
            {
                status = status.Where(x => x.Name == "Available");
            }

            var statusId = await status.Select(x => x.Id).FirstOrDefaultAsync();

            DateTime? assignedDate = null;
            if (vm.AssignedAssetId != null || vm.AssignedUserId != null)
            {
                assignedDate = DateTime.Now;
            }

            var asset = new Asset()
            {
                InvoiceItemId = vm.InvoiceItemId,
                Name = vm.Name,
                Description = vm.Description,
                AssetTag = vm.AssetTag,
                Serial = vm.Serial,
                LicenseKeyMulti = vm.LicenseKeyMulti,
                MacAddress = vm.MacAddress,
                DateReceived = vm.DateReceived,
                StatusId = statusId,
                LocationId = vm.LocationId,
                BuildingId = vm.BuildingId,
                RoomId = vm.RoomId,
                AssignedDate = assignedDate,
                AssignedAssetId = vm.AssignedAssetId,
                AssignedUserId = vm.AssignedUserId,
                Notes = vm.Notes,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.CreatedBy
            };

            var invoiceItem = await Db.InvoiceItems
                                      .Include(x => x.AssetType)
                                      .Include(x => x.LicenseType)
                                      .Where(x => x.Id == asset.InvoiceItemId)
                                      .FirstOrDefaultAsync();

            List<string> serials = asset.Serial != null ? asset.Serial.Split(',').ToList() : new List<string>();
            List<string> licenses = asset.LicenseKeyMulti != null ? asset.LicenseKeyMulti?.Split(',').ToList() : new List<string>();

            if (invoiceItem != null)
            {
                Asset item;
                for (int i = 0; i < (quantity ?? 0); i++)
                {
                    if (invoiceItem.AssetType.Name == "Hardware")
                    {
                        item = new Asset()
                        {
                            InvoiceItemId = asset.InvoiceItemId,
                            Name = asset.Name,
                            Description = asset.Description,
                            AssetTag = null,
                            Serial = serials.Count > 0 && i < serials.Count ? serials[i].RemoveSpaces() ?? null : null,
                            LicenseKeyMulti = null,
                            DateReceived = asset.DateReceived,
                            StatusId = asset.StatusId,
                            LocationId = asset.LocationId,
                            BuildingId = asset.BuildingId,
                            RoomId = asset.RoomId,
                            AssignedAssetId = null,
                            AssignedUserId = asset.AssignedUserId,
                            AssignedDate = asset.AssignedDate,
                            Display = true,
                            IsDeleted = false,
                            CreatedBy = asset.CreatedBy,
                            ModifiedBy = asset.ModifiedBy
                        };
                        Db.Assets.Add(item);
                    }
                    else if (invoiceItem.AssetType.Name == "Software")
                    {
                        if (invoiceItem.LicenseType != null && invoiceItem.LicenseType.Name == "Hardware-Multi")
                        {
                            item = new Asset()
                            {
                                InvoiceItemId = asset.InvoiceItemId,
                                Name = asset.Name,
                                Description = asset.Description,
                                AssetTag = null,
                                Serial = null,
                                LicenseKeyMulti = licenses.Count > 0 && i < licenses.Count ? licenses[i].RemoveSpaces() ?? null : null,
                                DateReceived = asset.DateReceived,
                                StatusId = asset.StatusId,
                                LocationId = null,
                                BuildingId = null,
                                RoomId = null,
                                AssignedAssetId = asset.AssignedAssetId,
                                AssignedUserId = null,
                                AssignedDate = asset.AssignedDate,
                                Display = true,
                                IsDeleted = false,
                                CreatedBy = asset.CreatedBy,
                                ModifiedBy = asset.ModifiedBy
                            };
                            Db.Assets.Add(item);
                        }
                        else if (invoiceItem.LicenseType != null && invoiceItem.LicenseType.Name == "Hardware-Single")
                        {
                            item = new Asset()
                            {
                                InvoiceItemId = asset.InvoiceItemId,
                                Name = asset.Name,
                                Description = asset.Description,
                                AssetTag = null,
                                Serial = null,
                                LicenseKeyMulti = null,
                                DateReceived = asset.DateReceived,
                                StatusId = asset.StatusId,
                                LocationId = null,
                                BuildingId = null,
                                RoomId = null,
                                AssignedAssetId = asset.AssignedAssetId,
                                AssignedUserId = null,
                                AssignedDate = asset.AssignedDate,
                                Display = true,
                                IsDeleted = false,
                                CreatedBy = asset.CreatedBy,
                                ModifiedBy = asset.ModifiedBy
                            };
                            Db.Assets.Add(item);
                        }
                        else if (invoiceItem.LicenseType != null && invoiceItem.LicenseType.Name == "User")
                        {
                            item = new Asset()
                            {
                                InvoiceItemId = asset.InvoiceItemId,
                                Name = asset.Name,
                                Description = asset.Description,
                                AssetTag = null,
                                Serial = null,
                                LicenseKeyMulti = null,
                                DateReceived = asset.DateReceived,
                                StatusId = asset.StatusId,
                                LocationId = null,
                                BuildingId = null,
                                RoomId = null,
                                AssignedAssetId = null,
                                AssignedUserId = asset.AssignedUserId,
                                AssignedDate = asset.AssignedDate,
                                Display = true,
                                IsDeleted = false,
                                CreatedBy = asset.CreatedBy,
                                ModifiedBy = asset.ModifiedBy
                            };
                            Db.Assets.Add(item);
                        }
                    }
                }
                await Db.SaveChangesAsync();
            }
        }

        //Save Asset
        public async Task SaveAsset(AssetVM vm)
        {
            bool removeAssigned = false;
            if (vm.StatusId != null)
            {
                var status = await Db.AssetStatuses.FindAsync(vm.StatusId);
                if (status != null && status.Group?.ToLower() == "inactive")
                {
                    removeAssigned = true;
                    vm.AssignedUserId = null;
                    vm.AssignedAssetId = null;
                    vm.AssignedDate = null;
                    vm.ConnectedAssetId = null;
                    vm.Drawer = null;
                }
                else if (status != null && status.Name?.ToLower() == "assigned")
                {
                    vm.AssignedAssetId = null;
                    vm.SurplusDate = null;
                    vm.SNFnumber = null;
                }
                else if (status != null && ((status.Name?.ToLower() == "available") || (status.Name?.ToLower() == "shared")))
                {
                    vm.AssignedUserId = null;
                    vm.SurplusDate = null;
                    vm.SNFnumber = null;
                }
            }

            if (!removeAssigned & vm.ConnectedAssetId != null && !ComputerTypeList.Contains(vm.AssetCategoryName))
            {
                var connectedAsset = await GetConnectedAsset(vm.ConnectedAssetId ?? 0);

                if (connectedAsset != null)
                {
                    vm.LocationId = connectedAsset.LocationId;
                    vm.BuildingId = connectedAsset.BuildingId;
                    vm.RoomId = connectedAsset.RoomId;
                    vm.AssignedUserId = connectedAsset.AssignedUserId;
                    vm.StatusId = connectedAsset.StatusId;
                }
            }

            if ((vm.RoomId != null) && (vm.RoomId != vm.OriginalRoomId))
            {
                var room = await RoomLogic.Get(vm.RoomId ?? 0);
                if (room != null)
                {
                    vm.BuildingId = room.BuildingId;
                    vm.LocationId = room.LocationId;
                }
            }

            if ((vm.RoomId == null && vm.BuildingId != null) && (vm.BuildingId != vm.OriginalBuildingId))
            {
                var building = await BuildingLogic.Get(vm.BuildingId ?? 0);
                if (building != null)
                {
                    vm.LocationId = building.LocationId;
                }
            }

            DateTime? assignedDate = null;
            if (vm.AssignedAssetId != null || vm.AssignedUserId != null)
            {
                assignedDate = DateTime.Now;
            }

            var asset = new Asset()
            {
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description,
                Drawer = vm.Drawer,
                InvoiceItemId = vm.InvoiceItemId,
                AssetTag = vm.AssetTag,
                LicenseKeyMulti = vm.LicenseKeyMulti.RemoveSpaces(),
                Serial = vm.Serial.RemoveSpaces(),
                MacAddress = vm.MacAddress,
                IPAddress = vm.IPAddress,
                Notes = vm.Notes?.Trim(),
                DateReceived = vm.DefaultDateReceived,
                StatusId = vm.StatusId,
                SurplusDate = vm.SurplusDate,
                SNFnumber = vm.SNFnumber,
                LocationId = vm.LocationId,
                BuildingId = vm.BuildingId,
                RoomId = vm.RoomId,
                AssignedAssetId = vm.AssignedAssetId,
                AssignedUserId = vm.AssignedUserId,
                ConnectedAssetId = vm.ConnectedAssetId,
                AssignedDate = assignedDate,
                Display = vm.Display,
                ModifiedBy = vm.ModifiedBy
            };

            var entry = Db.Entry(asset);
            entry.State = EntityState.Modified;
            Db.Entry(asset).Property(x => x.CreatedBy).IsModified = false;
            Db.Entry(asset).Property(x => x.DateAdded).IsModified = false;
            Db.Entry(asset).Property(x => x.LastLoginDate).IsModified = false;
            Db.Entry(asset).Property(x => x.LastBootDate).IsModified = false;
            await Db.SaveChangesAsync();

            if(removeAssigned && vm.AssetTypeName == "Hardware")
            {
                await RemoveAssignedAsset(vm.Id, vm.ModifiedBy);
                await RemoveConnectedAssets(vm.Id, vm.ModifiedBy);
            }

            if (vm.ConnectedAssetId == null && ComputerTypeList.Contains(vm.AssetCategoryName))
            {
                await UpdateConnectedAssets(asset);
            }
        }

        //Delete Asset
        public async Task DeleteAsset(AssetVM vm)
        {
            var asset = await Db.Assets.FindAsync(vm.Id);

            if (asset != null)
            {
                if (vm.AssetTypeName == "Hardware")
                {
                    await RemoveAssignedAsset(vm.Id, vm.ModifiedBy);
                    await RemoveConnectedAssets(vm.Id, vm.ModifiedBy);
                }
                await AssetListLogic.RemoveAssetFromAllList(vm.Id);

                asset.Drawer = null;
                asset.AssignedUserId = null;
                asset.AssignedAssetId = null;
                asset.AssignedDate = null;
                asset.ConnectedAssetId = null;
                asset.LocationId = null;
                asset.BuildingId = null;
                asset.RoomId = null;
                asset.LicenseKeyMulti = null;
                asset.IsDeleted = true;
                asset.ModifiedBy = vm.ModifiedBy;

                var entry = Db.Entry(asset);
                entry.State = EntityState.Modified;
                Db.Entry(asset).Property(x => x.CreatedBy).IsModified = false;
                Db.Entry(asset).Property(x => x.DateAdded).IsModified = false;
                Db.Entry(asset).Property(x => x.LastLoginDate).IsModified = false;
                Db.Entry(asset).Property(x => x.LastBootDate).IsModified = false;
                await Db.SaveChangesAsync();
            }
        }

        //Get Asset
        public async Task<AssetVM> GetConnectedAsset(int Id)
        {
            var item = await Db.Assets
                                .Include(x => x.InvoiceItem)
                                .Include(x => x.InvoiceItem.AssetType)
                                .Include(x => x.InvoiceItem.AssetCategory)
                                .Include(x => x.InvoiceItem.Manufacturer)
                                .Include(x => x.InvoiceItem.Product)
                                .Include(x => x.AssignedUser)
                                .Include(x => x.Status)
                                .Include(x => x.Location)
                                .Include(x => x.Building)
                                .Include(x => x.Room)
                                .Where(x => x.IsDeleted == false && x.Id == Id).FirstOrDefaultAsync();


            if (item == null)
            {
                return null;
            }

            var vm = new AssetVM()
            {
                Id = item.Id,
                InvoiceItemId = item.InvoiceItemId,
                InvoiceItem = item.InvoiceItem != null ? new InvoiceItemVM()
                {
                    InvoiceId = item.InvoiceItem.InvoiceId,
                    Invoice = item.InvoiceItem.Invoice != null ? new InvoiceVM()
                    {
                        Id = item.InvoiceItem.Invoice.Id,
                        PONumber = item.InvoiceItem.Invoice.PONumber,

                        SupplierId = item.InvoiceItem.Invoice.SupplierId,
                        Supplier = item.InvoiceItem.Invoice.Supplier != null ? new SupplierVM()
                        {
                            Id = item.InvoiceItem.Invoice.Supplier.Id,
                            Name = item.InvoiceItem.Invoice.Supplier.Name,
                            DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Invoice.Supplier.DisplayName) ? item.InvoiceItem.Invoice.Supplier.DisplayName : item.InvoiceItem.Invoice.Supplier.Name,
                            Description = item.InvoiceItem.Invoice.Supplier.Description
                        } : new SupplierVM(),

                        PurchaseDate = item.InvoiceItem.Invoice.PurchaseDate,
                        TotalPrice = item.InvoiceItem.Invoice.TotalPrice
                    } : new InvoiceVM(),

                    InvoiceItemNumber = item.InvoiceItem.InvoiceItemNumber,

                    AssetTypeId = item.InvoiceItem.AssetTypeId,
                    AssetType = item.InvoiceItem.AssetType != null ? new AssetTypeVM()
                    {
                        Id = item.InvoiceItem.AssetType.Id,
                        Name = item.InvoiceItem.AssetType.Name,
                        Description = item.InvoiceItem.AssetType.Description
                    } : new AssetTypeVM(),

                    AssetCategoryId = item.InvoiceItem.AssetCategoryId,
                    AssetCategory = item.InvoiceItem.AssetCategory != null ? new AssetCategoryVM()
                    {
                        Id = item.InvoiceItem.AssetCategory.Id,
                        Name = item.InvoiceItem.AssetCategory.Name,
                        Description = item.InvoiceItem.AssetCategory.Description
                    } : new AssetCategoryVM(),

                    ManuId = item.InvoiceItem.ManuId,
                    Manufacturer = item.InvoiceItem.Manufacturer != null ? new ManufacturerVM()
                    {
                        Id = item.InvoiceItem.Manufacturer.Id,
                        Name = item.InvoiceItem.Manufacturer.Name,
                        DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Manufacturer.DisplayName) ? item.InvoiceItem.Manufacturer.DisplayName : item.InvoiceItem.Manufacturer.Name,
                        Description = item.InvoiceItem.Manufacturer.Description
                    } : new ManufacturerVM(),

                    ProductId = item.InvoiceItem.ProductId,
                    Product = item.InvoiceItem.Product != null ? new ProductVM()
                    {
                        Id = item.InvoiceItem.Product.Id,
                        Name = item.InvoiceItem.Product.Name,
                        DisplayName = !string.IsNullOrEmpty(item.InvoiceItem.Product.DisplayName) ? item.InvoiceItem.Product.DisplayName : item.InvoiceItem.Product.Name,
                        Description = item.InvoiceItem.Product.Description
                    } : new ProductVM(),

                    Quantity = 0, //item.InvoiceItem.Quantity,
                    Assigned = item.InvoiceItem.Assets != null ? item.InvoiceItem.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false && (x.AssignedUserId != null || x.AssignedAssetId != null)).Count() : 0,
                    Available = 0,

                    UnitPrice = item.InvoiceItem.UnitPrice,
                    ExpirationDate = item.InvoiceItem.ExpirationDate,
                    Specifications = item.InvoiceItem.Specifications,
                    Notes = item.InvoiceItem.Notes,

                } : new InvoiceItemVM(),

                Name = item.Name,
                Description = item.Description,
                AssetTag = item.AssetTag,
                Serial = item.Serial,
                LicenseKeyMulti = item.LicenseKeyMulti,
                MacAddress = item.MacAddress,
                
                StatusId = item.StatusId,
                Status = item.Status != null ? new AssetStatusVM()
                {
                    Id = item.Status.Id,
                    Name = item.Status.Name,
                    Description = item.Status.Description
                } : new AssetStatusVM(),

                OrignialStatusId = item.StatusId,

                LocationId = item.LocationId,
                Location = item.Location != null ? new LocationVM()
                {
                    Id = item.Location.Id,
                    Code = item.Location.Code,
                    Name = item.Location.Name,
                    DisplayName = !string.IsNullOrEmpty(item.Location.DisplayName) ? item.Location.DisplayName : item.Location.Name,
                    Description = item.Location.Description
                } : new LocationVM(),

                BuildingId = item.BuildingId,
                Building = item.Building != null ? new BuildingVM()
                {
                    Id = item.Building.Id,
                    Name = item.Building.Name,
                    DisplayName = !string.IsNullOrEmpty(item.Building.DisplayName) ? item.Building.DisplayName : item.Building.Name,
                    Description = item.Building.Description
                } : new BuildingVM(),

                RoomId = item.RoomId,
                Room = item.Room != null ? new RoomVM()
                {
                    Id = item.Room.Id,
                    Name = item.Room.Name,
                    DisplayName = !string.IsNullOrEmpty(item.Room.DisplayName) ? item.Room.DisplayName : item.Room.Name,
                    Description = item.Room.Description
                } : new RoomVM(),

                AssignedUserId = item.AssignedUserId,
                AssignedUser = item.AssignedUser != null ? new UserVM()
                {
                    Id = item.AssignedUser.Id,
                    UserId = item.AssignedUser.Id,
                    UserName = item.AssignedUser.UserName,
                    Park = item.AssignedUser.Park,
                    Title = item.AssignedUser.Title,
                    Phone = item.AssignedUser.Phone?.ToPhoneFormat(),
                    MobilePhone = item.AssignedUser.MobilePhone?.ToPhoneFormat(),
                    Name = !string.IsNullOrEmpty(item.AssignedUser.LastName) ? item.AssignedUser.LastName + ", " + item.AssignedUser.FirstName : null,
                    NameWithUserName = item.AssignedUser.LastName + ", " + item.AssignedUser.FirstName + " (" + item.AssignedUser.UserName + ")",
                    Active = item.AssignedUser.Active

                } : new UserVM(),

                Notes = item.Notes,
                Display = item.Display
            };

            return vm;
        }

        //Get Connected Assets
        public async Task<IEnumerable<AssetVM>> GetConnectedAssets(int AssetId)
        {
            var vmAssetList = new List<AssetVM>();
            var assetList = await Db.Assets
                                    .Include(x => x.InvoiceItem)
                                    .Include(x => x.InvoiceItem.AssetType)
                                    .Include(x => x.InvoiceItem.AssetCategory)
                                    .Where(x => x.IsDeleted == false && x.ConnectedAssetId == AssetId)
                                    .OrderBy(r => r.Name).ToListAsync();

            foreach (var item in assetList)
            {
                var vmAsset = item != null ? new AssetVM()
                {
                    Id = item.Id,
                    Name = !string.IsNullOrEmpty(item.Name) ? item.Name : item.Serial ?? "",
                    ConnectedAssetNameAndAssetCategory = (!string.IsNullOrEmpty(item.Name) ? item.Name : item.Serial) + (!string.IsNullOrEmpty(item.InvoiceItem?.AssetCategory?.Name) ? " (" + item.InvoiceItem?.AssetCategory?.Name + ")" : ""),
                    Description = item.Description

                } : new AssetVM();
                vmAssetList.Add(vmAsset);
            };
            return vmAssetList.OrderBy(x => x.Name);
        }

        //Get Computer Type Assets
        public async Task<IEnumerable<AssetVM>> GetComputerTypeAssets(int? connectedAssetId, bool onlyActive = true, bool hasSerial = true, int locationId = 0)
        {
            var vmAssetList = new List<AssetVM>();
            var query = Db.Assets
                          .Include(x => x.AssignedUser)
                          .Include(x => x.Location)
                          .Include(x => x.InvoiceItem.AssetCategory)
                          .Where(x => x.IsDeleted == false
                                 && x.InvoiceItem.AssetType.Name == "Hardware"
                                 && ComputerTypeList.Contains(x.InvoiceItem.AssetCategory.Name))
                          .GroupBy(x => new { x.Id, x.Name, x.Serial, x.AssetTag, x.AssignedUser, x.InvoiceItem, x.IsDeleted, x.Status, x.ConnectedAssetId, x.LocationId })
                          .Select(x => new { x.Key.Id, x.Key.Name, x.Key.Serial, x.Key.AssetTag, x.Key.AssignedUser, x.Key.InvoiceItem, x.Key.IsDeleted, x.Key.Status, x.Key.ConnectedAssetId, x.Key.LocationId });


            if (connectedAssetId != null && onlyActive)
            {
                query = query.Where(x => x.Status.Group == "Active" || (x.ConnectedAssetId != null && x.ConnectedAssetId == connectedAssetId));
            }      
            else if (connectedAssetId != null && !onlyActive)
            {
                query = query.Where(x => x.ConnectedAssetId != null && x.ConnectedAssetId == connectedAssetId);
            }
            else if (onlyActive)
            {
                query = query.Where(x => x.Status.Group == "Active");
            }

            if (hasSerial)
            {
                query = query.Where(x => x.Serial != null && x.Serial != "");
            }

            if (locationId != 0)
            {
                query = query.Where(x => x.LocationId == locationId);
            }

            var assetList = await query.Select(x => new { x.Id, x.Name, x.Serial, x.AssignedUser }).Distinct().ToListAsync();

            foreach (var item in assetList)
            {
                var vmAsset = item != null ? new AssetVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Serial = item.Serial,
                    AssetNameOrSerial = !string.IsNullOrEmpty(item.Name) ? item.Name : item.Serial ?? "",
                    SerialandAssignedUser = item.Serial + (item.AssignedUser != null ? " - " + item.AssignedUser.LastName + ", " + item.AssignedUser.FirstName : ""),
                    AssetNameOrSerialAndUser = (!string.IsNullOrEmpty(item.Name) ? item.Name : item.Serial ?? "") + (item.AssignedUser != null ? " - " + item.AssignedUser.LastName + ", " + item.AssignedUser.FirstName : "")

                } : new AssetVM();

                vmAssetList.Add(vmAsset);
            };
            return vmAssetList.OrderBy(x => x.AssetNameOrSerial);
        }

        //Get Asset Type
        public async Task<AssetTypeVM> GetAssetType(int assetTypeId)
        {
            var query = Db.AssetTypes.Where(x => x.Id == assetTypeId).AsQueryable();
            
            var item = await query.OrderBy(r => r.Name).FirstOrDefaultAsync();

            var typeVm = item != null ? new AssetTypeVM()
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Active = item.Active
            } : new AssetTypeVM();

            return typeVm;
        }

        //Get Asset Type
        public async Task<AssetTypeVM> GetAssetType(string assetTypeName)
        {
            if (string.IsNullOrEmpty(assetTypeName))
            {
                return null;
            }
            
            var query = Db.AssetTypes.Where(x => x.Name == assetTypeName).AsQueryable();

            var item = await query.OrderBy(r => r.Name).FirstOrDefaultAsync();

            var typeVm = item != null ? new AssetTypeVM()
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Active = item.Active
            } : new AssetTypeVM();

            return typeVm;
        }

        //Get Asset Categories
        public async Task<IEnumerable<AssetCategoryVM>> GetAssetCategories()
        {
            var vmCategoryList = new List<AssetCategoryVM>();

            var query = Db.AssetCategories.Where(x => x.Active == true).AsQueryable();

            var categoryList = await query.OrderBy(r => r.Name).ToListAsync();

            foreach (var item in categoryList)
            {
                var vmCategory = item != null ? new AssetCategoryVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Active = item.Active
                } : new AssetCategoryVM();
                vmCategoryList.Add(vmCategory);
            };
            return vmCategoryList.OrderBy(x => x.Name);
        }

        //Get Asset Category
        public async Task<IEnumerable<AssetCategoryVM>> GetAssetCategoriesByType(int? assetTypeId)
        {
            assetTypeId = assetTypeId ?? 0;

            var vmCategoryList = new List<AssetCategoryVM>();

            var query = Db.AssetCategories.Where(s => s.AssetTypeId == assetTypeId).AsQueryable();

            var categoryList = await query.OrderBy(r => r.Name).ToListAsync();

            foreach (var item in categoryList)
            {
                var vmCategory = item != null ? new AssetCategoryVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Active = item.Active
                } : new AssetCategoryVM();
                vmCategoryList.Add(vmCategory);
            };
            return vmCategoryList.OrderBy(x => x.Name);
        }

        //Get Asset Statuses
        public async Task<IEnumerable<AssetStatusVM>> GetAssetStatuses(string assetType, string licenseType)
        {
            var vmStatusList = new List<AssetStatusVM>();

            var query = Db.AssetStatuses
                          .Include(x => x.AssetStatusTypes)
                          .Where(x => x.Active == true);

            if (!string.IsNullOrEmpty(assetType) && (assetType == "Hardware"))
            {
                query = query.Where(x => x.AssetStatusTypes.Any(s => s.AssetType.Name == "Hardware"));
            }
            else if (!string.IsNullOrEmpty(assetType) && (assetType == "Software"))
            {
                query = query.Where(x => x.AssetStatusTypes.Any(s => s.AssetType.Name == "Software"));
            }

            if (!string.IsNullOrEmpty(licenseType) && (licenseType == "User"))
            {
                query = query.Where(x => x.AssetStatusTypes.Any(s => s.LicenseType.Name == "User"));
            }
            else if (!string.IsNullOrEmpty(licenseType) && (licenseType != "User"))
            {
                query = query.Where(x => x.AssetStatusTypes.Any(s => s.LicenseType.Name != null && s.LicenseType.Name != "User"));
            }

            var statusList = await query.ToListAsync();

            foreach (var item in statusList)
            {
                var vmStatus = item != null ? new AssetStatusVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Sequence = item.Sequence,
                    Group = item.Group,
                    Description = item.Description,
                    Active = item.Active
                } : new AssetStatusVM();
                vmStatusList.Add(vmStatus);
            };
            return vmStatusList.OrderBy(x => x.Sequence);
        }

        //Get Asset Status
        public async Task<AssetStatusVM> GetAssetStatus(int statusId)
        {
            var status = await Db.AssetStatuses.Where(x => x.Id == statusId).FirstOrDefaultAsync();

            var statusVm = status != null ? new AssetStatusVM()
            {
                Id = status.Id,
                Name = status.Name,
                Description = status.Description,
                Active = status.Active
            } : new AssetStatusVM();

            return statusVm;
        }

        //Get License Types
        public async Task<IEnumerable<LicenseTypeVM>> GetLicenseTypes()
        {
            var vmLicenseTypesList = new List<LicenseTypeVM>();

            var licenseTypeList = await Db.LicenseTypes
                                          .Where(x => x.Active == true)
                                          .OrderBy(r => r.Name)
                                          .ToListAsync();

            foreach (var item in licenseTypeList)
            {
                var vmLicenseType = item != null ? new LicenseTypeVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Sequence = item.Sequence,
                    Active = item.Active
                } : new LicenseTypeVM();
                vmLicenseTypesList.Add(vmLicenseType);
            };
            return vmLicenseTypesList.OrderBy(x => x.Sequence);
        }

        //Get License Type
        public async Task<LicenseTypeVM> GetLicenseType(int licenseTypeId)
        {
            var item = await Db.LicenseTypes.Where(x => x.Id == licenseTypeId)
                                            .FirstOrDefaultAsync();

            var typeVm = item != null ? new LicenseTypeVM()
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Active = item.Active
            } : new LicenseTypeVM();

            return typeVm;
        }

        //Get License Type
        public async Task<LicenseTypeVM> GetLicenseType(string licenseTypeName)
        {
            if (string.IsNullOrEmpty(licenseTypeName))
            {
                return null;
            }
            var item = await Db.LicenseTypes.Where(x => x.Name == licenseTypeName)
                                            .FirstOrDefaultAsync();

            var typeVm = item != null ? new LicenseTypeVM()
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Active = item.Active
            } : new LicenseTypeVM();

            return typeVm;
        }

        //Check For Serials
        public async Task<string> CheckSerials(List<string> serials, int manuId)
        {
            var asset = await Db.Assets
                                .Where(x => x.InvoiceItem != null
                                       && x.InvoiceItem.AssetType != null
                                       && x.InvoiceItem.Manufacturer != null
                                       && x.InvoiceItem.AssetType.Name == "Hardware"
                                       && x.InvoiceItem.ManuId == manuId
                                       && x.IsDeleted == false
                                       && serials.Contains(x.Serial.Replace(" ", String.Empty).ToUpper()))
                                .FirstOrDefaultAsync();

            if (asset != null)
            {
                return asset.Serial;
            }
            else
            {
                return null;
            }
        }

        //Check For Serial
        public async Task<bool> CheckSerial(string serial, int manuId)
        {
            var asset = await Db.Assets
                                .Where(x => x.InvoiceItem != null
                                       && x.InvoiceItem.AssetType != null
                                       && x.InvoiceItem.Manufacturer != null
                                       && x.InvoiceItem.AssetType.Name == "Hardware"
                                       && x.InvoiceItem.ManuId == manuId
                                       && x.IsDeleted == false
                                       && x.Serial.Replace(" ", String.Empty).ToUpper() == serial.Replace(" ", String.Empty).ToUpper())
                                .FirstOrDefaultAsync();

            if (asset != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Check For Licenses
        public async Task<string> CheckLicenses(List<string> licenses, string licenseType)
        {
            string licenseKey = null;

            if (licenseType == "Hardware-Single")
            {
                licenseKey = await Db.InvoiceItems.Where(x => x.AssetType != null
                                                    && x.AssetType.Name == "Software"
                                                    && x.IsDeleted == false
                                                    && licenses.Contains(x.LicenseKeySingle))
                                                   .Select(x => x.LicenseKeySingle)
                                                   .FirstOrDefaultAsync();
            }
            else if (licenseType == "Hardware-Multi")
            {
                licenseKey = await Db.Assets
                                     .Where(x => x.InvoiceItem.AssetType != null
                                            && x.InvoiceItem.AssetType.Name == "Software"
                                            && x.IsDeleted == false
                                            && licenses.Contains(x.LicenseKeyMulti))
                                     .Select(x => x.LicenseKeyMulti)
                                     .FirstOrDefaultAsync();
            }

            return licenseKey;
        }

        //Check For Asset Tag
        public async Task<bool> CheckAssetTag(string assetTag)
        {
            var asset = await Db.Assets
                                .Where(x => x.IsDeleted == false
                                       && x.AssetTag != null 
                                       && x.AssetTag == assetTag)
                                .FirstOrDefaultAsync();

            if (asset != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Check For MacAddress
        public async Task<bool> CheckMacAddress(string macAddress)
        {
            var macAddressList = await Db.Assets
                                         .Where(x => x.IsDeleted == false
                                          && x.MacAddress != null && x.MacAddress != "")
                                         .Select(x => x.MacAddress)
                                         .ToListAsync();

            macAddressList = macAddressList.Select(x => x.GetAlphaNumeric()?.ToUpper()).ToList();

            if (macAddressList.Contains(macAddress.GetAlphaNumeric()?.ToUpper()))
            { 
                return true;
            }
            else
            {
                return false;
            }
        }

        //Check For Drawer
        public async Task<bool> CheckDrawer(string drawer)
        {
            var asset = await Db.Assets
                                .Where(x => x.IsDeleted == false
                                       && x.Drawer != null
                                       && x.Drawer == drawer)
                                .FirstOrDefaultAsync();

            if (asset != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Check For IPAddress
        public async Task<bool> CheckIPAddress(string ipaddress)
        {
            var asset = await Db.Assets
                                .Where(x => x.IsDeleted == false
                                       && x.IPAddress != null
                                       && x.IPAddress == ipaddress)
                                .FirstOrDefaultAsync();

            if (asset != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Get License Type Name for Asset
        public async Task<string> GetAssetLicenseType(int? invoiceItemId)
        {
            invoiceItemId = invoiceItemId ?? 0;
            var item = await Db.InvoiceItems.Include(x => x.LicenseType)
                                            .Where(x => x.IsDeleted == false && x.Id == invoiceItemId)
                                            .FirstOrDefaultAsync();


            if (item != null && item.LicenseType != null)
            {
                return item.LicenseType.Name;
            }
            else
            {
                return null;
            }
        }

        //Remove Assigned
        public async Task RemoveAssignedAsset(int assetId, int? userId)
        {
            var assets = await Db.Assets.Where(x => x.AssignedAssetId == assetId).ToListAsync();

            foreach (var item in assets)
            {
                item.ModifiedBy = userId;
                item.AssignedAssetId = null;
                item.AssignedDate = null;
                var assetEntry = Db.Entry(item);
                assetEntry.State = EntityState.Modified;
            }
            await Db.SaveChangesAsync();
        }

        //Update Connected
        public async Task UpdateConnectedAssets(Asset asset)
        {
            var assets = await Db.Assets.Where(x => x.ConnectedAssetId == asset.Id && x.IsDeleted == false).ToListAsync();

            foreach (var item in assets)
            {
                item.ModifiedBy = asset.ModifiedBy;
                item.LocationId = asset.LocationId;
                item.BuildingId = asset.BuildingId;
                item.RoomId = asset.RoomId;
                item.StatusId = asset.StatusId;
                item.AssignedUserId = asset.AssignedUserId;
                item.AssignedDate = asset.AssignedDate;

                var assetEntry = Db.Entry(item);
                assetEntry.State = EntityState.Modified;
            }
            await Db.SaveChangesAsync();
        }

        //Remove Connected
        public async Task RemoveConnectedAssets(int assetId, int? userId)
        {
            var assets = await Db.Assets.Where(x => x.ConnectedAssetId == assetId).ToListAsync();

            foreach (var item in assets)
            {
                item.ModifiedBy = userId;
                item.ConnectedAssetId = null;
                item.AssignedDate = null;
                var assetEntry = Db.Entry(item);
                assetEntry.State = EntityState.Modified;
            }
            await Db.SaveChangesAsync();
        }
    }
}