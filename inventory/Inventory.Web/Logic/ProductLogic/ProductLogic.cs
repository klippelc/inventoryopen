using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PagedList;
using PagedList.EntityFramework;
using Inventory.Data.Models;
using Inventory.Data.Services;
using Inventory.Web.Common;
using Inventory.Web.ViewModels;

namespace Inventory.Web.Logic
{
    public class ProductLogic : AssetBaseLogic, IProductLogic
    {
        //Constructor
        public ProductLogic(InventoryDbContext db, IManuLogic manuLogic, IAssetLogic assetLogic, ICommonLogic commonLogic)
        {
            Db = db;
            ManuLogic = manuLogic;
            AssetLogic = assetLogic;
            CommonLogic = commonLogic;
        }

        //Get List
        public async Task<IEnumerable<ProductVM>> GetList(int? manuId, string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000)
        {
            int pageNumber = page;
            var vmList = new List<ProductVM>();

            //
            var query = Db.Products
                          .Include(x => x.Manufacturer)
                          .Include(x => x.AssetType)
                          .Include(x => x.AssetCategory)
                          .Where(x => x.IsDeleted == false);
            //

            if (manuId != null)
            {
                query = query.Where(x => x.ManuId == manuId).AsQueryable();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => (string.IsNullOrEmpty(x.DisplayName) ? x.Name.Contains(searchString) : x.DisplayName.Contains(searchString))
                                      || ((x.Manufacturer != null) && (string.IsNullOrEmpty(x.Manufacturer.DisplayName) ? x.Manufacturer.Name.Contains(searchString) : x.Manufacturer.DisplayName.Contains(searchString))));
            }

