using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PagedList;
using PagedList.EntityFramework;
using Inventory.Data.Models;
using Inventory.Data.Services;
using Inventory.Web.Common;
using Inventory.Web.ViewModels;

namespace Inventory.Web.Logic
{
    public class ManuLogic : AssetBaseLogic, IManuLogic
    {
        //Constructor
        public ManuLogic(InventoryDbContext db)
        {
            Db = db;
        }

        //Get List
        public async Task<IEnumerable<ManufacturerVM>> GetList(string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000)
        {
            int pageNumber = page;
            var vmList = new List<ManufacturerVM>();

            //
            var query = Db.Manufacturers
                          .Include(x => x.Products)
                          .Where(x => x.IsDeleted == false);
            //

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => x.Name.Contains(searchString) || x.DisplayName.Contains(searchString));
            }

            var List = await query.ToListAsync();

            foreach (var item in List)
            {
                var vm = new ManufacturerVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,
                    Description = item.Description,
                    Active = item.Active,
                    Notes = item.Notes,
                    SupportEmail = item.SupportEmail,
                    SupportPhone = item.SupportPhone,
                    SupportUrl = item.SupportUrl,
                    ProductCount = item.Products.Where(x => x.IsDeleted == false).Count()
                };
                vmList.Add(vm);
            };

            switch (sortOrder)
            {
                case "id":
                    vmList = vmList.OrderBy(s => s.Id).ToList();
                    break;

                case "id_desc":
                    vmList = vmList.OrderByDescending(s => s.Id).ToList();
                    break;

                case "displayname":
                    vmList = vmList.OrderBy(s => s.DisplayName ?? s.Name ?? "zzz").ToList();
                    break;

                case "displayname_desc":
                    vmList = vmList.OrderByDescending(s => s.DisplayName ?? s.Name).ToList();
                    break;

                case "phone":
                    vmList = vmList.OrderBy(s => s.SupportPhone ?? "zzz")
                                   .ThenBy(s => s.DisplayName ?? s.Name ?? "zzz").ToList();
                    break;

                case "phone_desc":
                    vmList = vmList.OrderByDescending(s => s.SupportPhone ?? "")
                                   .ThenBy(s => s.DisplayName ?? s.Name ?? "zzz").ToList();
                    break;

                case "email":
                    vmList = vmList.OrderBy(s => s.SupportEmail ?? "zzz").ToList();
                    break;

                case "email_desc":
                    vmList = vmList.OrderByDescending(s => s.SupportEmail ?? "")
                                   .ThenBy(s => s.DisplayName ?? s.Name ?? "zzz").ToList();
                    break;

                case "url":
                    vmList = vmList.OrderBy(s => s.SupportUrl ?? "zzz")
                                   .ThenBy(s => s.DisplayName ?? s.Name ?? "zzz").ToList();
                    break;

                case "url_desc":
                    vmList = vmList.OrderByDescending(s => s.SupportUrl ?? "").ToList();
                    break;

                case "productcount":
                    vmList = vmList.OrderBy(s => s.ProductCount)
                                   .ThenBy(s => s.DisplayName ?? s.Name ?? "zzz").ToList();
                    break;

                case "productcount_desc":
                    vmList = vmList.OrderByDescending(s => s.ProductCount)
                                   .ThenBy(s => s.DisplayName ?? s.Name ?? "zzz").ToList();
                    break;

                default:
                    vmList = vmList.OrderBy(s => s.DisplayName ?? s.Name ?? "zzz").ToList();
                    break;
            }
            
            var items = vmList.ToPagedList(pageNumber, pageSize);

            StaticPagedList<ManufacturerVM> list = new StaticPagedList<ManufacturerVM>(items, items.PageNumber, items.PageSize, items.TotalItemCount);

            return list;
        }

        //Get Manufacturers
        public async Task<IEnumerable<ManufacturerVM>> GetManufacturers(int? assetTypeId, int? assetCategoryId)
        {
            var vmManufacturerList = new List<ManufacturerVM>();

            var query = Db.Manufacturers.Where(x => x.IsDeleted == false && x.Active == true).AsQueryable();

            if (assetTypeId != null)
            {
                query = query.Where(x => x.Products.Any(t => t.AssetTypeId == assetTypeId)).AsQueryable();
            }
            if (assetCategoryId != null)
            {
                query = query.Where(x => x.Products.Any(t => t.AssetCategoryId == assetCategoryId)).AsQueryable();
            }

            var manufacturerList = await query.OrderBy(r => r.Name).ToListAsync();

            foreach (var item in manufacturerList)
            {
                var vmManufacturer = item != null ? new ManufacturerVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,
                    Description = item.Description,
                    Active = item.Active
                } : new ManufacturerVM();
                vmManufacturerList.Add(vmManufacturer);
            };
            return vmManufacturerList.OrderBy(x => x.DisplayName ?? x.Name);
        }

        //Create
        public async Task<int> Create(ManufacturerVM vm)
        {
            var item = new Manufacturer()
            {
                Name = vm.Name?.Trim(),
                DisplayName = vm.DisplayName?.Trim(),
                Description = vm.Description?.Trim(),
                SupportEmail = vm.SupportEmail?.Trim().ToLower(),
                SupportPhone = vm.SupportPhone?.Trim(),
                SupportUrl = vm.SupportUrl?.Trim().ToLower(),
                Active = vm.Active,
                Notes = vm.Notes?.Trim(),
                IsDeleted = false,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.CreatedBy
            };

            var result = Db.Manufacturers.Add(item);
            await Db.SaveChangesAsync();

            return result.Id;
        }

        //Get
        public async Task<ManufacturerVM> Get(int Id)
        {
            var item = await Db.Manufacturers
                               .Where(x => x.IsDeleted == false && x.Id == Id)
                               .FirstOrDefaultAsync();

            if (item == null)
            {
                return null;
            }

            var vm = new ManufacturerVM()
            {
                Id = item.Id,
                Name = item.Name,
                OriginalName = item.Name,
                DisplayName = item.DisplayName,
                OriginalDisplayName = item.DisplayName,
                Description = item.Description,
                Active = item.Active,
                Notes = item.Notes,
                SupportEmail = item.SupportEmail,
                SupportPhone = item.SupportPhone,
                SupportUrl = item.SupportUrl,
                ProductCount = item.Products.Count(x => x.IsDeleted == false),
                InvoiceItemCount = item.InvoiceItems.Count(x => x.IsDeleted == false),

                CreatedBy = item.CreatedBy,
                ModifiedBy = item.ModifiedBy,
                DateAdded = item.DateAdded,
                DateModified = item.DateModified
            };

            return vm;
        }

        //Save
        public async Task Save(ManufacturerVM vm)
        {
            var item = new Manufacturer()
            {
                Id = vm.Id,
                Name = vm.Name?.Trim(),
                DisplayName = vm.DisplayName?.Trim(),
                Description = vm.Description?.Trim(),
                SupportEmail = vm.SupportEmail?.Trim().ToLower(),
                SupportPhone = vm.SupportPhone?.Trim(),
                SupportUrl = vm.SupportUrl?.Trim().ToLower(),
                Active = vm.Active,
                Notes = vm.Notes?.Trim(),
                ModifiedBy = vm.ModifiedBy
            };

            var entry = Db.Entry(item);
            entry.State = EntityState.Modified;
            Db.Entry(item).Property(x => x.CreatedBy).IsModified = false;
            Db.Entry(item).Property(x => x.DateAdded).IsModified = false;
            await Db.SaveChangesAsync();
        }

        //Delete
        public async Task Delete(ManufacturerVM vm)
        {
            var item = await Db.Manufacturers.FindAsync(vm.Id);

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
    }
}