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
    public class SupplierLogic : AssetBaseLogic, ISupplierLogic
    {
        //Constructor
        public SupplierLogic(InventoryDbContext db)
        {
            Db = db;
        }

        //Get List
        public async Task<IEnumerable<SupplierVM>> GetList(string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000)
        {
            int pageNumber = page;
            var vmList = new List<SupplierVM>();

            //
            var query = Db.Suppliers.Where(x => x.IsDeleted == false);
            //

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => x.Name.Contains(searchString) || x.DisplayName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "id":
                    query = query.OrderBy(s => s.Id);
                    break;

                case "id_desc":
                    query = query.OrderByDescending(s => s.Id);
                    break;

                case "displayname":
                    query = query.OrderBy(s => s.DisplayName ?? s.Name ?? "zzz");
                    break;

                case "displayname_desc":
                    query = query.OrderByDescending(s => s.DisplayName ?? s.Name);
                    break;

                case "phone":
                    query = query.OrderBy(s => s.Phone ?? "zzz");
                    break;

                case "phone_desc":
                    query = query.OrderByDescending(s => s.Phone ?? "");
                    break;

                case "email":
                    query = query.OrderBy(s => s.Email ?? "zzz");
                    break;

                case "email_desc":
                    query = query.OrderByDescending(s => s.Email ?? "");
                    break;

                case "url":
                    query = query.OrderBy(s => s.Url ?? "");
                    break;

                case "url_desc":
                    query = query.OrderByDescending(s => s.Url ?? "");
                    break;

                default:
                    query = query = query.OrderBy(s => s.DisplayName ?? s.Name ?? "zzz");

                    break;
            }

            var List = await query.ToPagedListAsync(page, pageSize);

            var items = (IPagedList)List;

            foreach (var item in List)
            {
                var vm = new SupplierVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,
                    Description = item.Description,
                    ContactName = item.ContactName,
                    Email = item.Email,
                    Phone = item.Phone,
                    Url = item.Url,
                    AddressLine1 = item.AddressLine1,
                    AddressLine2 = item.AddressLine2,
                    City = item.City,

                    StateId = item.StateId,
                    State = item.State != null ? new StateVM()
                    {
                        Id = item.State.Id,
                        Code = item.State.Code,
                        Name = item.State.Name,
                        Active = item.State.Active
                    } : new StateVM(),

                    PostalCode = item.PostalCode,

                    Active = item.Active,
                    Notes = item.Notes
                };
                vmList.Add(vm);
            };

            StaticPagedList<SupplierVM> list = new StaticPagedList<SupplierVM>(vmList, items.PageNumber, items.PageSize, items.TotalItemCount);

            return list;
        }

        //Get Suppliers
        public async Task<IEnumerable<SupplierVM>> GetSuppliers()
        {
            var vmSuppliersList = new List<SupplierVM>();

            var suppliersList = await Db.Suppliers.Where(x => x.IsDeleted == false && x.Active == true).OrderBy(r => r.Name).ToListAsync();

            foreach (var item in suppliersList)
            {
                var vmSupplier = item != null ? new SupplierVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,
                    Description = item.Description,
                    ContactName = item.ContactName,
                    Email = item.Email,
                    Phone = item.Phone,
                    Url = item.Url,
                    AddressLine1 = item.AddressLine1,
                    AddressLine2 = item.AddressLine2,
                    City = item.City,

                    StateId = item.StateId,
                    State = item.State != null ? new StateVM()
                    {
                        Id = item.State.Id,
                        Code = item.State.Code,
                        Name = item.State.Name,
                        Active = item.State.Active
                    } : new StateVM(),

                    PostalCode = item.PostalCode,

                    Active = item.Active,
                    Notes = item.Notes
                } : new SupplierVM();
                vmSuppliersList.Add(vmSupplier);
            };
            return vmSuppliersList.OrderBy(x => x.DisplayName ?? x.Name);
        }

        //Create
        public async Task<int> Create(SupplierVM vm)
        {
            var item = new Supplier()
            {
                Name = vm.Name?.Trim(),
                DisplayName = vm.DisplayName?.Trim(),
                Description = vm.Description?.Trim(),
                ContactName = vm.ContactName?.Trim(),
                Email = vm.Email?.Trim().ToLower(),
                Phone = vm.Phone?.Trim(),
                Url = vm.Url?.Trim().ToLower(),
                AddressLine1 = vm.AddressLine1?.Trim(),
                AddressLine2 = vm.AddressLine2?.Trim(),
                City = vm.City?.Capitalize(),
                StateId = vm.StateId,
                PostalCode = vm.PostalCode,
                Active = vm.Active,
                Notes = vm.Notes?.Trim(),
                IsDeleted = false,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.CreatedBy
            };

            var result = Db.Suppliers.Add(item);
            await Db.SaveChangesAsync();

            return result.Id;
        }

        //Get
        public async Task<SupplierVM> Get(int Id)
        {
            var item = await Db.Suppliers
                               .Include(x => x.State)
                               .Where(x => x.IsDeleted == false && x.Id == Id)
                               .FirstOrDefaultAsync();

            if (item == null)
            {
                return null;
            }

            var vm = new SupplierVM()
            {
                Id = item.Id,
                Name = item.Name,
                OriginalName = item.Name,
                DisplayName = item.DisplayName,
                OriginalDisplayName = item.DisplayName,
                Description = item.Description,
                ContactName = item.ContactName,
                Email = item.Email,
                Phone = item.Phone,
                Url = item.Url,
                AddressLine1 = item.AddressLine1,
                AddressLine2 = item.AddressLine2,
                City = item.City,

                StateId = item.StateId,
                State = item.State != null ? new StateVM()
                {
                    Id = item.State.Id,
                    Code = item.State.Code,
                    Name = item.State.Name,
                    Active = item.State.Active
                } : new StateVM(),

                PostalCode = item.PostalCode,

                Active = item.Active,
                Notes = item.Notes,
                InvoiceCount = item.Invoices.Count(x => x.IsDeleted == false),

                ModifiedBy = item.ModifiedBy,
                DateModified = item.DateModified
            };

            return vm;
        }

        //Save
        public async Task Save(SupplierVM vm)
        {
            var item = new Supplier()
            {
                Id = vm.Id,
                Name = vm.Name?.Trim(),
                DisplayName = vm.DisplayName?.Trim(),
                Description = vm.Description?.Trim(),
                ContactName = vm.ContactName?.Trim(),
                Email = vm.Email?.Trim().ToLower(),
                Phone = vm.Phone?.Trim(),
                Url = vm.Url?.Trim().ToLower(),
                AddressLine1 = vm.AddressLine1?.Trim(),
                AddressLine2 = vm.AddressLine2?.Trim(),
                City = vm.City?.Capitalize(),
                StateId = vm.StateId,
                PostalCode = vm.PostalCode,
                Active = vm.Active,
                Notes = vm.Notes?.Trim(),
                ModifiedBy = vm.ModifiedBy,
            };

            var entry = Db.Entry(item);
            entry.State = EntityState.Modified;
            Db.Entry(item).Property(x => x.CreatedBy).IsModified = false;
            Db.Entry(item).Property(x => x.DateAdded).IsModified = false;
            await Db.SaveChangesAsync();
        }

        //Delete
        public async Task Delete(SupplierVM vm)
        {
            var item = await Db.Suppliers.FindAsync(vm.Id);

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