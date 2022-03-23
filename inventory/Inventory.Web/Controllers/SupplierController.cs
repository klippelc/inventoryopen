using Microsoft.AspNet.Identity;
using Inventory.Web.Common;
using Inventory.Web.Logic;
using Inventory.Web.ViewModels;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Inventory.Web.Controllers
{
    [Authorize(Roles = "SuppliersManage")]
    public class SupplierController : AssetBaseMvcController
    {
        //Constructor
        public SupplierController(ISupplierLogic supplierLogic, ICommonLogic commonLogic)
        {
            SupplierLogic = supplierLogic;
            CommonLogic = commonLogic;
            AssetType = "Supplier";
            ViewBag.AssetType = AssetType;
            ViewBag.SettingsType = AssetType;
        }

        //Get All
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ModelState.Remove("SearchString");
            searchString = searchString?.Trim();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortOrder = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.DisplayNameSortOrder = sortOrder == "displayname" ? "displayname_desc" : "displayname";
            ViewBag.PhoneSortOrder = sortOrder == "phone" ? "phone_desc" : "phone";
            ViewBag.EmailSortOrder = sortOrder == "email" ? "email_desc" : "email";
            ViewBag.UrlSortOrder = sortOrder == "url" ? "url_desc" : "url";

            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var vmAssetList = await SupplierLogic.GetList(sortOrder, currentFilter, searchString, AssetType, page, pageSize);

            return View(vmAssetList);
        }

        //Create 
        [HttpGet]
        public async Task<ActionResult> Create(string previousUrl)
        {
            ReferrerController = await GetReferrerControlerNameAsync();

            var vm = new SupplierVM
            {
                Active = true
            };

            if (!string.IsNullOrEmpty(previousUrl))
            {
                vm.PreviousUrl = previousUrl;
            }
            else if ((Request.UrlReferrer != null) && (ReferrerController?.ToLower() != "account"))
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            vm.States = await CommonLogic.GetStates(null);
            return View(vm);
        }

        //Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SupplierVM vm)
        {
            if (string.IsNullOrEmpty(vm.Name))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name is required");
            }
            else if (!string.IsNullOrEmpty(vm.Name) && (await CommonLogic.NameExist(vm.Name, AssetType)))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name already exist in db");
            }
            else if (!string.IsNullOrEmpty(vm.DisplayName) && (await CommonLogic.NameExist(vm.DisplayName, AssetType)))
            {
                ModelState.AddModelError(nameof(vm.DisplayName), "The Name already exist in db");
            }

            if (ModelState.IsValid)
            {
                vm.CreatedBy = Convert.ToInt32(User.Identity.GetUserId());
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                var itemId = await SupplierLogic.Create(vm);
                TempData["Message"] = "You have created an " + AssetType + "!";
                TempData["AssetCreated"] = ViewBag.AssetType;
                return RedirectToAction("Details", new { id = itemId, previousUrl = vm.PreviousUrl });
            }
            vm.States = await CommonLogic.GetStates(null);

            return View(vm);
        }

        //Details
        public async Task<ActionResult> Details(int? Id, string previousUrl)
        {
            var vm = await SupplierLogic.Get(Id ?? 0);
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


        //Update (GET)
        [HttpGet]
        public async Task<ActionResult> Edit(int? Id, string previousUrl)
        {
            var vm = await SupplierLogic.Get(Id ?? 0);
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

            vm.States = await CommonLogic.GetStates(null);

            return View(vm);
        }

        //Update (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SupplierVM vm)
        {
            if (string.IsNullOrEmpty(vm.Name))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name is required");
            }
            else if ((vm.Name?.ToLower().Trim() != vm.OriginalName?.ToLower().Trim()) && (vm.Name?.ToLower().Trim() != vm.OriginalDisplayName?.ToLower().Trim()) && (await CommonLogic.NameExist(vm.Name, AssetType)))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name already exist in db");
            }
            else if ((vm.DisplayName?.ToLower().Trim() != vm.OriginalDisplayName?.ToLower().Trim()) && (vm.DisplayName?.ToLower().Trim() != vm.OriginalName?.ToLower().Trim()) && (await CommonLogic.NameExist(vm.DisplayName, AssetType)))
            {
                ModelState.AddModelError(nameof(vm.DisplayName), "The Display Name already exist in db");
            }

            if (ModelState.IsValid)
            {
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                await SupplierLogic.Save(vm);
                TempData["Message"] = "You have updated a " + AssetType + " component!";
                TempData["AssetCreated"] = null;

                if (!string.IsNullOrEmpty(vm.PreviousUrl))
                {
                    return Redirect(vm.PreviousUrl);
                }
                else
                {
                    return RedirectToAction("index");
                }
            }

            vm.States = await CommonLogic.GetStates(null);
            return View(vm);
        }

        // Delete (GET)
        [HttpGet]
        public async Task<ActionResult> Delete(int? Id)
        {
             ReferrerController = await GetReferrerControlerNameAsync();

            ViewBag.Warning = "";

            var vm = await SupplierLogic.Get(Id ?? 0);

            if (vm == null)
            {
                return View("NotFound");
            }

            if ((Request.UrlReferrer != null) && (ReferrerController?.ToLower() != "account"))
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            if (vm.InvoiceCount > 0)
            {
                ViewBag.Warning = "You have Invoices with this Supplier.";
            }

            return View(vm);
        }

        // Delete (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(SupplierVM vm)
        {
            vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
            await SupplierLogic.Delete(vm);
            TempData["Message"] = "You have deleted a " + AssetType + " component!";

            if (!string.IsNullOrEmpty(vm.PreviousUrl))
            {
                return Redirect(vm.PreviousUrl);
            }
            else
            {
                return RedirectToAction("index");
            }
        }
    }
}