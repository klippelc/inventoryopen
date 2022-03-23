using Microsoft.AspNet.Identity;
using Inventory.Web.Common;
using Inventory.Web.Logic;
using Inventory.Web.ViewModels;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Inventory.Web.Controllers
{
    [Authorize(Roles = "ManufacturersManage")]
    public class ManufacturerController : AssetBaseMvcController
    {
        //Constructor
        public ManufacturerController(IManuLogic manuLogic, ICommonLogic commonLogic)
        {
            ManuLogic = manuLogic;
            CommonLogic = commonLogic;
            AssetType = "Manufacturer";
            ViewBag.AssetType = AssetType;
            ViewBag.SettingsType = AssetType;
        }

        //Get All
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ModelState.Remove("SearchString");
            searchString = searchString.RemoveSpaces();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortOrder = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.DisplayNameSortOrder = sortOrder == "displayname" ? "displayname_desc" : "displayname";
            ViewBag.ProductCountSortOrder = sortOrder == "productcount" ? "productcount_desc" : "productcount";
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

            ViewBag.CurrentFilter = searchString?.Trim();

            var vmAssetList = await ManuLogic.GetList(sortOrder, currentFilter, searchString?.Trim(), AssetType, page, pageSize);

            return View(vmAssetList);
        }

        //Create 
        [HttpGet]
        public async Task<ActionResult> Create(string previousUrl)
        {
             ReferrerController = await GetReferrerControlerNameAsync();

            var vm = new ManufacturerVM
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

            return View(vm);
        }

        //Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ManufacturerVM vm)
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
                var itemId = await ManuLogic.Create(vm);
                TempData["Message"] = "You have created an " + AssetType + "!";
                TempData["AssetCreated"] = ViewBag.AssetType;
                return RedirectToAction("Details", new { id = itemId, previousUrl = vm.PreviousUrl });
            }

            return View(vm);
        }

        //Details
        public async Task<ActionResult> Details(int? Id, string previousUrl)
        {
            var vm = await ManuLogic.Get(Id ?? 0);
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
            var vm = await ManuLogic.Get(Id ?? 0);
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

        //Update (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ManufacturerVM vm)
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
                await ManuLogic.Save(vm);
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
            else
            {
                return View(vm);
            }
        }

        // Delete (GET)
        [HttpGet]
        public async Task<ActionResult> Delete(int? Id)
        {
             ReferrerController = await GetReferrerControlerNameAsync();

            ViewBag.Warning = "";

            var vm = await ManuLogic.Get(Id ?? 0);

            if (vm == null)
            {
                return View("NotFound");
            }

            if ((Request.UrlReferrer != null) && (ReferrerController?.ToLower() != "account"))
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            if (vm.InvoiceItemCount > 0)
            {
                ViewBag.Warning = "You have Invoice Items with this Manufacturer.";
            }
            else if (vm.ProductCount > 0)
            {
                ViewBag.Warning = "You have to remove Products prior to deleting an Manufacturer.";
            }

            return View(vm);
        }

        // Delete (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(ManufacturerVM vm)
        {
            vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
            await ManuLogic.Delete(vm);
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