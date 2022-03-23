using Inventory.Web.Common;
using Inventory.Web.Logic;
using Inventory.Web.ViewModels;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Inventory.Web.Controllers
{
    [Authorize(Roles = "AssetListManage")]
    public class AssetListController : AssetBaseMvcController
    {
        //Constructor
        public AssetListController(IAssetListLogic assetListLogic, ICommonLogic commonLogic)
        {
            AssetListLogic = assetListLogic;
            CommonLogic = commonLogic;
            AssetType = "AssetList";
            ViewBag.PageName = "Asset List";
            ViewBag.AssetType = AssetType;
        }

        //Get All
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ModelState.Remove("SearchString");
            searchString = searchString.RemoveSpaces();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortOrder = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.NameSortOrder = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.DescriptionSortOrder = sortOrder == "description" ? "description_desc" : "description";
            ViewBag.AssetTypeSortOrder = sortOrder == "assettype" ? "assettype_desc" : "assettype";
            ViewBag.SharedSortOrder = sortOrder == "shared" ? "shared_desc" : "shared";
            ViewBag.ItemCountSortOrder = sortOrder == "itemcount" ? "itemcount_desc" : "itemcount";

            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString?.Trim();

            var vmAssetList = await AssetListLogic.GetList(LoginUserId, sortOrder, currentFilter, searchString?.Trim(), page, pageSize);

            return View(vmAssetList);
        }

        //Create 
        [HttpGet]
        public async Task<ActionResult> Create(string previousUrl)
        {
            var vm = await AssetListLogic.GetBlankAssetList();
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
        public async Task<ActionResult> Create(AssetListVM vm)
        {
            if (string.IsNullOrEmpty(vm.Name))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name is required");
            }
            else if (!string.IsNullOrEmpty(vm.Name) && (await CommonLogic.NameExist(vm.Name, AssetType, 0, 0, 0, LoginUserId)))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name already exist in db");
            }
            else if (vm.AssetTypeId == null)
            {
                ModelState.AddModelError(nameof(vm.AssetTypeId), "The Asset Type is required");
            }

            if (ModelState.IsValid)
            {
                vm.UserId = LoginUserId;
                var itemId = await AssetListLogic.Create(vm);
                TempData["Message"] = "You have created an " + AssetType + "!";
                TempData["AssetCreated"] = ViewBag.AssetType;
                return RedirectToAction("Details", new { id = itemId, previousUrl = vm.PreviousUrl });
            }
            else
            {
                vm.AssetTypes = await CommonLogic.GetAssetTypes();
                return View(vm);
            }

        }

        //Details
        public async Task<ActionResult> Details(int? Id, string previousUrl)
        {
            var vm = await AssetListLogic.Get(Id ?? 0, LoginUserId);
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

        //Update (GET)
        [HttpGet]
        public async Task<ActionResult> Edit(int? Id, string previousUrl)
        {
            var vm = await AssetListLogic.Get(Id ?? 0, LoginUserId);
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
        public async Task<ActionResult> Edit(AssetListVM vm)
        {
            if (string.IsNullOrEmpty(vm.Name))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name is required");
            }
            else if ((vm.Name?.ToLower().Trim() != vm.OriginalName?.ToLower().Trim()) && (await CommonLogic.NameExist(vm.Name, AssetType, 0, 0, 0, LoginUserId)))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name already exist in db");
            }
            else if ((vm.ItemCount == 0) && (vm.AssetTypeId == null))
            {
                ModelState.AddModelError(nameof(vm.AssetTypeId), "The Asset Type is required");
            }

            if (ModelState.IsValid)
            {
                vm.ModifiedBy = LoginUserId;
                await AssetListLogic.Save(vm);
                TempData["Message"] = "You have updated a " + AssetType + " item!";
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
                if (vm.ItemCount > 0)
                {
                    vm.AssetTypeId = vm.OriginalAssetTypeId;
                }
                vm.AssetTypes = await CommonLogic.GetAssetTypes();
                return View(vm);
            }
        }

        // Delete (GET)
        [HttpGet]
        public async Task<ActionResult> Delete(int? Id)
        {
             ReferrerController = await GetReferrerControlerNameAsync();

            ViewBag.Warning = "";

            var vm = await AssetListLogic.Get(Id ?? 0, LoginUserId);

            if (vm == null)
            {
                return View("NotFound");
            }

            if ((Request.UrlReferrer != null) && (ReferrerController?.ToLower() != "account"))
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            return View(vm);
        }

        // Delete (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(AssetListVM vm)
        {
            vm.ModifiedBy = LoginUserId;
            await AssetListLogic.Delete(vm);
            TempData["Message"] = "You have deleted a " + AssetType + " item!";

            if (!string.IsNullOrEmpty(vm.PreviousUrl))
            {
                return Redirect(vm.PreviousUrl);
            }
            else
            {
                return RedirectToAction("index");
            }
        }

        //GET Asset List By User and AssetType (GET)
        [HttpGet]
        public async Task<JsonResult> GetAssetList(string assetType)
        {
            var Roles = await GetRoles();

            var vm = await AssetListLogic.GetUserLists(LoginUserId, assetType);
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        //GET Asset List By Id and User (GET)
        [HttpGet]
        public async Task<JsonResult> GetAssetListById(int AssetListId, string assetType)
        {
            var vm = await AssetListLogic.GetUserList(AssetListId, LoginUserId);
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        //POST Add Assets to Asset-User List
        [HttpPost]
        public async Task<JsonResult> AddAssetsToUserList(AssetListVM vm)
        {
            vm.UserId = LoginUserId;

            var result = await AssetListLogic.AddAssetsToUserList(vm);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //POST Remove Assets from Asset-User List
        [HttpPost]
        public async Task<JsonResult> RemoveAssetsFromUserList(AssetListVM vm)
        {
            vm.UserId = LoginUserId;

            var result = await AssetListLogic.RemoveAssetsFromUserList(vm);
            return Json(result, JsonRequestBehavior.AllowGet);

        }
    }
}