            switch (sortOrder)
            {
                case "assettype":
                    query = query.OrderBy(s => s.AssetType != null ? s.AssetType.Name : "zzz")
                                 .ThenBy(s => s.AssetCategory != null ? s.AssetCategory.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz");
                    break;

                case "assettype_desc":
                    query = query.OrderByDescending(s => s.AssetType != null ? s.AssetType.Name : "")
                                 .ThenBy(s => s.AssetCategory != null ? s.AssetCategory.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz");
                    break;

                case "assetcat":
                    query = query.OrderBy(s => s.AssetCategory != null ? s.AssetCategory.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz");
                    break;

                case "assetcat_desc":
                    query = query.OrderByDescending(s => s.AssetCategory != null ? s.AssetCategory.Name ?? "" : "")
                                 .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz");
                    break;

                case "manu":
                    query = query.OrderBy(s => s.Manufacturer != null ? (s.Manufacturer.DisplayName != null && s.Manufacturer.DisplayName != "") ? s.Manufacturer.DisplayName : s.Manufacturer.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz");
                    break;

                case "manu_desc":
                    query = query.OrderByDescending(s => s.Manufacturer != null ? (s.Manufacturer.DisplayName != null && s.Manufacturer.DisplayName != "") ? s.Manufacturer.DisplayName : s.Manufacturer.Name ?? "" : "")
                                 .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz");
                    break;

                case "displayname":
                    query = query.OrderBy(s => (s.DisplayName != null && s.DisplayName !="") ? s.DisplayName : s.Name ?? "zzz");
                    break;

                case "displayname_desc":
                    query = query.OrderByDescending(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "");
                    break;

                default:
                    query = query.OrderBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz");
                    break;
            }

            var List = await query.ToPagedListAsync(page, pageSize);

            var items = (IPagedList)List;

            foreach (var item in List)
            {
                var vm = new ProductVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,
                    Description = item.Description,
                    Active = item.Active,
                    Notes = item.Notes,

                    AssetTypeId = item.AssetTypeId,
                    OriginalAssetTypeId = item.AssetTypeId,
                    AssetType = item.AssetType != null ? new AssetTypeVM()
                    {
                        Id = item.AssetType.Id,
                        Name = item.AssetType.Name,
                        Description = item.AssetType.Description,
                        IconCss = item.AssetType.IconCss
                    } : new AssetTypeVM(),

                    AssetCategoryId = item.AssetCategoryId,
                    OriginalAssetCategoryId = item.AssetCategoryId,
                    AssetCategory = item.AssetCategory != null ? new AssetCategoryVM()
                    {
                        Id = item.AssetCategory.Id,
                        Name = item.AssetCategory.Name,
                        Description = item.AssetCategory.Description,
                        IconCss = item.AssetCategory.IconCss
                    } : new AssetCategoryVM(),

                    ManuId = item.ManuId,
                    OriginalManuId = item.ManuId,
                    Manufacturer = item.Manufacturer != null ? new ManufacturerVM()
                    {
                        Id = item.Manufacturer.Id,
                        Name = item.Manufacturer.Name,
                        DisplayName = !string.IsNullOrEmpty(item.Manufacturer.DisplayName) ? item.Manufacturer.DisplayName : item.Manufacturer.Name,
                        Description = item.Manufacturer.Description,
                        Active = item.Manufacturer.Active
                    } : new ManufacturerVM(),
                };
                vm.AssetTypeName = vm.AssetType.Name;
                vm.AssetCategoryName = vm.AssetCategory.Name;
                vm.ManufacturerName = vm.Manufacturer.Name;

                vmList.Add(vm);
            };

            StaticPagedList<ProductVM> list = new StaticPagedList<ProductVM>(vmList, items.PageNumber, items.PageSize, items.TotalItemCount);

            return list;
        }

        //Get Products
        public async Task<IEnumerable<ProductVM>> GetProducts(int? manuId, int? assetTypeId, int? assetCategoryId)
        {
            var vmProductList = new List<ProductVM>();

            var query = Db.Products.Where(x => x.IsDeleted == false && x.Active == true).AsQueryable();

            if (manuId != null)
            {
                query = query.Where(x => x.ManuId == manuId).AsQueryable();
            }

            if (assetTypeId != null)
            {
                query = query.Where(x => x.AssetTypeId == assetTypeId).AsQueryable();
            }

            if (assetCategoryId != null)
            {
                query = query.Where(x => x.AssetCategoryId == assetCategoryId).AsQueryable();
            }

            var productList = await query.OrderBy(r => r.Name).ToListAsync();

            foreach (var item in productList)
            {
                var vmProduct = item != null ? new ProductVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,
                    Description = item.Description,
                    Active = item.Active
                } : new ProductVM();
                vmProductList.Add(vmProduct);
            };
            return vmProductList.OrderBy(x => x.DisplayName ?? x.Name);
        }

        //Create
        public async Task<int> Create(ProductVM vm)
        {
            var item = new Product()
            {
                AssetTypeId = vm.AssetTypeId,
                AssetCategoryId = vm.AssetCategoryId,
                ManuId = vm.ManuId,

                Name = vm.Name?.Trim(),
                DisplayName = vm.DisplayName?.Trim(),
                Description = vm.Description?.Trim(),

                Active = vm.Active,
                Notes = vm.Notes?.Trim(),
                IsDeleted = false,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.CreatedBy
            };

            var result = Db.Products.Add(item);
            await Db.SaveChangesAsync();

            return result.Id;
        }

        //Get
        public async Task<ProductVM> Get(int Id)
        {
            var item = await Db.Products
                               .Include(x => x.AssetType)
                               .Include(x => x.AssetCategory)
                               .Include(x => x.Manufacturer)
                               .Where(x => x.IsDeleted == false && x.Id == Id)
                               .FirstOrDefaultAsync();

            if (item == null)
            {
                return null;
            }

            var vm = new ProductVM()
            {
                Id = item.Id,
                Name = item.Name,
                OriginalName = item.Name,
                DisplayName = item.DisplayName,
                OriginalDisplayName = item.DisplayName,
                Description = item.Description,
                Active = item.Active,
                Notes = item.Notes,

                AssetTypeId = item.AssetTypeId,
                OriginalAssetTypeId = item.AssetTypeId,
                AssetType = item.AssetType != null ? new AssetTypeVM()
                {
                    Id = item.AssetType.Id,
                    Name = item.AssetType.Name,
                    Description = item.AssetType.Description,
                    IconCss = item.AssetType.IconCss
                } : new AssetTypeVM(),

                AssetCategoryId = item.AssetCategoryId,
                OriginalAssetCategoryId = item.AssetCategoryId,
                AssetCategory = item.AssetCategory != null ? new AssetCategoryVM()
                {
                    Id = item.AssetCategory.Id,
                    Name = item.AssetCategory.Name,
                    Description = item.AssetCategory.Description,
                    IconCss = item.AssetCategory.IconCss
                } : new AssetCategoryVM(),

                ManuId = item.ManuId,
                OriginalManuId = item.ManuId,
                Manufacturer = item.Manufacturer != null ? new ManufacturerVM()
                {
                    Id = item.Manufacturer.Id,
                    Name = item.Manufacturer.Name,
                    DisplayName = !string.IsNullOrEmpty(item.Manufacturer.DisplayName) ? item.Manufacturer.DisplayName : item.Manufacturer.Name,
                    Description = item.Manufacturer.Description
                } : new ManufacturerVM(),

                InvoiceItemCount = item.InvoiceItems.Count(x => x.IsDeleted == false),

                CreatedBy = item.CreatedBy,
                ModifiedBy = item.ModifiedBy,
                DateAdded = item.DateAdded,
                DateModified = item.DateModified
            };
            vm.AssetTypeName = vm.AssetType.Name;
            vm.AssetCategoryName = vm.AssetCategory.Name;
            vm.ManufacturerName = vm.Manufacturer.DisplayName;

            vm.AssetTypes = await CommonLogic.GetAssetTypes();
            vm.AssetCategories = await AssetLogic.GetAssetCategories();
            vm.Manufacturers = await ManuLogic.GetManufacturers(null, null);

            return vm;
        }

        //Save
        public async Task Save(ProductVM vm)
        {
            var item = new Product()
            {
                Id = vm.Id,

                AssetTypeId = vm.AssetTypeId,
                AssetCategoryId = vm.AssetCategoryId,
                ManuId = vm.ManuId,

                Name = vm.Name?.Trim(),
                DisplayName = vm.DisplayName?.Trim(),
                Description = vm.Description?.Trim(),
                Active = vm.Active,
                Notes = vm.Notes?.Trim(),
                ModifiedBy = vm.ModifiedBy
            };

            var entry = Db.Entry(item);
            entry.State = EntityState.Modified;
            Db.Entry(item).Property(x => x.CreatedBy).IsModified = false;
            Db.Entry(item).Property(x => x.DateAdded).IsModified = false;
            await Db.SaveChangesAsync();

            if (vm.ManuId != vm.OriginalManuId)
            {
                await UpdateManufacturer(vm.ManuId, vm.Id, vm.ModifiedBy);
            }
        }

        //Delete
        public async Task Delete(ProductVM vm)
        {
            var item = await Db.Products.FindAsync(vm.Id);

            if (item != null)
            {
                item.IsDeleted = true;
                item.ModifiedBy = vm.ModifiedBy;
                var entry = Db.Entry(item);
                entry.State = EntityState.Modified;
                Db.Entry(item).Property(x => x.CreatedBy).IsModified = false;
                Db.Entry(item).Property(x => x.DateAdded).IsModified = false;
                await Db.SaveChangesAsync();
            }
        }

        //Update Manufacturer
        private async Task UpdateManufacturer(int? manuId, int productId, int? userId)
        {
            var items = await Db.InvoiceItems.Where(x => x.ProductId == productId).ToListAsync();

            foreach (var item in items)
            {
                item.ManuId = manuId;
                item.ModifiedBy = userId;
                var entry = Db.Entry(item);
                entry.State = EntityState.Modified;
                Db.Entry(item).Property(x => x.CreatedBy).IsModified = false;
                Db.Entry(item).Property(x => x.DateAdded).IsModified = false;
            }
            await Db.SaveChangesAsync();
        }
    }
}