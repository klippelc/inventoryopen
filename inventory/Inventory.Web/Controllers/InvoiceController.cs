using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Inventory.Web.Common;
using Inventory.Web.Logic;
using Inventory.Web.ViewModels;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Inventory.Web.Controllers
{
    public class InvoiceController : AssetBaseMvcController
    {
        //Constructor
        public InvoiceController(IInvoiceLogic invoiceLogic, IAssetLogic assetLogic, ISupplierLogic supplierLogic, ICommonLogic commonLogic)
        {
            InvoiceLogic = invoiceLogic;
            AssetLogic = assetLogic;
            SupplierLogic = supplierLogic;
            CommonLogic = commonLogic;
            AssetType = "Invoice";
            ViewBag.PageName = "Purchase Order";
            ViewBag.AssetType = AssetType;
        }

        //Get Invoices
        [Authorize(Roles = "InvoicesView")]
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ModelState.Remove("SearchString");
            searchString = searchString?.Trim();

            sortOrder = string.IsNullOrEmpty(sortOrder) ? "product" : sortOrder;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.PONumberSortOrder = sortOrder == "ponumber" ? "ponumber_desc" : "ponumber";
            ViewBag.InvoiceNoSortOrder = sortOrder == "invoiceno" ? "invoiceno_desc" : "invoiceno";
            ViewBag.SupplierSortOrder = sortOrder == "supplier" ? "supplier_desc" : "supplier";
            ViewBag.InvoiceItemsCountSortOrder = sortOrder == "invoiceitemscount" ? "invoiceitemscount_desc" : "invoiceitemscount";
            ViewBag.InvoiceItemsTotalSortOrder = sortOrder == "invoiceitemstotal" ? "invoiceitemstotal_desc" : "invoiceitemstotal";
            ViewBag.PriceSortOrder = sortOrder == "price" ? "price_desc" : "price";
            ViewBag.PurchaseDateSortOrder = sortOrder == "purchasedate" ? "purchasedate_desc" : "purchasedate";

            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var vmList = await InvoiceLogic.GetInvoiceList(sortOrder, currentFilter, searchString, page, pageSize);

            return View(vmList);
        }

        [Authorize(Roles = "InvoicesView")]
        public async Task<ActionResult> ExportToExcel(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            var search = !string.IsNullOrEmpty(searchString) ? searchString : currentFilter;
            var vmInvoiceList = await InvoiceLogic.GetInvoiceList(sortOrder, currentFilter, search, 1, 9999999);

            var list = vmInvoiceList.Select(x => new {
                x.Id,
                x.PONumber,
                Supplier = x.SupplierName,
                ItemsCount = x.InvoiceItemsCount.ToString(),
                ItemsTotal = "$" + x.InvoiceItemsTotal,
                TotalPrice = "$" + x.TotalPrice,
                PurchaseDate = x.PurchaseDate?.ToString("MM/dd/yyyy")
            });

            //Build Excel
            var grid = new GridView();
            grid.DataSource = list;
            grid.DataBind();

            for (int row = 0; row < grid.Rows.Count; row++)
            {
                grid.Rows[row].HorizontalAlign = HorizontalAlign.Center;

                for (int col = 0; col < grid.Rows[row].Cells.Count; col++)
                {
                    grid.Rows[row].Cells[col].Attributes.Add("style", "mso-number-format:\\@");
                    if ((row % 2) == 0)
                    {
                        grid.Rows[row].Cells[col].BackColor = Color.LightGray;
                    }
                }
            }

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);

            Response.ClearContent();
            Response.Buffer = true;
            var FileName = "PurchaseOrders_" + DateTime.Now.ToString("MM/dd/yyyy") + ".xls";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            //

            return View("Export");
        }

        //Create (GET)
        [HttpGet]
        [Authorize(Roles = "InvoicesCreate")]
        public async Task<ActionResult> Create(string previousUrl)
        {
            var vm = await InvoiceLogic.GetBlankInvoice();
             ReferrerController = await GetReferrerControlerNameAsync();

            if (!string.IsNullOrEmpty(previousUrl))
            {
                vm.PreviousUrl = previousUrl;
            }
            else if ((Request.UrlReferrer != null) && (ReferrerController?.ToLower() != "account"))
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            return View(vm);
        }

        //Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "InvoicesCreate")]
        public async Task<ActionResult> Create(InvoiceVM vm)
        {
            vm.PONumber = vm.PONumber?.Replace(" ", String.Empty);
            vm.PONumberOriginal = vm.PONumberOriginal?.Replace(" ", String.Empty);

            if (string.IsNullOrEmpty(vm.PONumber))
            {
                ModelState.AddModelError(nameof(vm.PONumber), "The PO Number is required");
            }
            else if (vm.PONumber.Length < 0 || vm.PONumber.Length > 30)
            {
                ModelState.AddModelError(nameof(vm.PONumber), "Please check the PO Number");
            }
            else if (await InvoiceLogic.CheckPONumber(vm.PONumber))
            {
                ModelState.AddModelError(nameof(vm.PONumber), "The PO Number already exist");
            }
            else if (vm.SupplierId == null)
            {
                ModelState.AddModelError(nameof(vm.SupplierId), "The Supplier is required");
            }
            else if ((vm.SupplierId != null) && (await CommonLogic.IsDeleted(vm.SupplierId, "Supplier")))
            {
                vm.SupplierId = null;
                ModelState.AddModelError(nameof(vm.SupplierId), "The Supplier was recently deleted");
            }
            else if (vm.TotalPrice == null)
            {
                ModelState.AddModelError(nameof(vm.TotalPrice), "The Total Price is required");
            }
            else if (vm.TotalPrice < 0 || vm.TotalPrice > 100000000)
            {
                ModelState.AddModelError(nameof(vm.TotalPrice), "Please check the Total Price");
            }
            else if (vm.PurchaseDate == null)
            {
                ModelState.AddModelError(nameof(vm.PurchaseDate), "The Purchase Date is required");
            }

            if (ModelState.IsValid)
            {
                vm.CreatedBy = Convert.ToInt32(User.Identity.GetUserId());
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                var invoice = await InvoiceLogic.CreateInvoice(vm);
                TempData["Message"] = "You have created an " + AssetType + "!";
                TempData["AssetCreated"] = ViewBag.AssetType;
                return RedirectToAction("Details", new { id = invoice.Id, previousUrl = vm.PreviousUrl });
            }

            vm.Suppliers = await SupplierLogic.GetSuppliers();
            return View(vm);
        }

        //Edit (GET)
        [HttpGet]
        [Authorize(Roles = "InvoicesEdit")]
        public async Task<ActionResult> Edit(int? Id, string previousUrl)
        {
            var vm = await InvoiceLogic.GetInvoice(Id ?? 0);
             ReferrerController = await GetReferrerControlerNameAsync();

            if (vm == null)
            {
                return View("NotFound");
            }

            if (!string.IsNullOrEmpty(previousUrl))
            {
                vm.PreviousUrl = previousUrl;
            }
            else if ((Request.UrlReferrer != null) && (ReferrerController?.ToLower() != "account"))
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            vm.Suppliers = vm.Suppliers?.Append(vm.Supplier)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);

            return View(vm);
        }

        //Edit (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "InvoicesEdit")]
        public async Task<ActionResult> Edit(InvoiceVM vm)
        {
            vm.PONumber = vm.PONumber?.Replace(" ", String.Empty);
            vm.PONumberOriginal = vm.PONumberOriginal?.Replace(" ", String.Empty);

            if (string.IsNullOrEmpty(vm.PONumber))
            {
                ModelState.AddModelError(nameof(vm.PONumber), "The PO Number is required");
            }
            else if (vm.PONumber.Length < 0 || vm.PONumber.Length > 30)
            {
                ModelState.AddModelError(nameof(vm.PONumber), "Please check the PO Number");
            }
            else if (vm.PONumber != vm.PONumberOriginal && await InvoiceLogic.CheckPONumber(vm.PONumber))
            {
                ModelState.AddModelError(nameof(vm.PONumber), "The PO Number already exist");
            }
            else if (vm.SupplierId == null)
            {
                ModelState.AddModelError(nameof(vm.SupplierId), "The Supplier is required");
            }
            else if ((vm.SupplierId != vm.SupplierIdOriginal) && (await CommonLogic.IsDeleted(vm.SupplierId, "Supplier")))
            {
                vm.SupplierId = null;
                ModelState.AddModelError(nameof(vm.SupplierId), "The Supplier was recently deleted");
            }
            else if (vm.TotalPrice == null)
            {
                ModelState.AddModelError(nameof(vm.TotalPrice), "The Total Price is required");
            }
            else if (vm.TotalPrice < 0 || vm.TotalPrice > 100000000)
            {
                ModelState.AddModelError(nameof(vm.TotalPrice), "Please check the Total Price");
            }
            else if (vm.PurchaseDate == null)
            {
                ModelState.AddModelError(nameof(vm.PurchaseDate), "The Purchase Date is required");
            }

            if (ModelState.IsValid)
            {
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                await InvoiceLogic.SaveInvoice(vm);
                TempData["Message"] = "You have edited an " + AssetType + "!";
                
                if (!string.IsNullOrEmpty(vm.PreviousUrl))
                {
                    return Redirect(vm.PreviousUrl);
                }
                else
                {
                    return RedirectToAction("index");
                }
            }

            vm.Suppliers = await SupplierLogic.GetSuppliers();
            vm.Suppliers = vm.Suppliers?.Append(vm.Supplier)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);

            return View(vm);
        }

        //Details
        [Authorize(Roles = "InvoicesView")]
        public async Task<ActionResult> Details(int? Id, string previousUrl)
        {
            var vm = await InvoiceLogic.GetInvoice(Id ?? 0);
             ReferrerController = await GetReferrerControlerNameAsync();

            if (vm == null)
            {
                return View("NotFound");
            }

            if (!string.IsNullOrEmpty(previousUrl))
            {
                vm.PreviousUrl = previousUrl;
            }
            else if ((Request.UrlReferrer != null) && (ReferrerController?.ToLower() != "account"))
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            return View(vm);
        }

        //Delete (GET)
        [HttpGet]
        [Authorize(Roles = "InvoicesDelete")]
        public async Task<ActionResult> Delete(int? Id)
        {
             ReferrerController = await GetReferrerControlerNameAsync();

            ViewBag.Warning = "";

            var vm = await InvoiceLogic.GetInvoice(Id ?? 0);

            if (vm == null)
            {
                return View("NotFound");
            }

            if ((Request.UrlReferrer != null) && (ReferrerController?.ToLower() != "account"))
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            if (vm.InvoiceItemsCount > 0)
            {
                ViewBag.Warning = "You have to delete Invoice Items prior to deleting an Invoice.";
            }

            return View(vm);
        }

        //Delete (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "InvoicesDelete")]
        public async Task<ActionResult> Delete(InvoiceVM vm)
        {
            if (vm.InvoiceItemsCount == 0)
            {
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                await InvoiceLogic.DeleteInvoice(vm);
                TempData["Message"] = "You have deleted an " + AssetType + "!";

                if (!string.IsNullOrEmpty(vm.PreviousUrl))
                {
                    return Redirect(vm.PreviousUrl);
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
            else
                return View(vm);
        }
    }
}