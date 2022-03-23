using Microsoft.AspNet.Identity;
using Inventory.Web.Common;
using Inventory.Web.Logic;
using Inventory.Web.ViewModels;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Inventory.Web.Controllers
{
    [Authorize(Roles = "UsersManage")]
    public class UserController : AssetBaseMvcController
    {
        //Constructor
        public UserController(IUserLogic userLogic)
        {
            UserLogic = userLogic;
            AssetType = "User";
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
            ViewBag.NameSortOrder = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.UserNameSortOrder = sortOrder == "username" ? "username_desc" : "username";
            ViewBag.TitleSortOrder = sortOrder == "title" ? "title_desc" : "title";
            ViewBag.ParkSortOrder = sortOrder == "park" ? "park_desc" : "park";
            ViewBag.PhoneSortOrder = sortOrder == "phone" ? "phone_desc" : "phone";
            ViewBag.MobileSortOrder = sortOrder == "mobile" ? "mobile_desc" : "mobile";
            ViewBag.ManagerSortOrder = sortOrder == "manager" ? "manager_desc" : "manager";
            ViewBag.LastLoginSortOrder = sortOrder == "lastlogin" ? "lastlogin_desc" : "lastlogin";
            ViewBag.LastLogonSortOrder = sortOrder == "lastlogon" ? "lastlogon_desc" : "lastlogon";

            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var vmAssetList = await UserLogic.GetList(sortOrder, currentFilter, searchString, AssetType, page, pageSize);

            return View(vmAssetList);
        }

        //Details
        public async Task<ActionResult> Details(int? Id, string previousUrl)
        {
            var vm = await UserLogic.Get(Id ?? 0);
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
            var vm = await UserLogic.Get(Id ?? 0);
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

            vm.UserRoles = await UserLogic.GetRoles(vm.UserRoleIds);

            return View(vm);
        }

        //Update (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserVM vm)
        {
            if (ModelState.IsValid)
            {
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                await UserLogic.Save(vm);
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
            vm.UserRoles = await UserLogic.GetRoles(vm.UserRoleIds);

            return View(vm);
        }
    }
}