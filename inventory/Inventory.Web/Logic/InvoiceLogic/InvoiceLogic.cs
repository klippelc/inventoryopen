using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PagedList;
using Inventory.Data.Models;
using Inventory.Data.Services;
using Inventory.Web.Common;
using Inventory.Web.ViewModels;
using System.Data.Entity;
using System;
using System.Text.RegularExpressions;

namespace Inventory.Web.Logic
{
    public class InvoiceLogic : AssetBaseLogic, IInvoiceLogic
    {
        //Constructor
        public InvoiceLogic(InventoryDbContext db, ISupplierLogic supplierLogic)
        {
            Db = db;
            SupplierLogic = supplierLogic;
        }

        //Get Invoices List
        public async Task<IEnumerable<InvoiceVM>> GetInvoiceList(string sortOrder = "", string currentFilter = "", string searchString = "", int page = 1, int pageSize = 10000000)
        {
            int pageNumber = page;
            var vmList = new List<InvoiceVM>();

            var query = Db.Invoices
                          .Include(x => x.Supplier)
                          .Where(x => x.IsDeleted == false)
                          .OrderBy(x => x.PONumber)
                          .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                var search = searchString.Trim();
                var numbersearch = searchString.RemoveSpaces();

                query = query.Where(s => (s.Id.ToString().Contains(search.ToLower())) 
                                      || (s.PONumber.ToLower().Contains(numbersearch.ToLower())) 
                                      || (s.Supplier != null && string.IsNullOrEmpty(s.Supplier.DisplayName) ? s.Supplier.Name.ToLower().Contains(search.ToLower()) : s.Supplier.DisplayName.ToLower().Contains(search.ToLower())));
            }  

            var InvoiceList = await query.ToListAsync();

            foreach (var item in InvoiceList)
            {
                var vm = item != null ? new InvoiceVM()
                {
                    Id = item.Id,
                    PONumber = item.PONumber,
                    PurchaseDate = item.PurchaseDate,
                    SupplierId = item.SupplierId,
                    Supplier = item.Supplier != null ? new SupplierVM()
                    {
                        Id = item.Supplier.Id,
                        Name = item.Supplier.Name,
                        DisplayName = !string.IsNullOrEmpty(item.Supplier.DisplayName) ? item.Supplier.DisplayName : item.Supplier.Name,
                        Description = item.Supplier.Description
                    } : new SupplierVM(),

                    InvoiceItemsCount = item.InvoiceItems.Count(x => x.InvoiceId == item.Id && x.IsDeleted == false),
                    InvoiceItemsTotal = await GetInvoiceTotal(item.Id),
                    TotalPrice = item.TotalPrice ?? 0,

                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy,
                    DateAdded = item.DateAdded,
                    DateModified = item.DateModified
                } : null;

                vm.SupplierName = vm.Supplier.DisplayName;

                vmList.Add(vm);
            };

            switch (sortOrder)
            {
                case "invoiceno":
                    vmList = vmList.OrderBy(x => x.Id).ToList();
                    break;

                case "invoiceno_desc":
                    vmList = vmList.OrderByDescending(x => x.Id).ToList();
                    break;

                case "ponumber":
                    vmList = vmList.OrderBy(x => x.PONumber ?? "zzzzzzzzzz").ToList();
                    break;

                case "ponumber_desc":
                    vmList = vmList.OrderByDescending(x => x.PONumber).ToList();
                    break;

                case "supplier":
                    vmList = vmList.OrderBy(s => s.Supplier != null ? s.Supplier.Name ?? "zzzzzzzzzz" : "").ToList();
                    break;

                case "supplier_desc":
                    vmList = vmList.OrderByDescending(s => s.Supplier != null ? s.Supplier.Name : "").ToList();
                    break;

                case "invoiceitemscount":
                    vmList = vmList.OrderBy(s => s.InvoiceItemsCount ?? 999999999).ToList();
                    break;

                case "invoiceitemscount_desc":
                    vmList = vmList.OrderByDescending(s => s.InvoiceItemsCount).ToList();
                    break;

                case "invoiceitemstotal":
                    vmList = vmList.OrderBy(s => s.InvoiceItemsTotal ?? 999999999).ToList();
                    break;

                case "invoiceitemstotal_desc":
                    vmList = vmList.OrderByDescending(s => s.InvoiceItemsTotal).ToList();
                    break;

                case "price":
                    vmList = vmList.OrderBy(s => s.TotalPrice ?? 999999999).ToList();
                    break;

                case "price_desc":
                    vmList = vmList.OrderByDescending(s => s.TotalPrice).ToList();
                    break;

                case "purchasedate":
                    vmList = vmList.OrderBy(s => s.PurchaseDate ?? DateTime.MaxValue).ToList();
                    break;

                case "purchasedate_desc":
                    vmList = vmList.OrderByDescending(s => s.PurchaseDate).ToList();
                    break;

                default:
                    vmList = vmList.OrderBy(x => x.Id)
                                   .ThenBy(x => x.Supplier.Name).ToList();
                    break;
            }

