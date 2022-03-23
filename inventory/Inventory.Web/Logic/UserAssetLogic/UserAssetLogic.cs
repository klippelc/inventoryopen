using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using PagedList;
using Inventory.Data.Services;
using Inventory.Web.Common;
using Inventory.Web.ViewModels;
using PagedList.EntityFramework;
using System;
using Inventory.Data.Models;

namespace Inventory.Web.Logic
{
    public class UserAssetLogic : AssetBaseLogic, IUserAssetLogic
    {
        //Constructor
        public UserAssetLogic(InventoryDbContext db, IAssetLogic assetLogic)
        {
            Db = db;
            AssetLogic = assetLogic;
        }

        public async Task<IEnumerable<PagedListVM>> GetUserAssetList(Parameters parameters)
        {
            var activeStatus = parameters.ActiveStatus;
            var assetType = parameters.AssetType;
            var assetId = parameters.AssetId;
            var showConnectedAssets = parameters.ShowConnectedAssets;
            var invoiceItemId = parameters.InvoiceItemId;
            var assetCategory = parameters.AssetCategory;
            var manuId = parameters.ManuId;
            var productId = parameters.ProductId;
            var licenseType = parameters.LicenseType;
            var searchString = parameters.SearchString;
            var sortOrder = parameters.SortOrder;
            var LoginUserId = parameters.LoginUserId;
            var ManagedUserId = parameters.ManagedUserId;
            var export = parameters.Export;
            var page = (export ?? false) ? 1 : parameters.CurrentPage;
            var pageSize = (export ?? false) ? 9999 : parameters.PageSize;

            var vmPagedList = new List<PagedListVM>();
            var vmPaged = new PagedListVM();
            var vmAssetList = new List<AssetVM>();
            var vmAssetCategoryList = new List<AssetCategoryVM>();
            var vmLicenseTypeList = new List<LicenseTypeVM>();
            var vmAssetStatusList = new List<AssetStatusVM>();
            var vmManufacturerList = new List<ManufacturerVM>();
            var vmProductList = new List<ProductVM>();
            var vmUsersManagedList = new List<UserVM>();

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
               .Include(x => x.ConnectedAsset.InvoiceItem.AssetCategory)
               .Include(x => x.ConnectedAssets.Select(a => a.InvoiceItem.AssetCategory))
               .Include(x => x.UserAssetConfirmations)
               .Where(x => x.IsDeleted == false);

            if (ManagedUserId != null)
            {
                query = query.Where(s => s.AssignedUser != null 
                                           && s.AssignedUserId == ManagedUserId 
                                           && s.AssignedUser.ManagerId == LoginUserId).AsQueryable();
            }
            else
            {
                query = query.Where(s => s.AssignedUser != null && s.AssignedUserId == LoginUserId).AsQueryable();
            }

            if (!string.IsNullOrEmpty(assetType))
            {
                query = query.Where(s => s.InvoiceItem.AssetType.Name.ToLower() == assetType.ToLower()).AsQueryable();
            }

            if (activeStatus == 1)
            {
                query = query.Where(s => s.Status.Group == "Active").AsQueryable();
            }
            else if (activeStatus == 2)
            {
                query = query.Where(s => s.Status.Group != "Active").AsQueryable();
            }

            if (assetId != null && showConnectedAssets == true)
            {
                query = query.Where(s => s.Id == assetId || s.ConnectedAssetId == assetId).AsQueryable();
            }
            else if (assetId != null)
            {
                query = query.Where(s => s.Id == assetId).AsQueryable();
            }
            else if (showConnectedAssets == true)
            {
                query = query.Where(s => s.ConnectedAssetId != null).AsQueryable();
            }

            if (invoiceItemId != null)
            {
                query = query.Where(s => s.InvoiceItemId == invoiceItemId).AsQueryable();
            }

            if (!string.IsNullOrEmpty(assetCategory))
            {
                query = query.Where(s => (s.InvoiceItem.AssetCategory != null
                                       && s.InvoiceItem.AssetCategory.Name.ToLower() == assetCategory.ToLower())).AsQueryable();
            }

            if (manuId != null)
            {
                query = query.Where(s => s.InvoiceItem.ManuId == manuId).AsQueryable();
            }

            if (productId != null)
            {
                query = query.Where(s => s.InvoiceItem.ProductId == productId).AsQueryable();
            }

            if (!string.IsNullOrEmpty(licenseType) && (assetType.Equals("Software")))
            {
                query = query.Where(s => (s.InvoiceItem.LicenseType != null
                                       && s.InvoiceItem.LicenseType.Name.ToLower() == licenseType.ToLower())).AsQueryable();
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
                    var phone = search.Length > 3 ? search.Replace("-", "").Substring(search.Length - 4) : search;

                    var searchList = search?.Split('-').ToList();
                    var invoiceNumber = searchList.Count > 0 ? searchList[0] : "";
                    var invoiceItemNumber = searchList.Count > 1 ? searchList[1] : "";
                    var serialNumber = searchList.Count > 2 ? searchList[2] : "";

                    query = query.Where(s => ((s.InvoiceItem.Invoice != null)
                                        && (s.InvoiceItem.Invoice.Id.ToString().Contains(invoiceNumber))
                                        && (s.InvoiceItem.InvoiceItemNumber.ToString().Contains(invoiceItemNumber))
                                        || (s.InvoiceItem.Invoice.PONumber.ToLower().Contains(search.ToLower())))
                                        || (s.Name != null && s.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Description != null && s.Description.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Notes != null && s.Notes.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssetTag != null && s.AssetTag.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Serial != null && s.Serial.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Serial != null && s.Serial.ToString().ToLower() == serialNumber.ToLower())
                                        || (s.MacAddress != null && s.MacAddress.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.LastName != null && s.AssignedUser.LastName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.FirstName != null && s.AssignedUser.FirstName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.Phone != null && s.AssignedUser.Phone.ToString() == phone)
                                        || (s.Location != null && s.Location.Name != null && s.Location.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.DisplayName != null && s.Location.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.Code != null && s.Location.Code.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.DisplayName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.DisplayName.ToString().ToLower().Contains(search.ToLower()))).AsQueryable();
                }
                else
                {
                    query = query.Where(s => ((s.InvoiceItem.Invoice != null)
                                        && (s.InvoiceItem.Invoice.Id.ToString().Contains(search))
                                        || (s.InvoiceItem.InvoiceItemNumber.ToString().Contains(search))
                                        || (s.InvoiceItem.Invoice.PONumber.ToLower().Contains(search.ToLower())))
                                        || (s.Name != null && s.Name.ToString().ToLower() == search.ToLower())
                                        || (s.Description != null && s.Description.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Notes != null && s.Notes.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssetTag != null && s.AssetTag.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Serial != null && s.Serial.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.MacAddress != null && s.MacAddress.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.LastName != null && s.AssignedUser.LastName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.FirstName != null && s.AssignedUser.FirstName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.FirstName != null && s.AssignedUser.FirstName.ToString().ToLower().Contains(FirstName.ToLower())
                                             && s.AssignedUser.LastName != null && s.AssignedUser.LastName.ToString().ToLower().Contains(LastName.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.Phone != null && s.AssignedUser.Phone.ToString().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.UserName != null && s.AssignedUser.UserName.ToString().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.Name != null && s.InvoiceItem.Manufacturer.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.DisplayName != null && s.InvoiceItem.Manufacturer.DisplayName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.Name != null && s.InvoiceItem.Product.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.DisplayName != null && s.InvoiceItem.Product.DisplayName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.Name != null && s.Location.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.DisplayName != null && s.Location.DisplayName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.Code != null && s.Location.Code.ToString().ToLower().Contains(search.ToLower()))).AsQueryable();
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
                                        && (s.InvoiceItem.Invoice.Id.ToString().Contains(invoiceNumber))
                                        && (s.InvoiceItem.InvoiceItemNumber.ToString().Contains(invoiceItemNumber))
                                        || (s.InvoiceItem.Invoice.PONumber.ToLower().Contains(search.ToLower())))
                                        || (s.InvoiceItem.LicenseType != null && s.InvoiceItem.LicenseType.Name.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.LicenseKeySingle.ToLower().Contains(search.ToLower()))
                                        || (s.LicenseKeyMulti.ToLower().Contains(search.ToLower()))
                                        || (s.AssignedAsset != null && s.AssignedAsset.AssetTag != null && s.AssignedAsset.AssetTag.ToString().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.LastName != null && s.AssignedUser.LastName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.FirstName != null && s.AssignedUser.FirstName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.Phone != null && s.AssignedUser.Phone == phone)
                                        || (s.Location != null && s.Location.Name != null && s.Location.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.DisplayName != null && s.Location.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.Code != null && s.Location.Code.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.Name.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.Name.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.DisplayName.ToLower().Contains(search.ToLower()))).AsQueryable();
                }
                else
                {
                    query = query.Where(s => ((s.InvoiceItem.Invoice != null)
                                        && (s.InvoiceItem.Invoice.Id.ToString().Contains(search))
                                        || (s.InvoiceItem.InvoiceItemNumber.ToString().Contains(search))
                                        || (s.InvoiceItem.Invoice.PONumber.ToLower().Contains(search.ToLower())))
                                        || (s.InvoiceItem.LicenseType != null && s.InvoiceItem.LicenseType.Name.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.LicenseKeySingle.ToLower().Contains(search.ToLower()))
                                        || (s.LicenseKeyMulti.ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.LastName != null && s.AssignedUser.LastName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.FirstName != null && s.AssignedUser.FirstName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.FirstName != null && s.AssignedUser.FirstName.ToString().ToLower().Contains(FirstName.ToLower())
                                             && s.AssignedUser.LastName != null && s.AssignedUser.LastName.ToString().ToLower().Contains(LastName.ToLower()))
                                        || (s.AssignedUser != null && s.AssignedUser.Phone != null && s.AssignedUser.Phone.Contains(search))
                                        || (s.AssignedUser != null && s.AssignedUser.UserName != null && s.AssignedUser.UserName.ToString().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.Name != null && s.Location.Name.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.DisplayName != null && s.Location.DisplayName.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.Location != null && s.Location.Code != null && s.Location.Code.ToString().ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.Name.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Product != null && s.InvoiceItem.Product.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.Name.ToLower().Contains(search.ToLower()))
                                        || (s.InvoiceItem.Manufacturer != null && s.InvoiceItem.Manufacturer.DisplayName.ToLower().Contains(search.ToLower()))).AsQueryable();
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
                    query = query.OrderBy(s => s.AssignedUser != null ? s.AssignedUser.LastName ?? "zzz" : "zzz")
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
                    query = query.OrderBy(s => s.AssignedAsset != null && s.AssignedAsset.Location != null ? s.AssignedAsset.Location.Code ?? "zzz" : "zzz")
                                 .ThenBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
                                 .ThenBy(s => s.InvoiceItem.Product != null ? string.IsNullOrEmpty(s.InvoiceItem.Product.DisplayName) ? s.InvoiceItem.Product.Name : s.InvoiceItem.Product.DisplayName : "");
                    break;

                case "assignedlocation_desc":
                    query = query.OrderByDescending(s => s.AssignedAsset != null && s.AssignedAsset.Location != null ? s.AssignedAsset.Location.Code : "")
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
                                 .ThenBy(s => s.Building != null ? string.IsNullOrEmpty(s.Building.DisplayName) ? s.Building.Name ?? "zzz" : s.Building.DisplayName : "zzz")
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

                default:
                    query = query.OrderBy(s => s.InvoiceItem.Manufacturer != null ? string.IsNullOrEmpty(s.InvoiceItem.Manufacturer.DisplayName) ? s.InvoiceItem.Manufacturer.Name : s.InvoiceItem.Manufacturer.DisplayName : "")
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
                                                    .GroupBy(x => new { x.Manufacturer.Id, x.Manufacturer.Name, x.Manufacturer.DisplayName })
                                                    .Select(x => new { x.Key.Id, x.Key.Name, x.Key.DisplayName }).ToList();

            var ProductList = InventoryItemList.Where(x => x.Product != null)
                                               .GroupBy(x => new { x.Product.Id, x.Product.Name, x.Product.DisplayName })
                                               .Select(x => new { x.Key.Id, x.Key.Name, x.Key.DisplayName }).ToList();

            var AssetStatusList = await query.Where(x => x.Status != null)
                                             .GroupBy(x => new { x.Status.Id, x.Status.Name, x.Status.Group, x.Status.Sequence })
                                             .Select(x => new { x.Key.Id, x.Key.Name, x.Key.Group, x.Key.Sequence }).ToListAsync();

            var UsersManagedList = await Db.Users.Include(x => x.Manager)
                                           .Where(x => x.ManagerId == LoginUserId)
                                           .ToListAsync();

            var ConnectedAsset = await query.Where(x => x.ConnectedAsset != null).Select(x => x.ConnectedAsset).FirstOrDefaultAsync();

            var items = (IPagedList)AssetsList;

            foreach (var item in AssetsList)
            {
                var vm = new AssetVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,

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
                        Description = item.Status.Description
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
                        Name = item.Room.Name,
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

                vm.HasItBeen1Year = ((vm.AssetConfirmedDate ?? DateTime.Now).AddYears(1) <= DateTime.Now);
                vm.AbleToConfirm = ((vm.HasItBeen1Year ?? false) || (vm.AssetConfirmedDate == null && vm.AssignedUserId == LoginUserId)) ? true : false;

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

            foreach (var item in AssetStatusList)
            {
                var vm = new AssetStatusVM()
                {
                    Id = item.Group != "Active" ? 2 : 1,
                    Name = item.Group,
                };
                vmAssetStatusList.Add(vm);
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
                    Name = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,
                };
                vmManufacturerList.Add(vm);
            }

            foreach (var item in ProductList)
            {
                var vm = new ProductVM()
                {
                    Id = item.Id,
                    Name = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,
                };
                vmProductList.Add(vm);
            }

            foreach (var item in UsersManagedList)
            {
                var vm = new UserVM()
                {
                    Id = item.Id,
                    UserId = item.Id,
                    Title = item.Title,
                    UserName = item.UserName,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Name = item.LastName + ", " + item.FirstName,
                    NameWithUserName = item.LastName + ", " + item.FirstName + " (" + item.UserName + ")",
                    Active = item.Active,
                    ManagerName = item.Manager?.LastName + ", " + item.Manager?.FirstName
                };
                vmUsersManagedList.Add(vm);
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
            vmPaged.Manufacturers = vmManufacturerList;
            vmPaged.Products = vmProductList;
            vmPaged.UsersManaged = vmUsersManagedList;

            vmPagedList.Add(vmPaged);

            StaticPagedList<PagedListVM> assetList = new StaticPagedList<PagedListVM>(vmPagedList, items.PageNumber, items.PageSize, items.TotalItemCount);

            return assetList;
        }

        //Get Asset
        public async Task<AssetVM> GetUserAsset(int Id, int loginUserId, string assetType)
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
                                .Where(x => (x.IsDeleted == false
                                       && x.InvoiceItem.AssetType.Name == assetType
                                       && x.Id == Id)
                                       && ((x.AssignedUserId == loginUserId)
                                       || (x.AssignedUserId != null && x.AssignedUser.ManagerId == loginUserId))).FirstOrDefaultAsync();


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

            vm.HasItBeen1Year = ((vm.AssetConfirmedDate ?? DateTime.Now).AddYears(1) <= DateTime.Now);
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
            vm.ConnectedAssets = await AssetLogic.GetConnectedAssets(vm.Id);

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

        //Confirm Asset
        public async Task<bool> ConfirmAsset(UserAssetConfirmationVM vm, int loginUserId)
        {
            var asset = await Db.Assets
                                .Include(x => x.UserAssetConfirmations)
                                .Where(x => x.Id == vm.AssetId && x.AssignedUserId == loginUserId)
                                .FirstOrDefaultAsync();

            if (asset == null)
            {
                return false;
            }

            var AssetConfirmedDate = asset.UserAssetConfirmations.Where(z => (z.UserId == asset.AssignedUserId) && ((z.DateConfirmed > asset.AssignedDate) || (asset.AssignedDate == null))).Max(x => x.DateConfirmed);
            var HasItBeen1Year = ((AssetConfirmedDate ?? DateTime.Now).AddYears(1) <= DateTime.Now);
            var AbleToConfirm = (HasItBeen1Year || (AssetConfirmedDate == null && asset.AssignedUserId == loginUserId));

            if (!AbleToConfirm)
            {
                return false;
            }

            var confirmation = new UserAssetConfirmation()
            {
                AssetId = asset.Id,
                UserId = loginUserId,
                DateConfirmed = DateTime.Now,
                CreatedBy = loginUserId,
                ModifiedBy = loginUserId
            };

            Db.UserAssetConfirmations.Add(confirmation);
            await Db.SaveChangesAsync();

            return true;
        }
    }
}