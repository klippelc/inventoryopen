using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Inventory.Web.Common;
using Inventory.Web.Logic;
using Inventory.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Inventory.Web.Controllers
{
    [Authorize(Roles = "ProductsManage")]
    public class ProductController : AssetBaseMvcController
    {
        //Constructor
        public ProductController(IProductLogic productLogic, ICommonLogic commonLogic, IAssetLogic assetLogic, IManuLogic manuLogic)
        {
            ProductLogic = productLogic;
            CommonLogic = commonLogic;
            AssetLogic = assetLogic;
            ManuLogic = manuLogic;
            AssetType = "Product";
            ViewBag.AssetType = AssetType;
            ViewBag.SettingsType = AssetType;
        }

        //Get All
        public async Task<ActionResult> Index(int? manuId, string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ModelState.Remove("SearchString");
            searchString = searchString?.Trim();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.ManuId = manuId;
            ViewBag.AssetTypeSortOrder = sortOrder == "assettype" ? "assettype_desc" : "assettype";
            ViewBag.AssetCategorySortOrder = sortOrder == "assetcat" ? "assetcat_desc" : "assetcat";
            ViewBag.ManuSortOrder = sortOrder == "manu" ? "manu_desc" : "manu";
            ViewBag.DisplayNameSortOrder = sortOrder == "displayname" ? "displayname_desc" : "displayname";


            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var vmAssetList = await ProductLogic.GetList(manuId, sortOrder, currentFilter, searchString, AssetType, page, pageSize);

            return View(vmAssetList);
        }

        //Create 
        [HttpGet]
        public async Task<ActionResult> Create(string previousUrl)
        {
            var vm = new ProductVM
            {
                Active = true
            };
            
             ReferrerController = await GetReferrerControlerNameAsync();

            if (!string.IsNullOrEmpty(previousUrl))
            {
                vm.PreviousUrl = previousUrl;
            }
            else if ((Request.UrlReferrer != null) && (ReferrerController?.ToLower() != "account"))
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            vm.AssetTypes = await CommonLogic.GetAssetTypes();
            vm.AssetCategories = new List<AssetCategoryVM>();
            vm.Manufacturers = await ManuLogic.GetManufacturers(null, null);

            return View(vm);
        }

        //Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductVM vm)
        {
            if (string.IsNullOrEmpty(vm.Name))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name is required");
            }
            else if (!string.IsNullOrEmpty(vm.Name) && (await CommonLogic.NameExist(vm.Name, AssetType, vm.ManuId ?? 0)))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name already exist in db");
            }
            else if (!string.IsNullOrEmpty(vm.DisplayName) && (await CommonLogic.NameExist(vm.DisplayName, AssetType, vm.ManuId ?? 0)))
            {
                ModelState.AddModelError(nameof(vm.DisplayName), "The Name already exist in db");
            }
            else if ((vm.ManuId != null) && (await CommonLogic.IsDeleted(vm.ManuId, "Manufacturer")))
            {
                vm.ManuId = null;
                ModelState.AddModelError(nameof(vm.ManuId), "The Manufacturer was recently deleted");
            }

            if (ModelState.IsValid)
            {
                vm.CreatedBy = Convert.ToInt32(User.Identity.GetUserId());
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                var itemId = await ProductLogic.Create(vm);
                TempData["Message"] = "You have created an " + AssetType + "!";
                TempData["AssetCreated"] = ViewBag.AssetType;
                return RedirectToAction("Details", new { id = itemId, previousUrl = vm.PreviousUrl });
            }

            vm.AssetTypes = await CommonLogic.GetAssetTypes();
            vm.AssetCategories = await AssetLogic.GetAssetCategoriesByType(vm.AssetTypeId);
            vm.Manufacturers = await ManuLogic.GetManufacturers(null, null);

            return View(vm);
        }

        //Details
        public async Task<ActionResult> Details(int? Id, string previousUrl)
        {
            var vm = await ProductLogic.Get(Id ?? 0);
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
            var vm = await ProductLogic.Get(Id ?? 0);
             ReferrerController = await GetReferrerControlerNameAsync();

            ViewBag.Warning = "";

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

            if (vm.InvoiceItemCount > 0)
            {
                ViewBag.Warning = "You have Invoice Items associated to this Product.";
            }

            vm.AssetTypes = await CommonLogic.GetAssetTypes();
            vm.AssetCategories = await AssetLogic.GetAssetCategoriesByType(vm.AssetTypeId);
            vm.Manufacturers = await ManuLogic.GetManufacturers(null, null);
            vm.Manufacturers = vm.Manufacturers?.Append(vm.Manufacturer)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);

            return View(vm);
        }

        //Update (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductVM vm)
        {
            if (string.IsNullOrEmpty(vm.Name))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name is required");
            }
            else if ((vm.ManuId != vm.OriginalManuId) && (await CommonLogic.NameExist(vm.Name, AssetType, vm.ManuId ?? 0)))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name already exist in db");
            }
            else if ((vm.ManuId != vm.OriginalManuId) && (!string.IsNullOrEmpty(vm.DisplayName)) && (await CommonLogic.NameExist(vm.DisplayName, AssetType, vm.ManuId ?? 0)))
            {
                ModelState.AddModelError(nameof(vm.DisplayName), "The Name already exist in db");
            }
            else if ((vm.Name?.ToLower().Trim() != vm.OriginalName?.ToLower().Trim()) && (vm.Name?.ToLower().Trim() != vm.OriginalDisplayName?.ToLower().Trim()) && (await CommonLogic.NameExist(vm.Name, AssetType, vm.ManuId ?? 0)))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name already exist in db");
            }
            else if ((vm.DisplayName?.ToLower().Trim() != vm.OriginalDisplayName?.ToLower().Trim()) && (vm.DisplayName?.ToLower().Trim() != vm.OriginalName?.ToLower().Trim()) && (await CommonLogic.NameExist(vm.DisplayName, AssetType, vm.ManuId ?? 0)))
            {
                ModelState.AddModelError(nameof(vm.DisplayName), "The Display Name already exist in db");
            }
            else if ((vm.ManuId != vm.OriginalManuId) && (await CommonLogic.IsDeleted(vm.ManuId, "Manufacturer")))
            {
                vm.ManuId = null;
                ModelState.AddModelError(nameof(vm.ManuId), "The Manufacturer was recently deleted");
            }

            if (ModelState.IsValid)
            {
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                await ProductLogic.Save(vm);
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
                vm.AssetTypes = await CommonLogic.GetAssetTypes();
                vm.AssetCategories = await AssetLogic.GetAssetCategoriesByType(vm.AssetTypeId);
                vm.Manufacturers = await ManuLogic.GetManufacturers(null, null);
                vm.Manufacturers = vm.Manufacturers?.Append(vm.Manufacturer)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);

                return View(vm);
            }
        }

        // Delete (GET)
        [HttpGet]
        public async Task<ActionResult> Delete(int? Id)
        {
             ReferrerController = await GetReferrerControlerNameAsync();

            ViewBag.Warning = "";

            var vm = await ProductLogic.Get(Id ?? 0);

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
                ViewBag.Warning = "You have Invoice Items with this Product.";
            }

            return View(vm);
        }

        // Delete (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(ProductVM vm)
        {
            vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
            await ProductLogic.Delete(vm);
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