            return vmList.ToPagedList(pageNumber, pageSize);
        }

        //Get Invoices
        public async Task<IEnumerable<InvoiceVM>> GetInvoices()
        {
            var vmInvoiceList = new List<InvoiceVM>();

            var query = Db.Invoices
                          .Include(x => x.Supplier)
                          .Where(x => x.IsDeleted == false)
                          .OrderBy(x => x.PONumber)
                          .AsQueryable();

            var InvoiceList = await query.ToListAsync();

            foreach (var item in InvoiceList)
            {
                var vm = item != null ? new InvoiceVM()
                {
                    Id = item.Id,
                    PONumber = item.PONumber.RemoveSpaces(),
                    PurchaseDate = item.PurchaseDate,

                    SupplierId = item.SupplierId,
                    Supplier = item.Supplier != null ? new SupplierVM()
                    {
                        Id = item.Supplier.Id,
                        Name = item.Supplier.Name,
                        DisplayName = !string.IsNullOrEmpty(item.Supplier.DisplayName) ? item.Supplier.DisplayName : item.Supplier.Name,
                        Description = item.Supplier.Description
                    } : new SupplierVM(),

                    InvoiceItemsCount = item.InvoiceItems != null ? item.InvoiceItems.Where(x => x.InvoiceId == item.Id && x.IsDeleted == false).Count() : 0,
                    InvoiceItemsTotal = item.InvoiceItems != null ? item.InvoiceItems.Where(x => x.InvoiceId == item.Id && x.IsDeleted == false).Sum(x => x.UnitPrice) : 0,
                    TotalPrice = item.TotalPrice,

                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy,
                    DateAdded = item.DateAdded,
                    DateModified = item.DateModified
                } : null;
                vm.SupplierName = vm.Supplier.DisplayName;

                vmInvoiceList.Add(vm);
            };

            return vmInvoiceList.OrderBy(x => x.Id);
        }

        //Get Blank Invoice
        public async Task<InvoiceVM> GetBlankInvoice()
        {
            var vm = new InvoiceVM
            {
                Suppliers = await SupplierLogic.GetSuppliers()
            };

            return vm;
        }

        //Create Invoice
        public async Task<InvoiceVM> CreateInvoice(InvoiceVM vm)
        {
            var invoice = new Invoice()
            {
                PONumber = vm.PONumber.RemoveSpaces(),
                SupplierId = vm.SupplierId,
                PurchaseDate = vm.PurchaseDate,
                TotalPrice = vm.TotalPrice,
                IsDeleted = false,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.CreatedBy
            };

            Db.Invoices.Add(invoice);
            await Db.SaveChangesAsync();

            var vmInvoice = new InvoiceVM()
            {
                Id = invoice.Id,
                PONumber = invoice.PONumber,
                PurchaseDate = invoice.PurchaseDate,
                SupplierId = invoice.SupplierId,
                Supplier = invoice.Supplier != null ? new SupplierVM()
                {
                    Id = invoice.Supplier.Id,
                    Name = invoice.Supplier.Name,
                    DisplayName = !string.IsNullOrEmpty(invoice.Supplier.DisplayName) ? invoice.Supplier.DisplayName : invoice.Supplier.Name,
                    Description = invoice.Supplier.Description
                } : new SupplierVM(),
            };

            return vmInvoice;
        }

