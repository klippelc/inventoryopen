using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PagedList;
using Inventory.Data.Models;
using Inventory.Data.Services;
using Inventory.Web.Common;
using Inventory.Web.ViewModels;
using System.Data.Entity;

namespace Inventory.Web.Logic
{
    public class InvoiceItemLogic : AssetBaseLogic, IInvoiceItemLogic
    {
        //Constructor
        public InvoiceItemLogic(InventoryDbContext db, IInvoiceLogic invoiceLogic, 
            IAssetLogic assetLogic, IUserLogic userLogic, IManuLogic manuLogic, IProductLogic productLogic, 
            ISupplierLogic supplierLogic, ILocationLogic locationLogic, IBuildingLogic buildingLogic, IRoomLogic roomLogic, ICommonLogic commonLogic)
        {
            Db = db;
            InvoiceLogic = invoiceLogic;
            AssetLogic = assetLogic;
            UserLogic = userLogic;
            ManuLogic = manuLogic;
            ProductLogic = productLogic;
            SupplierLogic = supplierLogic;
            LocationLogic = locationLogic;
            BuildingLogic = buildingLogic;
            RoomLogic = roomLogic;
            CommonLogic = commonLogic;
        }

        //Get Invoice Items
        public async Task<IEnumerable<PagedListVM>> GetInvoiceItems(int? invoiceId, int? invoiceItemId, string assetType = "", string assetCategory = "", string sortOrder = "", string searchString = "", int page = 1, int pageSize = 10000000)
        {
            int pageNumber = page;

            var vmPagedList = new List<PagedListVM>();
            var vmPaged = new PagedListVM();
            var vmList = new List<InvoiceItemVM>();
            var vmAssetCategoryList = new List<AssetCategoryVM>();
            var vmAssetTypeList = new List<AssetTypeVM>();
            var vmAssetStatusList = new List<AssetStatusVM>();

            var query = Db.InvoiceItems
                          .Include(x => x.Invoice)
                          .Include(x => x.Manufacturer)
                          .Include(x => x.Product)
                          .Include(x => x.AssetType)
                          .Include(x => x.AssetCategory)
                          .Include(x => x.Assets.Select(s => s.Status))
                          .Where(x => x.IsDeleted == false)
                          .AsQueryable();

            if (invoiceId != null)
            {
                query = query.Where(s => s.InvoiceId == invoiceId).AsQueryable();
            }

            if (invoiceItemId != null)
            {
                query = query.Where(s => s.Id == invoiceItemId).AsQueryable();
            }

            if (!string.IsNullOrEmpty(assetType))
            {
                query = query.Where(s => (s.AssetType != null
                                       && s.AssetType.Name.ToLower() == assetType.ToLower())).AsQueryable();
            }

            if (!string.IsNullOrEmpty(assetCategory))
            {
                query = query.Where(s => (s.AssetCategory != null
                                       && s.AssetCategory.Name.ToLower() == assetCategory.ToLower())).AsQueryable();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                var search = searchString?.Trim(); 

                if (search.Contains("-"))
                {
                    search = search?.RemoveSpaces();
                    var searchList = search?.Split('-').ToList();
                    var invoiceNumber = searchList.Count > 0 ? searchList[0] : "";
                    var invoiceItemNumber = searchList.Count > 1 ? searchList[1] : "";

                    query = query.Where(s => ((s.Invoice != null) 
                                        && (s.Invoice.Id.ToString().Contains(invoiceNumber))
                                        && (s.InvoiceItemNumber.ToString().Contains(invoiceItemNumber))
                                        || (s.Invoice.PONumber.ToLower().Contains(search.ToLower())))
                                        || (s.Product != null && s.Product.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Product != null && s.Product.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.Manufacturer != null && s.Manufacturer.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Manufacturer != null && s.Manufacturer.DisplayName.ToLower().Contains(search.ToLower()))).AsQueryable();
                }
                else
                {
                    query = query.Where(s => ((s.Invoice != null) 
                                        && (s.Invoice.Id.ToString().Contains(search))
                                        || (s.InvoiceItemNumber.ToString().Contains(search))
                                        || (s.Invoice.PONumber.ToLower().Contains(search.ToLower())))
                                        || (s.Product != null && s.Product.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Product != null && s.Product.DisplayName.ToLower().Contains(search.ToLower()))
                                        || (s.Manufacturer != null && s.Manufacturer.Name.ToLower().Contains(search.ToLower()))
                                        || (s.Manufacturer != null && s.Manufacturer.DisplayName.ToLower().Contains(search.ToLower()))).AsQueryable();
                }
            }

            var InvoiceList = await query.ToListAsync();
            var CategoryList = await query.Select(x => x.AssetCategory).ToListAsync();
            var TypeList = await query.Select(x => x.AssetType).ToListAsync();

            foreach (var item in InvoiceList)
            {
                var vm = item != null ? new InvoiceItemVM()
                {
                    Id = item.Id,
                    InvoiceId = item.InvoiceId,
                    Invoice = item.Invoice != null ? new InvoiceVM()
                    {
                        Id = item.Invoice.Id,
                        PONumber = item.Invoice.PONumber,

                        SupplierId = item.Invoice.SupplierId,
                        Supplier = item.Invoice.Supplier != null ? new SupplierVM()
                        {
                            Id = item.Invoice.Supplier.Id,
                            Name = item.Invoice.Supplier.Name,
                            DisplayName = !string.IsNullOrEmpty(item.Invoice.Supplier.DisplayName) ? item.Invoice.Supplier.DisplayName : item.Invoice.Supplier.Name,
                            Description = item.Invoice.Supplier.Description
                        } : new SupplierVM(),

                        PurchaseDate = item.Invoice.PurchaseDate,
                        TotalPrice = item.Invoice.TotalPrice
                    } : new InvoiceVM(),

                    InvoiceItemNumber = item.InvoiceItemNumber,

                    AssetTypeId = item.AssetTypeId,
                    AssetType = item.AssetType != null ? new AssetTypeVM()
                    {
                        Id = item.AssetType.Id,
                        Name = item.AssetType.Name,
                        Description = item.AssetType.Description,
                        IconCss = item.AssetType.IconCss
                    } : new AssetTypeVM(),

                    AssetCategoryId = item.AssetCategoryId,
                    AssetCategory = item.AssetCategory != null ? new AssetCategoryVM()
                    {
                        Id = item.AssetCategory.Id,
                        Name = item.AssetCategory.Name,
                        Description = item.AssetCategory.Description,
                        IconCss = item.AssetCategory.IconCss
                    } : new AssetCategoryVM(),

                    LicenseTypeId = item.LicenseTypeId,
                    LicenseType = item.LicenseType != null ? new LicenseTypeVM()
                    {
                        Id = item.LicenseType.Id,
                        Name = item.LicenseType.Name,
                        Description = item.LicenseType.Description
                    } : new LicenseTypeVM(),

                    ManuId = item.ManuId,
                    Manufacturer = item.Manufacturer != null ? new ManufacturerVM()
                    {
                        Id = item.Manufacturer.Id,
                        Name = item.Manufacturer.Name,
                        DisplayName = !string.IsNullOrEmpty(item.Manufacturer.DisplayName) ? item.Manufacturer.DisplayName : item.Manufacturer.Name,
                        Description = item.Manufacturer.Description
                    } : new ManufacturerVM(),

                    ProductId = item.ProductId,
                    Product = item.Product != null ? new ProductVM()
                    {
                        Id = item.Product.Id,
                        Name = item.Product.Name,
                        DisplayName = !string.IsNullOrEmpty(item.Product.DisplayName) ? item.Product.DisplayName : item.Product.Name,
                        Description = item.Product.Description
                    } : new ProductVM(),

                    Quantity = item.Assets != null ? item.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false).Count() : 0,
                    Active = item.Assets != null ? item.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false && x.Status != null && x.Status.Group?.ToLower() == "active").Count() : 0,
                    InActive = item.Assets != null ? item.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false && x.Status != null && x.Status.Group?.ToLower() != "active").Count() : 0,
                    Assigned = item.Assets != null ? item.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false && x.Status != null && x.Status.Name?.ToLower() == "assigned").Count() : 0,
                    Available = item.Assets != null ? item.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false && x.Status != null && x.Status.Name?.ToLower() == "available").Count() : 0,
                    Shared = item.Assets != null ? item.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false && x.Status != null && x.Status.Name?.ToLower() == "shared").Count() : 0,

                    UnitPrice = item.UnitPrice ?? 0,
                    ExpirationDate = item.ExpirationDate,
                    Specifications = item.Specifications,
                    Notes = item.Notes,

                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy,
                    DateAdded = item.DateAdded,
                    DateModified = item.DateModified
                } : null;

                vm.SupplierName = vm.Invoice.Supplier.DisplayName;
                vm.AssetTypeName = vm.AssetType.Name;
                vm.InvoiceNumberAndItemNumber = vm.Invoice.Id + "-" + vm.InvoiceItemNumber.ToString();
                vm.QuantityOriginal = vm.Quantity;

                vmList.Add(vm);
            };

            switch (sortOrder)
            {
                case "invoice":
                    vmList = vmList.OrderBy(s => s.Invoice.Id)
                                   .ThenBy(s => s.InvoiceItemNumber != null ? Convert.ToInt32(s.InvoiceItemNumber) : 0)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "invoice_desc":
                    vmList = vmList.OrderByDescending(s => s.Invoice.Id)
                                   .ThenByDescending(s => s.InvoiceItemNumber != null ? Convert.ToInt32(s.InvoiceItemNumber) : 0)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "ponumber":
                    vmList = vmList.OrderBy(s => s.Invoice.PONumber)
                                   .ThenBy(s => s.InvoiceItemNumber != null ? Convert.ToInt32(s.InvoiceItemNumber) : 0)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "ponumber_desc":
                    vmList = vmList.OrderByDescending(s => s.Invoice.PONumber)
                                   .ThenByDescending(s => s.InvoiceItemNumber != null ? Convert.ToInt32(s.InvoiceItemNumber) : 0)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "manufacturer":
                    vmList = vmList.OrderBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name ?? "zzzz" : "zzzz")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "manufacturer_desc":
                    vmList = vmList.OrderByDescending(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "product":
                    vmList = vmList.OrderBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name ?? "zzzz" : "zzzz")
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "").ToList();
                    break;

                case "product_desc":
                    vmList = vmList.OrderByDescending(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "")
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "").ToList();
                    break;

                case "assettype":
                    vmList = vmList.OrderBy(s => s.AssetType != null ? s.AssetType.Name ?? "zzzz" : "zzzz")
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "assettype_desc":
                    vmList = vmList.OrderByDescending(s => s.AssetType != null ? s.AssetType.Name : "")
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "assetcat":
                    vmList = vmList.OrderBy(s => s.AssetCategory != null ? s.AssetCategory.Name ?? "zzzz" : "zzzz")
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "assetcat_desc":
                    vmList = vmList.OrderByDescending(s => s.AssetCategory != null ? s.AssetCategory.Name : "")
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "licensetype":
                    vmList = vmList.OrderBy(s => s.LicenseType != null ? s.LicenseType.Name ?? "zzzz" : "zzzz")
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "licensetype_desc":
                    vmList = vmList.OrderByDescending(s => s.LicenseType != null ? s.LicenseType.Name : "")
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "quantity":
                    vmList = vmList.OrderBy(s => s.Quantity ?? 999999999)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "quantity_desc":
                    vmList = vmList.OrderByDescending(s => s.Quantity)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "assigned":
                    vmList = vmList.OrderBy(s => s.Assigned ?? 999999999)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "assigned_desc":
                    vmList = vmList.OrderByDescending(s => s.Assigned)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "shared":
                    vmList = vmList.OrderBy(s => s.Shared ?? 999999999)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "shared_desc":
                    vmList = vmList.OrderByDescending(s => s.Shared)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "inactive":
                    vmList = vmList.OrderBy(s => s.InActive ?? 999999999)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "inactive_desc":
                    vmList = vmList.OrderByDescending(s => s.InActive)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "active":
                    vmList = vmList.OrderBy(s => s.Active ?? 999999999)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "active_desc":
                    vmList = vmList.OrderByDescending(s => s.Active)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "available":
                    vmList = vmList.OrderBy(s => s.Available ?? 999999999)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "available_desc":
                    vmList = vmList.OrderByDescending(s => s.Available)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "unitprice":
                    vmList = vmList.OrderBy(s => s.UnitPrice ?? 999999999)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                case "unitprice_desc":
                    vmList = vmList.OrderByDescending(s => s.UnitPrice)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;

                default:
                    vmList = vmList.OrderBy(s => s.Invoice.PONumber)
                                   .ThenBy(s => s.InvoiceItemNumber != null ? Convert.ToInt32(s.InvoiceItemNumber) : 0)
                                   .ThenBy(s => s.Manufacturer != null ? s.Manufacturer.DisplayName.NullorEmpty() ?? s.Manufacturer.Name : "")
                                   .ThenBy(s => s.Product != null ? s.Product.DisplayName.NullorEmpty() ?? s.Product.Name : "").ToList();
                    break;
            }

            foreach (var item in TypeList.GroupBy(x => new { x.Id, x.Name }).Select(x => new { x.Key.Id, x.Key.Name }))
            {
                var vm = new AssetTypeVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                vmAssetTypeList.Add(vm);
            }

            foreach (var item in CategoryList.GroupBy(x => new { x.Id, x.Name }).Select(x => new { x.Key.Id, x.Key.Name }))
            {
                var vm = new AssetCategoryVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                vmAssetCategoryList.Add(vm);
            }

            var AssetStatusList = await Db.AssetStatuses.Where(x => x.Active == true).ToListAsync();

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

            vmPaged.InvoiceItems = vmList.ToPagedList(pageNumber, pageSize);
            vmPaged.AssetTypes = vmAssetTypeList;
            vmPaged.AssetCategories = vmAssetCategoryList;
            vmPaged.AssetStatuses = vmAssetStatusList;
            vmPagedList.Add(vmPaged);

            StaticPagedList<PagedListVM> invoiceItemsList = new StaticPagedList<PagedListVM>(vmPagedList, pageNumber, pageSize, vmList.Count());

            return invoiceItemsList;
        }

        //Get Blank Invoice Item
        public async Task<InvoiceItemVM> GetBlankInvoiceItem()
        {
            var Assets = await AssetLogic.GetComputerTypeAssets(null);

            var vm = new InvoiceItemVM
            {
                DateReceived = DateTime.Now,
                Invoices = await InvoiceLogic.GetInvoices(),
                AssetTypes = await CommonLogic.GetAssetTypes(),
                LicenseTypes = await AssetLogic.GetLicenseTypes(),
                Locations = await LocationLogic.GetLocations(),
                Users = await UserLogic.GetUsers(),
                Assets = Assets.OrderBy(x => x.Serial),
                AssetCategories = new List<AssetCategoryVM>(),
                Manufacturers = new List<ManufacturerVM>(),
                Products = new List<ProductVM>(),
                Buildings = new List<BuildingVM>(),
                Rooms = new List<RoomVM>(),
                Suppliers = await SupplierLogic.GetSuppliers()
            };

            return vm;
        }

        //Create Invoice Item
        public async Task<InvoiceItemVM> CreateInvoiceItem(InvoiceItemVM vm)
        {
            vm.InvoiceItemNumber = await GetMinInvoiceItemNumber(vm.InvoiceId ?? 0) + 1;

            var product = await ProductLogic.Get(vm.ProductId ?? 0);
            if (product != null)
            {
                    vm.ManuId = product.ManuId;
            }

            if (vm.AssetRoomId != null)
            {
                var room = await RoomLogic.Get(vm.AssetRoomId ?? 0);
                if (room != null)
                {
                    vm.AssetBuildingId = room.BuildingId;
                    vm.AssetLocationId = room.LocationId;
                }
            }

            if (vm.AssetRoomId == null && vm.AssetBuildingId != null)
            {
                var building = await BuildingLogic.Get(vm.AssetBuildingId ?? 0);
                if (building != null)
                {
                    vm.AssetLocationId = building.LocationId;
                }
            }

            var invoiceItem = new InvoiceItem()
            {
                InvoiceId = vm.InvoiceId,
                InvoiceItemNumber = vm.InvoiceItemNumber,
                AssetTypeId = vm.AssetTypeId,
                AssetCategoryId = vm.AssetCategoryId,
                ManuId = vm.ManuId,
                ProductId = vm.ProductId,
                LicenseTypeId = vm.LicenseTypeId,
                LicenseKeySingle = vm.LicenseKeySingle,
                UnitPrice = vm.UnitPrice,
                ExpirationDate = vm.ExpirationDate,
                Specifications = vm.Specifications?.Trim(),
                Notes = vm.Notes?.Trim(),
                IsDeleted = false,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.CreatedBy
            };

            Db.InvoiceItems.Add(invoiceItem);
            await Db.SaveChangesAsync();
            vm.Id = invoiceItem.Id;

            var vmAsset = new AssetVM()
            {
                InvoiceItemId = vm.Id,
                AssetTag = null,
                Serial = vm.Serial,
                LicenseKeyMulti = vm.LicenseKeyMulti,
                MacAddress = null,
                DateReceived = vm.DateReceived,
                LocationId = vm.AssetLocationId,
                BuildingId = vm.AssetBuildingId,
                RoomId = vm.AssetRoomId,
                AssignedAssetId = vm.AssignedAssetId,
                AssignedUserId = vm.AssignedUserId,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.CreatedBy
            };

            await AssetLogic.CreateAssets(vmAsset, vm.Quantity);

            return vm;
        }

        //Get Invoice Item
        public async Task<InvoiceItemVM> GetInvoiceItem(int? invoiceItemId)
        {
            var item = await Db.InvoiceItems
                               .Include(x => x.Invoice)
                               .Include(x => x.Manufacturer)
                               .Include(x => x.Product)
                               .Include(x => x.AssetCategory)
                               .Include(x => x.AssetType)
                               .Include(x => x.LicenseType)
                               .Include(x => x.Assets.Select(s => s.Status))
                               .Where(x => x.Id == invoiceItemId && x.IsDeleted == false)
                               .FirstOrDefaultAsync(r => r.Id == invoiceItemId);

            var vm = item != null ? new InvoiceItemVM()
            {
                Id = item.Id,
                InvoiceId = item.InvoiceId,
                OriginalInvoiceId = item.InvoiceId,
                Invoice = item.Invoice != null ? new InvoiceVM()
                {
                    Id = item.Invoice.Id,
                    PONumber = item.Invoice.PONumber,

                    SupplierId = item.Invoice.SupplierId,
                    SupplierIdOriginal = item.Invoice.SupplierId,

                    Supplier = item.Invoice.Supplier != null ? new SupplierVM()
                    {
                        Id = item.Invoice.Supplier.Id,
                        Name = item.Invoice.Supplier.Name,
                        DisplayName = !string.IsNullOrEmpty(item.Invoice.Supplier.DisplayName) ? item.Invoice.Supplier.DisplayName : item.Invoice.Supplier.Name,
                        Description = item.Invoice.Supplier.Description
                    } : new SupplierVM(),

                    SupplierDisplayNameOriginal = item.Invoice.Supplier != null ? item.Invoice.Supplier.DisplayName : "",

                    PurchaseDate = item.Invoice.PurchaseDate,
                    TotalPrice = item.Invoice.TotalPrice
                } : new InvoiceVM(),

                InvoiceItemNumber = item.InvoiceItemNumber,

                AssetTypeId = item.AssetTypeId,
                AssetType = item.AssetType != null ? new AssetTypeVM()
                {
                    Id = item.AssetType.Id,
                    Name = item.AssetType.Name,
                    Description = item.AssetType.Description
                } : new AssetTypeVM(),

                AssetCategoryId = item.AssetCategoryId,
                OriginalAssetCategoryId = item.AssetCategoryId,
                AssetCategory = item.AssetCategory != null ? new AssetCategoryVM()
                {
                    Id = item.AssetCategory.Id,
                    Name = item.AssetCategory.Name,
                    Description = item.AssetCategory.Description
                } : new AssetCategoryVM(),

                OriginalAssetCategoryName = item?.AssetCategory?.Name,

                LicenseTypeIdOriginal = item.LicenseTypeId,
                LicenseTypeId = item.LicenseTypeId,
                LicenseType = item.LicenseType != null ? new LicenseTypeVM()
                {
                    Id = item.LicenseType.Id,
                    Name = item.LicenseType.Name,
                    Description = item.LicenseType.Description
                } : new LicenseTypeVM(),

                ManuId = item.ManuId,
                OriginalManuId = item.ManuId,
                Manufacturer = item.Manufacturer != null ? new ManufacturerVM()
                {
                    Id = item.Manufacturer.Id,
                    Name = item.Manufacturer.Name,
                    DisplayName = !string.IsNullOrEmpty(item.Manufacturer.DisplayName) ? item.Manufacturer.DisplayName : item.Manufacturer.Name,
                    Description = item.Manufacturer.Description
                } : new ManufacturerVM(),

                ProductId = item.ProductId,
                OriginalProductId = item.ProductId,
                Product = item.Product != null ? new ProductVM()
                {
                    Id = item.Product.Id,
                    Name = item.Product.Name,
                    DisplayName = !string.IsNullOrEmpty(item.Product.DisplayName) ? item.Product.DisplayName : item.Product.Name,
                    Description = item.Product.Description,
                    Manufacturer = item.Manufacturer != null ? new ManufacturerVM()
                    {
                        Id = item.Manufacturer.Id,
                        Name = item.Manufacturer.Name,
                        DisplayName = !string.IsNullOrEmpty(item.Manufacturer.DisplayName) ? item.Manufacturer.DisplayName : item.Manufacturer.Name,
                        Description = item.Manufacturer.Description
                    } : new ManufacturerVM()
                } : new ProductVM(),

                LicenseKeySingle = item.LicenseKeySingle,
                OriginalLicenseKeySingle = item.LicenseKeySingle,

                Quantity = item.Assets != null ? item.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false).Count() : 0,
                Active = item.Assets != null ? item.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false && x.Status != null && x.Status.Group?.ToLower() == "active").Count() : 0,
                InActive = item.Assets != null ? item.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false && x.Status != null && x.Status.Group?.ToLower() != "active").Count() : 0,
                Assigned = item.Assets != null ? item.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false && x.Status != null && x.Status.Name?.ToLower() == "assigned").Count() : 0,
                Available = item.Assets != null ? item.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false && x.Status != null && x.Status.Name?.ToLower() == "available").Count() : 0,
                Shared = item.Assets != null ? item.Assets.Where(x => x.InvoiceItemId == item.Id && x.IsDeleted == false && x.Status != null && x.Status.Name?.ToLower() == "shared").Count() : 0,

                UnitPrice = item.UnitPrice,
                ExpirationDate = item.ExpirationDate,
                Specifications = item.Specifications,
                Notes = item.Notes,

                DateReceived = DateTime.Now,


                CreatedBy = item.CreatedBy,
                ModifiedBy = item.ModifiedBy,
                DateAdded = item.DateAdded,
                DateModified = item.DateModified
            } : null;

            if (item != null)
            {
                vm.SupplierName = vm.Invoice.Supplier.DisplayName;
                vm.QuantityOriginal = vm.Quantity;
                vm.Invoices = await InvoiceLogic.GetInvoices();
                vm.Suppliers = await SupplierLogic.GetSuppliers();

                vm.AssetTypeName = vm.AssetType.Name;
                vm.LicenseTypeName = vm.LicenseType.Name;
                vm.LicenseTypes = await AssetLogic.GetLicenseTypes();
                vm.InvoiceNumberAndItemNumber = vm.Invoice.Id.ToString() + "-" + vm.InvoiceItemNumber.ToString();

                vm.AssetTypes = await CommonLogic.GetAssetTypes();
                vm.AssetCategories = await AssetLogic.GetAssetCategoriesByType(vm.AssetTypeId);
                vm.Manufacturers = await ManuLogic.GetManufacturers(vm.AssetTypeId, vm.AssetCategoryId);
                vm.Products = await ProductLogic.GetProducts(vm.ManuId, vm.AssetTypeId, vm.AssetCategoryId);

                //Defaults
                vm.Users = await UserLogic.GetUsers();
                var Assets = await AssetLogic.GetComputerTypeAssets(null);
                vm.Assets = Assets.OrderBy(x => x.Serial);
                vm.Locations = await LocationLogic.GetLocations();
                vm.Buildings = new List<BuildingVM>();
                vm.Rooms = new List<RoomVM>();

            }

            return vm;
        }

        //Save Invoice Item
        public async Task SaveInvoiceItem(InvoiceItemVM vm)
        {
            if (vm.InvoiceId != vm.OriginalInvoiceId || vm.InvoiceId != null && vm.InvoiceItemNumber == null)
            {
                vm.InvoiceItemNumber = await GetMinInvoiceItemNumber(vm.InvoiceId ?? 0) + 1;
            }

            if (vm.ProductId != vm.OriginalProductId)
            {
                var product = await ProductLogic.Get(vm.ProductId ?? 0);
                if (product != null)
                {
                    vm.ManuId = product.ManuId;
                }
            }

            var statuses = await Db.AssetStatuses.ToListAsync();
            var statusAvailableId = statuses?.Where(x => x.Name == "Available").Select(x => x.Id).FirstOrDefault();
            var statusAssignedId = statuses?.Where(x => x.Name == "Assigned").Select(x => x.Id).FirstOrDefault();
            var statusSharedId = statuses?.Where(x => x.Name == "Shared").Select(x => x.Id).FirstOrDefault();

            var invoiceItem = new InvoiceItem()
            {
                Id = vm.Id,
                InvoiceId = vm.InvoiceId,
                InvoiceItemNumber = vm.InvoiceItemNumber,
                AssetTypeId = vm.AssetTypeId,
                AssetCategoryId = vm.AssetCategoryId,
                ManuId = vm.ManuId,
                ProductId = vm.ProductId,
                LicenseTypeId = vm.LicenseTypeId,
                LicenseKeySingle = vm.LicenseKeySingle,
                UnitPrice = vm.UnitPrice,
                ExpirationDate = vm.ExpirationDate,
                Specifications = vm.Specifications,
                Notes = vm.Notes,
                ModifiedBy = vm.ModifiedBy
            };

            var invoiceItemEntry = Db.Entry(invoiceItem);
            invoiceItemEntry.State = EntityState.Modified;
            Db.Entry(invoiceItem).Property(x => x.CreatedBy).IsModified = false;
            Db.Entry(invoiceItem).Property(x => x.DateAdded).IsModified = false;

            if (vm.LicenseTypeId != vm.LicenseTypeIdOriginal && vm.LicenseType != null)
            {
                var assets = await Db.Assets.Where(x => x.InvoiceItemId == vm.Id).ToListAsync();

                if (assets != null)
                {
                    //not a user type license, meaning its a single or multi
                    if (vm.LicenseType.Name != "User")
                    {
                        foreach (var asset in assets)
                        {
                            if (asset.AssignedAssetId == null)
                            {
                                asset.AssignedDate = null;
                            }
                            else if (asset.AssignedDate == null)
                            {
                                asset.AssignedDate = DateTime.Now;
                            }

                            asset.StatusId = asset.AssignedAssetId != null ? statusSharedId : statusAvailableId;
                            asset.AssignedUserId = null;
                            asset.LicenseKeyMulti = null;
                            asset.ModifiedBy = vm.ModifiedBy;
                            var assetEntry = Db.Entry(asset);
                            assetEntry.State = EntityState.Modified;
                            Db.Entry(invoiceItem).Property(x => x.CreatedBy).IsModified = false;
                            Db.Entry(invoiceItem).Property(x => x.DateAdded).IsModified = false;
                        }
                    }
                    else
                    {
                        foreach (var asset in assets)
                        {
                            if (asset.AssignedUserId == null)
                            {
                                asset.AssignedDate = null;
                            }
                            else if (asset.AssignedDate == null)
                            {
                                asset.AssignedDate = DateTime.Now;
                            }

                            asset.StatusId = statusAvailableId;
                            asset.AssignedAssetId = null;
                            asset.LicenseKeyMulti = null;
                            asset.ModifiedBy = vm.ModifiedBy;
                            var assetEntry = Db.Entry(asset);
                            assetEntry.State = EntityState.Modified;
                            Db.Entry(invoiceItem).Property(x => x.CreatedBy).IsModified = false;
                            Db.Entry(invoiceItem).Property(x => x.DateAdded).IsModified = false;
                        }
                    }
                }
            }
            await Db.SaveChangesAsync();

            //If InvoiceItem - Asset Category has changed to a non-PC category type, then remove Connected and Assigned Assets that link to it.
            var assetCategoryName = await Db.AssetCategories.Where(x => x.Id == vm.AssetCategoryId).Select(x => x.Name).FirstOrDefaultAsync();

            if (vm.AssetCategoryId != vm.OriginalAssetCategoryId && ComputerTypeList.Contains(vm.OriginalAssetCategoryName) && !ComputerTypeList.Contains(assetCategoryName))
            {
                var connectedAssets = await Db.Assets
                                              .Include(x => x.ConnectedAsset)
                                              .Where(x => x.ConnectedAsset != null && x.ConnectedAsset.InvoiceItemId == vm.Id)
                                              .ToListAsync();

                if (connectedAssets != null)
                {
                    foreach (var asset in connectedAssets)
                    {
                        asset.ConnectedAssetId = null;
                        asset.ModifiedBy = vm.ModifiedBy;
                        var assetEntry = Db.Entry(asset);
                        assetEntry.State = EntityState.Modified;
                        Db.Entry(invoiceItem).Property(x => x.CreatedBy).IsModified = false;
                        Db.Entry(invoiceItem).Property(x => x.DateAdded).IsModified = false;
                    }
                }

                var assignedAssets = await Db.Assets
                                             .Include(x => x.AssignedAsset)
                                             .Where(x => x.AssignedAsset != null && x.AssignedAsset.InvoiceItemId == vm.Id)
                                             .ToListAsync();

                if (assignedAssets != null)
                {
                    foreach (var asset in assignedAssets)
                    {
                        asset.StatusId = statusAvailableId;
                        asset.AssignedAssetId = null;
                        asset.ModifiedBy = vm.ModifiedBy;
                        var assetEntry = Db.Entry(asset);
                        assetEntry.State = EntityState.Modified;
                        Db.Entry(invoiceItem).Property(x => x.CreatedBy).IsModified = false;
                        Db.Entry(invoiceItem).Property(x => x.DateAdded).IsModified = false;
                    }
                }
            }
            await Db.SaveChangesAsync();
            ///

            if ((vm.Quantity ?? 0) > (vm.QuantityOriginal ?? 0))
            {
                var vmAsset = new AssetVM()
                {
                    InvoiceItemId = vm.Id,
                    AssetTag = null,
                    Serial = vm.Serial,
                    LicenseKeyMulti = vm.LicenseKeyMulti,
                    MacAddress = null,
                    DateReceived = vm.DateReceived,
                    LocationId = vm.AssetLocationId,
                    BuildingId = vm.AssetBuildingId,
                    RoomId = vm.AssetRoomId,
                    AssignedAssetId = vm.AssignedAssetId,
                    AssignedUserId = vm.AssignedUserId,
                    CreatedBy = vm.ModifiedBy
                };
                var quantity = ((vm.Quantity ?? 0) - (vm.QuantityOriginal ?? 0));
                await AssetLogic.CreateAssets(vmAsset, quantity);
            }
        }

        //Delete Invoice Item
        public async Task DeleteInvoiceItem(InvoiceItemVM vm)
        {
            var invoiceItem = await Db.InvoiceItems.FindAsync(vm.Id);

            if (invoiceItem != null)
            {
                invoiceItem.IsDeleted = true;
                invoiceItem.ModifiedBy = vm.ModifiedBy;

                var invoiceItemEntry = Db.Entry(invoiceItem);
                invoiceItemEntry.State = EntityState.Modified;
                Db.Entry(invoiceItem).Property(x => x.CreatedBy).IsModified = false;
                Db.Entry(invoiceItem).Property(x => x.DateAdded).IsModified = false;
                await Db.SaveChangesAsync();
            }
        }

        //Get Min Invoice Item Number
        public async Task<int> GetMinInvoiceItemNumber(int invoiceId)
        {

            var invoiceItemNumbers = await Db.InvoiceItems
                                             .Where(x => x.InvoiceId == invoiceId 
                                                    && x.IsDeleted == false 
                                                    && x.InvoiceItemNumber > 0)
                                            .OrderBy(x => x.InvoiceItemNumber)
                                            .Select(x => x.InvoiceItemNumber)
                                            .ToListAsync();

            if (invoiceItemNumbers == null)
                return 0;

            var count = 0;
            foreach (var item in invoiceItemNumbers)
            {
                if (item != (count + 1))
                    return count;
                count++;
            }
            return invoiceItemNumbers.Max() ?? 0;
        }

        //Check Manufacturer and Product for invoice item
        public async Task<bool> CheckManuandProduct(int invoiceId, int manuId, int productId)
        {
            var invoiceItem = await Db.InvoiceItems
                                      .Where(x => x.IsDeleted == false
                                             && x.InvoiceId == invoiceId
                                             && x.ManuId == manuId
                                             && x.ProductId == productId)
                                      .FirstOrDefaultAsync();
            if (invoiceItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


    }
}