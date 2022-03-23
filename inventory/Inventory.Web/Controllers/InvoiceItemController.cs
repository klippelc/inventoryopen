using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Inventory.Web.Common;
using Inventory.Web.Logic;
using Inventory.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Inventory.Web.Controllers
{
    public class InvoiceItemController : AssetBaseMvcController
    {
        //Constructor
        public InvoiceItemController(IInvoiceItemLogic invoiceItemLogic, IInvoiceLogic invoiceLogic, 
            IAssetLogic assetLogic, ILocationLogic locationLogic, IBuildingLogic buildingLogic, IRoomLogic roomLogic, 
            IUserLogic userLogic, IManuLogic manuLogic, IProductLogic productLogic, ISupplierLogic supplierLogic, ICommonLogic commonLogic)
        {
            InvoiceItemLogic = invoiceItemLogic;
            InvoiceLogic = invoiceLogic;
            AssetLogic = assetLogic;
            LocationLogic = locationLogic;
            BuildingLogic = buildingLogic;
            RoomLogic = roomLogic;
            UserLogic = userLogic;
            ManuLogic = manuLogic;
            ProductLogic = productLogic;
            SupplierLogic = supplierLogic;
            CommonLogic = commonLogic;
            AssetType = "Invoice Item";
            ViewBag.PageName = "PO Item";
            ViewBag.AssetType = AssetType;
        }

        //Create Invoice (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "InvoicesCreate")]
        public async Task<JsonResult> CreateInvoice(InvoiceItemVM vm)
        {
            IDictionary<string, string> errors = new Dictionary<string, string>();

            vm.Invoice.PONumber = vm.Invoice.PONumber?.Replace(" ", String.Empty);
            vm.Invoice.PONumberOriginal = vm.Invoice.PONumberOriginal?.Replace(" ", String.Empty);
            ModelState.Remove("Invoice.Id");

            if (string.IsNullOrEmpty(vm.Invoice.PONumber))
            {
                ModelState.AddModelError(nameof(vm.Invoice.PONumber), "The PO Number is required");
                errors.Add("PONumber", "The PO Number is required");
            }
            else if (vm.Invoice.PONumber.Length < 0 || vm.Invoice.PONumber.Length > 25)
            {
                ModelState.AddModelError(nameof(vm.Invoice.PONumber), "Please check the PO Number");
                errors.Add("PONumber", "The PO Number is required");
            }
            else if (await InvoiceLogic.CheckPONumber(vm.Invoice.PONumber))
            {
                ModelState.AddModelError(nameof(vm.Invoice.PONumber), "The PO Number already exist");
                errors.Add("PONumber", "The PO Number already exist");
            }
            else if (vm.Invoice.SupplierId == null)
            {
                ModelState.AddModelError(nameof(vm.Invoice.SupplierId), "The Supplier is required");
                errors.Add("SupplierId", "The Supplier is required");
            }
            else if (await CommonLogic.IsDeleted(vm.Invoice.SupplierId, "Supplier"))
            {
                vm.Invoice.SupplierId = null;
                ModelState.AddModelError(nameof(vm.Invoice.SupplierId), "The Supplier was recently deleted");
                errors.Add("SupplierId", "The Supplier was recently deleted");
            }
            else if (vm.Invoice.TotalPrice == null)
            {
                ModelState.AddModelError(nameof(vm.Invoice.TotalPrice), "The Total Price is required");
                errors.Add("TotalPrice", "The Total Price is required");
            }
            else if (vm.Invoice.TotalPrice < 0 || vm.Invoice.TotalPrice > 1000000)
            {
                ModelState.AddModelError(nameof(vm.Invoice.TotalPrice), "Please check the Total Price");
                errors.Add("TotalPrice", "Please check the Total Price");
            }
            else if (vm.Invoice.PurchaseDate == null)
            {
                ModelState.AddModelError(nameof(vm.Invoice.PurchaseDate), "The Purchase Date is required");
                errors.Add("PurchaseDate", "The Purchase Date is required");
            }

            if (ModelState.IsValid)
            {
                vm.Invoice.CreatedBy = Convert.ToInt32(User.Identity.GetUserId());
                var invoice = await InvoiceLogic.CreateInvoice(vm.Invoice);
                return Json(new { IsCreated = true, Content = invoice }, JsonRequestBehavior.AllowGet);
            }

            vm.Invoice.Suppliers = await SupplierLogic.GetSuppliers();

            var messageArray = this.ViewData.ModelState.Values.SelectMany(modelState => modelState.Errors, (modelState, error) => error.ErrorMessage).ToArray();
            return Json(new
            {
                IsCreated = false,
                errors, 
                Content = vm.Invoice
            }, JsonRequestBehavior.AllowGet);
        }

        //Update Invoice (GET)
        [HttpGet]
        [Authorize(Roles = "InvoicesEdit")]
        public async Task<JsonResult> EditInvoice(int Id)
        {
            var vm = await InvoiceLogic.GetInvoice(Id);
            vm.Suppliers = vm.Suppliers?.Append(vm.Supplier)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);

            var vmInvoice = JsonConvert.SerializeObject(vm);
            return Json(vmInvoice, JsonRequestBehavior.AllowGet);
        }

        //Update Invoice (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "InvoicesEdit")]
        public async Task<JsonResult> EditInvoice(InvoiceItemVM vm)
        {
            IDictionary<string, string> errors = new Dictionary<string, string>();

            vm.Invoice.PONumber = vm.Invoice.PONumber?.Replace(" ", String.Empty);
            vm.Invoice.PONumberOriginal = vm.Invoice.PONumberOriginal?.Replace(" ", String.Empty);

            if (string.IsNullOrEmpty(vm.Invoice.PONumber))
            {
                ModelState.AddModelError(nameof(vm.Invoice.PONumber), "The PO Number is required");
                errors.Add("PONumber", "The PO Number is required");
            }
            else if (vm.Invoice.PONumber.Length < 0 || vm.Invoice.PONumber.Length > 25)
            {
                ModelState.AddModelError(nameof(vm.Invoice.PONumber), "Please check the PO Number");
                errors.Add("PONumber", "The PO Number is required");
            }
            else if (vm.Invoice.PONumber != vm.Invoice.PONumberOriginal && await InvoiceLogic.CheckPONumber(vm.Invoice.PONumber))
            {
                ModelState.AddModelError(nameof(vm.Invoice.PONumber), "The PO Number already exist");
                errors.Add("PONumber", "The PO Number already exist");
            }
            else if (vm.Invoice.SupplierId == null)
            {
                ModelState.AddModelError(nameof(vm.Invoice.SupplierId), "The Supplier is required");
                errors.Add("SupplierId", "The Supplier is required");
            }
            else if ((vm.Invoice.SupplierId != vm.Invoice.SupplierIdOriginal) && (await CommonLogic.IsDeleted(vm.Invoice.SupplierId, "Supplier")))
            {
                vm.Invoice.SupplierId = null;
                ModelState.AddModelError(nameof(vm.Invoice.SupplierId), "The Supplier was recently deleted");
                errors.Add("SupplierId", "The Supplier was recently deleted");
            }
            else if (vm.Invoice.TotalPrice == null)
            {
                ModelState.AddModelError(nameof(vm.Invoice.TotalPrice), "The Total Price is required");
                errors.Add("TotalPrice", "The Total Price is required");
            }
            else if (vm.Invoice.TotalPrice < 0 || vm.Invoice.TotalPrice > 1000000)
            {
                ModelState.AddModelError(nameof(vm.Invoice.TotalPrice), "Please check the Total Price");
                errors.Add("TotalPrice", "Please check the Total Price");
            }
            else if (vm.Invoice.PurchaseDate == null)
            {
                ModelState.AddModelError(nameof(vm.Invoice.PurchaseDate), "The Purchase Date is required");
                errors.Add("PurchaseDate", "The Purchase Date is required");
            }

            if (ModelState.IsValid)
            {
                vm.Invoice.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                var invoice = await InvoiceLogic.SaveInvoice(vm.Invoice);
                return Json(new { IsUpdated = true, Content = invoice }, JsonRequestBehavior.AllowGet);
            }

            vm.Invoice.Suppliers = await SupplierLogic.GetSuppliers();

            if (vm.Invoice.SupplierIdOriginal != null)
            {
                var supplierVm = new SupplierVM()
                {
                    Id = vm.Invoice?.SupplierIdOriginal ?? 0,
                    DisplayName = vm.Invoice?.SupplierDisplayNameOriginal ?? ""
                };
                vm.Invoice.Suppliers = vm.Invoice.Suppliers?.Append(supplierVm)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);
            }


            return Json(new
            {
                IsUpdated = false,
                errors,
                Content = vm.Invoice
            }, JsonRequestBehavior.AllowGet);
        }

        //Get Invoice Items
        [Authorize(Roles = "InvoiceItemsView")]
        public async Task<ActionResult> Index(string assetType, string assetCategory, string sortOrder, string currentFilter, string searchString, int? invoiceId, int? invoiceItemId, int page = 1)
        {
            ModelState.Remove("SearchString");
            searchString = searchString?.Trim();

            ViewBag.AssetTypeDesc = assetType;
            ViewBag.AssetCategory = assetCategory;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.InvoiceId = invoiceId;
            ViewBag.InvoiceItemId = invoiceItemId;
            ViewBag.AssetTypeSortOrder = sortOrder == "assettype" ? "assettype_desc" : "assettype";
            ViewBag.AssetCatSortOrder = sortOrder == "assetcat" ? "assetcat_desc" : "assetcat";
            ViewBag.PONumberSortOrder = sortOrder == "ponumber" ? "ponumber_desc" : "ponumber";
            ViewBag.ManuSortOrder = sortOrder == "manufacturer" ? "manufacturer_desc" : "manufacturer";
            ViewBag.ProductSortOrder = sortOrder == "product" ? "product_desc" : "product";
            ViewBag.InvoiceSortOrder = sortOrder == "invoice" ? "invoice_desc" : "invoice";
            ViewBag.LicenseTypeSortOrder = sortOrder == "licensetype" ? "licensetype_desc" : "licensetype";
            ViewBag.QuantitySortOrder = sortOrder == "quantity" ? "quantity_desc" : "quantity";
            ViewBag.AssignedSortOrder = sortOrder == "assigned" ? "assigned_desc" : "assigned";
            ViewBag.SharedSortOrder = sortOrder == "shared" ? "shared_desc" : "shared";
            ViewBag.ActiveSortOrder = sortOrder == "active" ? "active_desc" : "active";
            ViewBag.InActiveSortOrder = sortOrder == "inactive" ? "inactive_desc" : "inactive";
            ViewBag.AvailableSortOrder = sortOrder == "available" ? "available_desc" : "available";
            ViewBag.UnitPriceSortOrder = sortOrder == "unitprice" ? "unitprice_desc" : "unitprice";

            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var vmList = await InvoiceItemLogic.GetInvoiceItems(invoiceId, invoiceItemId, assetType, assetCategory, sortOrder, searchString, page, pageSize);

            return View(vmList);
        }

        [Authorize(Roles = "InvoiceItemsView")]
        public async Task<ActionResult> ExportToExcel(string assetType, string assetCategory, string sortOrder, string currentFilter, string searchString, int? invoiceId, int? invoiceItemId, int page = 1)
        {
            var search = !string.IsNullOrEmpty(searchString) ? searchString : currentFilter;
            var vmInvoiceItemsList = await InvoiceItemLogic.GetInvoiceItems(invoiceId, invoiceItemId, assetType, assetCategory, sortOrder, search, 1, 999999);

            var itemList = vmInvoiceItemsList.Select(x => x.InvoiceItems).FirstOrDefault();
            var list = itemList.Select(x => new {
                x.Id,
                AssetType = x.AssetType.Name,
                Category = x.AssetCategory.Name,
                Manufacturer = x.Manufacturer.DisplayName,
                Product = x.Product.DisplayName,
                x.Quantity,
                x.Assigned,
                x.InActive,
                x.Available,
                x.UnitPrice,
                x.Notes
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
            var FileName = "POItems_" + DateTime.Now.ToString("MM/dd/yyyy") + ".xls";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            //

            return View("Export");
        }

        //Create 
        [HttpGet]
        [Authorize(Roles = "InvoiceItemsCreate")]
        public async Task<ActionResult> Create(string previousUrl)
        {
            var vm = await InvoiceItemLogic.GetBlankInvoiceItem();
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
        [Authorize(Roles = "InvoiceItemsCreate")]
        public async Task<ActionResult> Create(InvoiceItemVM vm)
        {
            vm.AssetType = await AssetLogic.GetAssetType(vm.AssetTypeId ?? 0);
            vm.LicenseType = await AssetLogic.GetLicenseType(vm.LicenseTypeId ?? 0);
            var ManuProductCheck = false;

            if (vm.ManuId != null && vm.ProductId != null)
            {
                ManuProductCheck = await InvoiceItemLogic.CheckManuandProduct(vm.InvoiceId ?? 0, vm.ManuId ?? 0, vm.ProductId ?? 0);
            }

            List<string> serialList = vm.Serial.CreateSerialList();
            ModelState.Remove("Serial");
            vm.Serial = serialList.ListToString();

            var badSerial = serialList.FindInvalidListItem(RegexSerial);
            var smallestSerial = serialList.SmallestItemInList();
            var largestSerial = serialList.LargestItemInList();
            var duplicateSerial = serialList.DuplicateItemInList();
            
            ///

            List<string> licenseList = vm.LicenseKeyMulti.CreateLicenseList();
            ModelState.Remove("LicenseKeyMulti");
            vm.LicenseKeyMulti = licenseList.ListToString();

            var badLicense = licenseList.FindInvalidListItem(RegexLicense);
            var smallestLicense = licenseList.SmallestItemInList();
            var largestLicense = licenseList.LargestItemInList();
            var duplicateLicense = licenseList.DuplicateItemInList();

            ///

            ModelState.Remove("LicenseKeySingle");
            vm.LicenseKeySingle = vm.LicenseKeySingle.RemoveSpaces();

            ///

            string existSerial = null;
            string existLicense = null;

            if ((vm.AssetType.Name == "Software") && (!string.IsNullOrEmpty(vm.LicenseKeyMulti)))
            {
                existLicense = await AssetLogic.CheckLicenses(licenseList, vm.LicenseType.Name);
            }
            else if ((vm.AssetType.Name == "Software") && (!string.IsNullOrEmpty(vm.LicenseKeySingle)))
            {
                existLicense = await AssetLogic.CheckLicenses(vm.LicenseKeySingle.ToStringList(), vm.LicenseType.Name);
            }
            else if (vm.AssetType.Name == "Hardware" && vm.ManuId != null && !string.IsNullOrEmpty(vm.Serial))
            {
                existSerial = await AssetLogic.CheckSerials(serialList, vm.ManuId ?? 0);
            }

            ////

            if (vm.InvoiceId == null)
            {
                ModelState.AddModelError(nameof(vm.InvoiceId), "The Invoice is required");
            }
            else if (vm.AssetTypeId == null)
            {
                ModelState.AddModelError(nameof(vm.AssetTypeId), "The Asset Type is required");
            }
            //Hardware
            else if (vm.AssetType.Name == "Hardware")
            {
                if (await CommonLogic.IsDeleted(vm.ProductId, "Product"))
                {
                    vm.ProductId = null;
                    ModelState.AddModelError(nameof(vm.ProductId), "The Model was recently deleted");
                }
                else if (ManuProductCheck)
                {
                    ModelState.AddModelError(nameof(vm.ProductId), "The Model already exist for this Purchase Order");
                }
                else if (vm.Quantity == null)
                {
                    ModelState.AddModelError(nameof(vm.Quantity), "The Quantity is required");
                }
                else if (vm.Quantity > 1000)
                {
                    ModelState.AddModelError(nameof(vm.Quantity), "The Quantity is too large");
                }
                else if (!string.IsNullOrEmpty(vm.Serial) && (serialList.Count > vm.Quantity))
                {
                    ModelState.AddModelError(nameof(vm.Serial), "More Serials than Quantity");
                }
                else if (!string.IsNullOrEmpty(vm.Serial) && smallestSerial?.Length < 5)
                {
                    ModelState.AddModelError(nameof(vm.Serial), "Serial Number (" + smallestSerial + ") is too small");
                }
                else if (!string.IsNullOrEmpty(vm.Serial) && largestSerial?.Length > 25)
                {
                    ModelState.AddModelError(nameof(vm.Serial), "Serial Number (" + largestSerial.Substring(0, 5) + "...) is too large");
                }
                else if (!string.IsNullOrEmpty(vm.Serial) && !string.IsNullOrEmpty(badSerial))
                {
                    ModelState.AddModelError(nameof(vm.Serial), "Serial Number (" + badSerial + ") is not in the correct format");
                }
                else if (!string.IsNullOrEmpty(vm.Serial) && !string.IsNullOrEmpty(duplicateSerial))
                {
                    ModelState.AddModelError(nameof(vm.Serial), "Serial Number (" + duplicateSerial + ") is duplicated");
                }
                else if (!string.IsNullOrEmpty(vm.Serial) && !string.IsNullOrEmpty(existSerial))
                {
                    ModelState.AddModelError(nameof(vm.Serial), "Serial Number (" + existSerial + ") already exist in db");
                }
                else if (vm.AssetLocationId == null)
                {
                    ModelState.AddModelError(nameof(vm.AssetLocationId), "The Location is required");
                }
                else if ((vm.AssetLocationId != null) && (await CommonLogic.IsDeleted(vm.AssetLocationId, "Location")))
                {
                    vm.AssetLocationId = null;
                    ModelState.AddModelError(nameof(vm.AssetLocationId), "The Location was recently deleted");
                }
                else if ((vm.AssetBuildingId != null) && (await CommonLogic.IsDeleted(vm.AssetBuildingId, "Building")))
                {
                    vm.AssetBuildingId = null;
                    ModelState.AddModelError(nameof(vm.AssetBuildingId), "The Building was recently deleted");
                }
                else if ((vm.AssetRoomId != null) && (await CommonLogic.IsDeleted(vm.AssetRoomId, "Room")))
                {
                    vm.AssetRoomId = null;
                    ModelState.AddModelError(nameof(vm.AssetRoomId), "The Room was recently deleted");
                }
            }
            //Software
            else if (vm.AssetType.Name == "Software")
            {
                if (await CommonLogic.IsDeleted(vm.ProductId, "Product"))
                {
                    vm.ProductId = null;
                    ModelState.AddModelError(nameof(vm.ProductId), "The Product was recently deleted");
                }
                if (ManuProductCheck)
                {
                    ModelState.AddModelError(nameof(vm.ProductId), "The Product already exist for this Purchase Order");
                }
                else if (vm.LicenseTypeId == null)
                {
                    ModelState.AddModelError(nameof(vm.LicenseTypeId), "The License Type is required");
                }
                else if (vm.Quantity == null)
                {
                    ModelState.AddModelError(nameof(vm.Quantity), "The Quantity is required");
                }
                else if (vm.Quantity > 100000)
                {
                    ModelState.AddModelError(nameof(vm.Quantity), "The Quantity is too large");
                }
                if (vm.LicenseType.Name == "Hardware-Single")
                {
                    if (!string.IsNullOrEmpty(vm.LicenseKeySingle) && vm.LicenseKeySingle.Length < 5)
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeySingle), "The License Key is too small");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeySingle) && vm.LicenseKeySingle.Length > 25)
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeySingle), "The License Key is too large");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeySingle) && (vm.LicenseKeySingle.InValidItem(RegexLicense)))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeySingle), "License Key is not in the correct format");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeySingle) && !string.IsNullOrEmpty(existLicense))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeySingle), "License Key already exist in db");
                    }
                }
                else if (vm.LicenseType.Name == "Hardware-Multi") {
                    if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && (licenseList.Count > vm.Quantity))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "More Licenses than Quantity");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && smallestLicense?.Length < 5)
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "License Key (" + smallestLicense + ") is too small");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && (largestLicense?.Length > 25))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "License Key (" + largestLicense.Substring(0, 5) + "...) is too large");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && !string.IsNullOrEmpty(badLicense))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "License Key (" + badLicense + ") is not in the correct format");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && !string.IsNullOrEmpty(duplicateLicense))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "License Key (" + duplicateLicense + ") is duplicated");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && !string.IsNullOrEmpty(existLicense))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "License Key (" + existLicense + ") already exist in db");
                    }
                }
                if ((vm.LicenseType.Name != "User") && (vm.AssignedAssetId != null) && (await CommonLogic.IsDeleted(vm.AssignedAssetId, "Asset")))
                {
                    ModelState.AddModelError(nameof(vm.AssignedAssetId), "Asset was recently deleted");
                }
            }
            else if (vm.UnitPrice != null && vm.UnitPrice > 99999)
            {
                ModelState.AddModelError(nameof(vm.UnitPrice), "The Unit Price is too large");
            }


            if (ModelState.IsValid)
            {
                vm.CreatedBy = Convert.ToInt32(User.Identity.GetUserId());
                var item = await InvoiceItemLogic.CreateInvoiceItem(vm);
                TempData["Message"] = "You have created a " + AssetType + "!";
                TempData["AssetCreated"] = ViewBag.AssetType;
                return RedirectToAction("Details", new { id = item.Id, previousUrl = vm.PreviousUrl });
            }
            else
            {
                vm.Invoices = await InvoiceLogic.GetInvoices();
                vm.AssetTypes = await CommonLogic.GetAssetTypes();
                vm.LicenseTypes = await AssetLogic.GetLicenseTypes();
                vm.Users = await UserLogic.GetUsers();
                var Assets = await AssetLogic.GetComputerTypeAssets(null);
                vm.Assets = Assets.OrderBy(x => x.Serial);
                vm.Suppliers = await SupplierLogic.GetSuppliers();

                vm.AssetCategories = await AssetLogic.GetAssetCategoriesByType(vm.AssetTypeId);
                vm.Manufacturers = await ManuLogic.GetManufacturers(vm.AssetTypeId, vm.AssetCategoryId);
                vm.Locations = await LocationLogic.GetLocations();
                vm.Products = await ProductLogic.GetProducts(vm.ManuId, vm.AssetTypeId, vm.AssetCategoryId);
                vm.Buildings = await BuildingLogic.GetBuildings(vm.AssetLocationId);
                vm.Rooms = await RoomLogic.GetRooms(vm.AssetBuildingId);

                return View(vm);
            }
        }

        //Update (GET)
        [HttpGet]
        [Authorize(Roles = "InvoiceItemsEdit")]
        public async Task<ActionResult> Edit(int? Id, string previousUrl)
        {
            var vm = await InvoiceItemLogic.GetInvoiceItem(Id ?? 0);
             ReferrerController = await GetReferrerControlerNameAsync();

            if (!string.IsNullOrEmpty(previousUrl))
            {
                vm.PreviousUrl = previousUrl;
            }
            else if ((Request.UrlReferrer != null) && (ReferrerController?.ToLower() != "account"))
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            if (vm == null)
            {
                return View("NotFound");
            }

            vm.Manufacturers = vm.Manufacturers?.Append(vm.Manufacturer)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);
            vm.Products = vm.Products?.Append(vm.Product)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);

            return View(vm);
        }

        //Update (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "InvoiceItemsEdit")]
        public async Task<ActionResult> Edit(InvoiceItemVM vm)
        {
            vm.AssetType = await AssetLogic.GetAssetType(vm.AssetTypeId ?? 0);
            vm.LicenseType = await AssetLogic.GetLicenseType(vm.LicenseTypeId ?? 0);
            vm.InvoiceItemNumber = vm.InvoiceItemNumber;
            var ManuProductCheck = false;

            if (vm.ManuId != null && vm.ProductId != null && (vm.ManuId != vm.OriginalManuId || vm.ProductId != vm.OriginalProductId || vm.InvoiceId != vm.OriginalInvoiceId))
            {
                ManuProductCheck = await InvoiceItemLogic.CheckManuandProduct(vm.InvoiceId ?? 0, vm.ManuId ?? 0, vm.ProductId ?? 0);
            }

            var licenseKeySingle = vm.LicenseKeySingle.RemoveSpaces();
            ModelState.Remove("LicenseKeySingle");
            vm.LicenseKeySingle = licenseKeySingle;

            List<string> serialList = vm.Serial.CreateSerialList();
            ModelState.Remove("Serial");
            vm.Serial = serialList.ListToString();

            var badSerial = serialList.FindInvalidListItem(RegexSerial);
            var smallestSerial = serialList.SmallestItemInList();
            var largestSerial = serialList.LargestItemInList();
            var duplicateSerial = serialList.DuplicateItemInList();

            ///

            List<string> licenseList = vm.LicenseKeyMulti.CreateLicenseList();
            ModelState.Remove("LicenseKeyMulti");
            vm.LicenseKeyMulti = licenseList.ListToString();

            var badLicense = licenseList.FindInvalidListItem(RegexLicense);
            var smallestLicense = licenseList.SmallestItemInList();
            var largestLicense = licenseList.LargestItemInList();
            var duplicateLicense = licenseList.DuplicateItemInList();

            ///

            ModelState.Remove("LicenseKeySingle");
            vm.LicenseKeySingle = vm.LicenseKeySingle.RemoveSpaces();

            ///

            string existSerial = null;
            string existLicense = null;

            if ((vm.AssetType.Name == "Software") && (!string.IsNullOrEmpty(vm.LicenseKeyMulti)))
            {
                existLicense = await AssetLogic.CheckLicenses(licenseList, vm.LicenseType?.Name);
            }
            else if ((vm.AssetType.Name == "Software") && (!string.IsNullOrEmpty(vm.LicenseKeySingle)) && (licenseKeySingle != vm.OriginalLicenseKeySingle))
            {
                existLicense = await AssetLogic.CheckLicenses(vm.LicenseKeySingle.ToStringList(), vm.LicenseType?.Name);
            }
            else if (vm.AssetType.Name == "Hardware" && vm.ManuId != null && vm.ManuId != vm.OriginalManuId && !string.IsNullOrEmpty(vm.Serial))
            {
                existSerial = await AssetLogic.CheckSerials(serialList, vm.OriginalManuId ?? 0);
            }
            else if (vm.AssetType.Name == "Hardware" && vm.ManuId != null && !string.IsNullOrEmpty(vm.Serial))
            {
                existSerial = await AssetLogic.CheckSerials(serialList, vm.ManuId ?? 0);
            }

            ////////////

            if (vm.InvoiceId == null)
            {
                ModelState.AddModelError(nameof(vm.InvoiceId), "The Invoice is required");
            }
            //Hardware
            else if (vm.AssetTypeName == "Hardware")
            {
                if ((vm.ProductId != vm.OriginalProductId) && (await CommonLogic.IsDeleted(vm.ProductId, "Product")))
                {
                    vm.ProductId = null;
                    ModelState.AddModelError(nameof(vm.ProductId), "The Model was recently deleted");
                }
                else if (ManuProductCheck)
                {
                    ModelState.AddModelError(nameof(vm.ProductId), "The Model already exist for this Purchase Order");
                }
                else if (vm.Quantity == null)
                {
                    ModelState.AddModelError(nameof(vm.Quantity), "The Quantity is required");
                }
                else if (vm.Quantity > 1000)
                {
                    ModelState.AddModelError(nameof(vm.Quantity), "The Quantity is too large");
                }
                else if (vm.QuantityOriginal != null && vm.Quantity < vm.QuantityOriginal)
                {
                    ModelState.AddModelError(nameof(vm.Quantity), "The Quantity can't be less than " + vm.QuantityOriginal.ToString());
                }
                else if (!string.IsNullOrEmpty(vm.Serial) && (serialList.Count > (vm.Quantity - vm.QuantityOriginal)))
                {
                    ModelState.AddModelError(nameof(vm.Serial), "More Serials than Quantity (Qty Increased by " + (vm.Quantity - vm.QuantityOriginal) + ")");
                }
                else if (!string.IsNullOrEmpty(vm.Serial) && smallestSerial?.Length < 5)
                {
                    ModelState.AddModelError(nameof(vm.Serial), "Serial Number (" + smallestSerial + ") is too small");
                }
                else if (!string.IsNullOrEmpty(vm.Serial) && largestSerial?.Length > 25)
                {
                    ModelState.AddModelError(nameof(vm.Serial), "Serial Number (" + largestSerial.Substring(0, 5) + "...) is too large");
                }
                else if (!string.IsNullOrEmpty(vm.Serial) && !string.IsNullOrEmpty(badSerial))
                {
                    ModelState.AddModelError(nameof(vm.Serial), "Serial Number (" + badSerial + ") is not in the correct format");
                }
                else if (!string.IsNullOrEmpty(vm.Serial) && !string.IsNullOrEmpty(duplicateSerial))
                {
                    ModelState.AddModelError(nameof(vm.Serial), "Serial Number (" + duplicateSerial + ") is duplicated");
                }
                else if (!string.IsNullOrEmpty(vm.Serial) && !string.IsNullOrEmpty(existSerial))
                {
                    ModelState.AddModelError(nameof(vm.Serial), "Serial Number (" + existSerial + ") already exist in db");
                }
                else if ((vm.AssetLocationId == null) && ((vm.QuantityOriginal != null) && (vm.Quantity > vm.QuantityOriginal)))
                {
                    ModelState.AddModelError(nameof(vm.AssetLocationId), "The Location is required");
                }
                else if ((vm.AssetLocationId != null) && (await CommonLogic.IsDeleted(vm.AssetLocationId, "Location")))
                {
                    vm.AssetLocationId = null;
                    ModelState.AddModelError(nameof(vm.AssetLocationId), "The Location was recently deleted");
                }
                else if ((vm.AssetBuildingId != null) && (await CommonLogic.IsDeleted(vm.AssetBuildingId, "Building")))
                {
                    vm.AssetBuildingId = null;
                    ModelState.AddModelError(nameof(vm.AssetBuildingId), "The Building was recently deleted");
                }
                else if ((vm.AssetRoomId != null) && (await CommonLogic.IsDeleted(vm.AssetRoomId, "Room")))
                {
                    vm.AssetRoomId = null;
                    ModelState.AddModelError(nameof(vm.AssetRoomId), "The Room was recently deleted");
                }
            }
            //Software
            else if (vm.AssetTypeName == "Software")
            {
                if ((vm.ProductId != vm.OriginalProductId) && (await CommonLogic.IsDeleted(vm.ProductId, "Product")))
                {
                    vm.ProductId = null;
                    ModelState.AddModelError(nameof(vm.ProductId), "The Product was recently deleted");
                }
                else if (ManuProductCheck)
                {
                    ModelState.AddModelError(nameof(vm.ProductId), "The Product already exist for this Purchase Order");
                }
                else if (vm.Quantity == null)
                {
                    ModelState.AddModelError(nameof(vm.Quantity), "The Quantity is required");
                }
                else if (vm.Quantity > 1000)
                {
                    ModelState.AddModelError(nameof(vm.Quantity), "The Quantity is too large");
                }
                else if (vm.QuantityOriginal != null && vm.Quantity < vm.QuantityOriginal)
                {
                    ModelState.AddModelError(nameof(vm.Quantity), "The Quantity can't be less than " + vm.QuantityOriginal.ToString());
                }
                else if (vm.LicenseType.Name == "Hardware-Single") {
                    if (!string.IsNullOrEmpty(vm.LicenseKeySingle) && vm.LicenseKeySingle.Length < 5)
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeySingle), "The License Key is too small");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeySingle) && vm.LicenseKeySingle.Length > 25)
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeySingle), "The License Key is too large");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeySingle) && vm.LicenseKeySingle.InValidItem(RegexLicense))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeySingle), "License Key is not in the correct format");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeySingle) && (!string.IsNullOrEmpty(existLicense)))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeySingle), "License Key already exist in db");
                    }
                }
                else if (vm.LicenseType.Name == "Hardware-Multi")
                {
                    if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && (licenseList.Count > (vm.Quantity - vm.QuantityOriginal)))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "More Licenses than Quantity (Qty Increased by " + (vm.Quantity - vm.QuantityOriginal) + ")");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && smallestLicense?.Length < 5)
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "License Key (" + smallestLicense + ") is too small");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && (largestLicense?.Length > 25))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "License Key (" + largestLicense.Substring(0, 5) + "...) is too large");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && !string.IsNullOrEmpty(badLicense))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "License Key (" + badLicense + ") is not in the correct format");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && !string.IsNullOrEmpty(duplicateLicense))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "License Key (" + duplicateLicense + ") is duplicated");
                    }
                    else if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && !string.IsNullOrEmpty(existLicense))
                    {
                        ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "License Key (" + existLicense + ") already exist in db");
                    }
                }
                if ((vm.LicenseType.Name != "User") && (vm.AssignedAssetId != null) && (await CommonLogic.IsDeleted(vm.AssignedAssetId, "Asset")))
                {
                    ModelState.AddModelError(nameof(vm.AssignedAssetId), "Asset was recently deleted");
                }
            }
            else if (vm.UnitPrice != null && vm.UnitPrice > 99999)
            {
                ModelState.AddModelError(nameof(vm.UnitPrice), "The Unit Price is too large");
            }

            if (ModelState.IsValid)
            {
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                await InvoiceItemLogic.SaveInvoiceItem(vm);
                TempData["Message"] = "You have updated a " + AssetType + "!";

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
            {
                vm.Invoices = await InvoiceLogic.GetInvoices();
                vm.Suppliers = await SupplierLogic.GetSuppliers();

                vm.LicenseTypes = await AssetLogic.GetLicenseTypes();
                vm.AssetCategories = await AssetLogic.GetAssetCategoriesByType(vm.AssetTypeId);
                vm.Manufacturers = await ManuLogic.GetManufacturers(vm.AssetTypeId, vm.AssetCategoryId);
                vm.Products = await ProductLogic.GetProducts(vm.ManuId, vm.AssetTypeId, vm.AssetCategoryId);

                vm.Manufacturers = vm.Manufacturers?.Append(vm.Manufacturer)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);
                vm.Products = vm.Products?.Append(vm.Product)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);


                //

                vm.AssetTypes = await CommonLogic.GetAssetTypes();
                vm.Users = await UserLogic.GetUsers();
                var Assets = await AssetLogic.GetComputerTypeAssets(null);
                vm.Assets = Assets.OrderBy(x => x.Serial);

                vm.Locations = await LocationLogic.GetLocations();
                vm.Products = await ProductLogic.GetProducts(vm.ManuId, vm.AssetTypeId, vm.AssetCategoryId);
                vm.Buildings = await BuildingLogic.GetBuildings(vm.AssetLocationId);
                vm.Rooms = await RoomLogic.GetRooms(vm.AssetBuildingId);

                //

                return View(vm);
            }
        }

        //Details
        [Authorize(Roles = "InvoiceItemsView")]
        public async Task<ActionResult> Details(int? Id, string previousUrl)
        {
            var vm = await InvoiceItemLogic.GetInvoiceItem(Id ?? 0);
             ReferrerController = await GetReferrerControlerNameAsync();

            if (!string.IsNullOrEmpty(previousUrl))
            {
                vm.PreviousUrl = previousUrl;
            }
            else if ((Request.UrlReferrer != null) && (ReferrerController?.ToLower() != "account"))
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            if (vm == null)
            {
                return View("NotFound");
            }

            return View(vm);
        }

        //Delete (GET)
        [HttpGet]
        [Authorize(Roles = "InvoiceItemsDelete")]
        public async Task<ActionResult> Delete(int? Id)
        {
            var vm = await InvoiceItemLogic.GetInvoiceItem(Id ?? 0);
             ReferrerController = await GetReferrerControlerNameAsync();

            if ((Request.UrlReferrer != null) && (ReferrerController?.ToLower() != "account"))
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            if (vm == null)
            {
                return View("NotFound");
            }

            if (vm.Quantity > 0)
            {
                ViewBag.Warning = "You have to delete the assets prior to deleting an Invoice Item.";
            }

            return View(vm);
        }

        //Delete (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "InvoiceItemsDelete")]
        public async Task<ActionResult> Delete(InvoiceItemVM vm)
        {
            if (vm.Quantity == 0)
            {
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                await InvoiceItemLogic.DeleteInvoiceItem(vm);
                TempData["Message"] = "You have deleted a " + AssetType + "!";

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