        //Get Invoice
        public async Task<InvoiceVM> GetInvoice(int invoiceId)
        {
            var item = await Db.Invoices
                               .Include(x => x.Supplier)
                               .Where(x => x.Id == invoiceId).FirstOrDefaultAsync();

            var vm = item != null ? new InvoiceVM()
            {
                Id = item.Id,
                PONumber = item.PONumber.RemoveSpaces(),
                PONumberOriginal = item.PONumber.RemoveSpaces(),
                SupplierId = item.SupplierId,

                Supplier = item.Supplier != null ? new SupplierVM()
                {
                    Id = item.Supplier.Id,
                    Name = item.Supplier.Name,
                    DisplayName = !string.IsNullOrEmpty(item.Supplier.DisplayName) ? item.Supplier.DisplayName : item.Supplier.Name,
                    Description = item.Supplier.Description
                } : new SupplierVM(),

                PurchaseDate = item.PurchaseDate,
                TotalPrice = item.TotalPrice,
                InvoiceItemsCount = item.InvoiceItems.Count(x => x.IsDeleted == false),
                InvoiceItemsTotal = await GetInvoiceTotal(item.Id),

                DateAdded = item.DateAdded,
                CreatedBy = item.CreatedBy,
                Suppliers = await SupplierLogic.GetSuppliers()
            } : null;

            vm.SupplierIdOriginal = vm.SupplierId;
            vm.SupplierName = vm.Supplier.DisplayName;
            vm.SupplierDisplayNameOriginal = vm.Supplier.DisplayName;

            return vm;
        }

        //Save Invoice
        public async Task<InvoiceVM> SaveInvoice(InvoiceVM vm)
        {
            var invoice = new Invoice()
            {
                Id = vm.Id,
                PONumber = vm.PONumber.RemoveSpaces(),
                SupplierId = vm.SupplierId,
                TotalPrice = vm.TotalPrice,
                PurchaseDate = vm.PurchaseDate,
                ModifiedBy = vm.ModifiedBy
            };

            var entry = Db.Entry(invoice);
            entry.State = EntityState.Modified;
            Db.Entry(invoice).Property(x => x.CreatedBy).IsModified = false;
            Db.Entry(invoice).Property(x => x.DateAdded).IsModified = false;
            await Db.SaveChangesAsync();

            var vmInvoice = new InvoiceVM()
            {
                Id = invoice.Id,
                PONumber = invoice.PONumber,
                PONumberOriginal = invoice.PONumber,
                PurchaseDate = invoice.PurchaseDate,
                SupplierId = invoice.SupplierId,
                Supplier = invoice.Supplier != null ? new SupplierVM()
                {
                    Id = invoice.Supplier.Id,
                    Name = invoice.Supplier.Name,
                    Description = invoice.Supplier.Description
                } : new SupplierVM(),
            };

            return vmInvoice;
        }

        //Delete Invoice
        public async Task DeleteInvoice(InvoiceVM vm)
        {
            var invoice = await Db.Invoices.FindAsync(vm.Id);

            if (invoice != null)
            {
                invoice.IsDeleted = true;
                invoice.ModifiedBy = vm.ModifiedBy;

                var entry = Db.Entry(invoice);
                entry.State = EntityState.Modified;
                Db.Entry(invoice).Property(x => x.CreatedBy).IsModified = false;
                Db.Entry(invoice).Property(x => x.DateAdded).IsModified = false;
                await Db.SaveChangesAsync();
            }
        }

        //Calculate Invoice Total
        private async Task<decimal> GetInvoiceTotal(int invoiceId)
        {
            decimal total = 0;
            var assets = await Db.Assets
                                 .Where(x => x.IsDeleted == false 
                                        && x.InvoiceItem.IsDeleted == false 
                                        && x.InvoiceItem.InvoiceId == invoiceId)
                                 .GroupBy(x => x.InvoiceItemId)
                                 .Select(cl => new 
                                 {
                                     ItemTotal = (cl.Count() * cl.Max(c => c.InvoiceItem.UnitPrice ?? 0))
                                 })
                                 .ToListAsync();

            if (assets != null)
            {
                total = assets.Sum(x => x.ItemTotal);
            }

            return total;
        }

        //Check PO Number
        public async Task<bool> CheckPONumber(string poNumber)
        {
            poNumber = poNumber.RemoveSpaces()?.ToLower();
            var invoice = await Db.Invoices
                                  .Where(x => x.IsDeleted == false &&
                                         x.PONumber.ToLower() 
                                         == poNumber)
                                  .FirstOrDefaultAsync();
            if (invoice == null)
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