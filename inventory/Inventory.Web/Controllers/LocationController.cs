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
    [Authorize(Roles = "LocationsManage")]
    public class LocationController : AssetBaseMvcController
    {
        //Constructor
        public LocationController(ILocationLogic locationLogic, ICommonLogic commonLogic, IUserLogic userLogic)
        {
            LocationLogic = locationLogic;
            CommonLogic = commonLogic;
            UserLogic = userLogic;
            AssetType = "Location";
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
            ViewBag.CodeSortOrder = sortOrder == "code" ? "code_desc" : "code";
            ViewBag.DisplayNameSortOrder = sortOrder == "displayname" ? "displayname_desc" : "displayname";
            ViewBag.AliasSortOrder = sortOrder == "alias" ? "alias_desc" : "alias";
            ViewBag.BuildingCountSortOrder = sortOrder == "buildingcount" ? "buildingcount_desc" : "buildingcount";
            ViewBag.RoomCountSortOrder = sortOrder == "roomcount" ? "roomcount_desc" : "roomcount";
            ViewBag.LeadManagerSortOrder = sortOrder == "leadmanager" ? "leadmanager_desc" : "leadmanager";

            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var vmAssetList = await LocationLogic.GetList(sortOrder, currentFilter, searchString, AssetType, page, pageSize);

            return View(vmAssetList);
        }

        [Authorize(Roles = "LocationsManage")]
        public async Task<ActionResult> ExportToExcel(string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            var search = !string.IsNullOrEmpty(searchString) ? searchString : currentFilter;
            var vmList = await LocationLogic.GetList(sortOrder, currentFilter, search, AssetType, 1, 99999);

            var list = vmList.Select(x => new {
                x.PropertyId,
                x.Code,
                Name = x.DisplayName,
                Aliases = x.LocationAliasNames,
                x.BuildingCount,
                x.RoomCount,
                LeadManager = x.LeadManager.Name,
                Subnet = x.SubnetAddress,
                x.AddressLine1,
                x.AddressLine2,
                City = x.City.Name,
                x.PostalCode,
                x.Phone,
                x.Longitude,
                x.Latitude,
                Status = x.ActiveStatus
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
            var FileName = "Locations_" + DateTime.Now.ToString("MM/dd/yyyy") + ".xls";
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
        [Authorize(Roles = "LocationsManage")]
        public async Task<ActionResult> Create(string previousUrl)
        {
            var vm = new LocationVM
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

            vm.States = await CommonLogic.GetStates("fl");
            vm.Cities = await CommonLogic.GetCities("fl");
            vm.Users = await UserLogic.GetUsers();
            vm.LocationAmenities = await LocationLogic.GetLocationAmenities();

            return View(vm);
        }

        //Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "LocationsManage")]
        public async Task<ActionResult> Create(LocationVM vm)
        {
            if (!string.IsNullOrEmpty(vm.PropertyId) && await LocationLogic.PropertyIdExist(vm.PropertyId?.Trim()))
            {
                ModelState.AddModelError(nameof(vm.PropertyId), "The Property Id already exist in db");
            }
            ///
            else if (string.IsNullOrEmpty(vm.Name))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name is required");
            }
            else if (await CommonLogic.NameExist(vm.Name?.Trim(), AssetType))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name already exist in db");
            }
            else if (!string.IsNullOrEmpty(vm.DisplayName) && (await CommonLogic.NameExist(vm.DisplayName?.Trim(), AssetType)))
            {
                ModelState.AddModelError(nameof(vm.DisplayName), "The Name already exist in db");
            }
            ///
            else if (!string.IsNullOrEmpty(vm.Code) && (await LocationLogic.CodeExist(vm.Code)))
            {
                ModelState.AddModelError(nameof(vm.Code), "The Code already exist in db");
            }
            else if (vm.Longitude != null && vm.Longitude.InValidItem(RegexLongitude))
            {
                ModelState.AddModelError(nameof(vm.Longitude), "Longitude is not in the correct format");
            }
            else if (vm.Latitude != null && vm.Latitude.InValidItem(RegexLatitude))
            {
                ModelState.AddModelError(nameof(vm.Latitude), "Latitude is not in the correct format");
            }
            else if (!string.IsNullOrEmpty(vm.SubnetAddress) && vm.SubnetAddress.InValidItem(RegexIPAddress))
            {
                ModelState.AddModelError(nameof(vm.SubnetAddress), "Format must be 0-255.0-255.0-255.0-255");
            }
            else if (!string.IsNullOrEmpty(vm.SubnetAddress) && (await LocationLogic.SubnetExist(vm.SubnetAddress)))
            {
                ModelState.AddModelError(nameof(vm.SubnetAddress), "The Subnet already exist in db");
            }

            if (ModelState.IsValid)
            {
                vm.CreatedBy = Convert.ToInt32(User.Identity.GetUserId());
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                var itemId = await LocationLogic.Create(vm);
                TempData["Message"] = "You have created an " + AssetType + "!";
                TempData["AssetCreated"] = ViewBag.AssetType;
                return RedirectToAction("Details", new { id = itemId, previousUrl = vm.PreviousUrl });
            }

            vm.Cities = await CommonLogic.GetCities("fl");
            vm.States = await CommonLogic.GetStates("fl");
            vm.Users = await UserLogic.GetUsers();
            vm.LocationAmenities = await LocationLogic.GetLocationAmenities(vm.LocationAmenityIds);

            return View(vm);
        }

        //Details
        [Authorize(Roles = "LocationsManage")]
        public async Task<ActionResult> Details(int? Id, string previousUrl)
        {
            var vm = await LocationLogic.Get(Id ?? 0);
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
        [Authorize(Roles = "LocationsManage")]
        public async Task<ActionResult> Edit(int? Id, string previousUrl)
        {
            var vm = await LocationLogic.Get(Id ?? 0);
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

            vm.States = await CommonLogic.GetStates("fl");
            vm.Cities = await CommonLogic.GetCities("fl");
            vm.Users = await UserLogic.GetUsers();
            vm.LocationAmenities = await LocationLogic.GetLocationAmenities(vm.LocationAmenityIds);

            return View(vm);
        }

        //Update (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "LocationsManage")]
        public async Task<ActionResult> Edit(LocationVM vm)
        {
            if (!string.IsNullOrEmpty(vm.PropertyId) && vm.PropertyId?.Trim() != vm.OriginalPropertyId?.Trim() && await LocationLogic.PropertyIdExist(vm.PropertyId?.Trim()))
            {
                ModelState.AddModelError(nameof(vm.PropertyId), "The Property Id already exist in db");
            }
            else if (string.IsNullOrEmpty(vm.Name))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name is required");
            }
            else if ((vm.Name?.ToLower().Trim() != vm.OriginalName?.ToLower().Trim()) && (vm.Name?.ToLower().Trim() != vm.OriginalDisplayName?.ToLower().Trim()) && (await CommonLogic.NameExist(vm.Name, AssetType, 0, vm.Id)))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name already exist in db");
            }
            else if ((vm.DisplayName?.ToLower().Trim() != vm.OriginalDisplayName?.ToLower().Trim()) && (vm.DisplayName?.ToLower().Trim() != vm.OriginalName?.ToLower().Trim()) && (await CommonLogic.NameExist(vm.DisplayName, AssetType, 0, vm.Id)))
            {
                ModelState.AddModelError(nameof(vm.DisplayName), "The Name already exist in db");
            }
            else if (!string.IsNullOrEmpty(vm.Code) && (vm.Code != vm.OriginalCode) && (await LocationLogic.CodeExist(vm.Code)))
            {
                ModelState.AddModelError(nameof(vm.Code), "The Code already exist in db");
            }
            else if (vm.Longitude != null && vm.Longitude.InValidItem(RegexLongitude))
            {
                ModelState.AddModelError(nameof(vm.Longitude), "Longitude is not in the correct format");
            }
            else if (vm.Latitude != null && vm.Latitude.InValidItem(RegexLatitude))
            {
                ModelState.AddModelError(nameof(vm.Latitude), "Latitude is not in the correct format");
            }
            else if (!string.IsNullOrEmpty(vm.SubnetAddress) && vm.SubnetAddress.InValidItem(RegexIPAddress))
            {
                ModelState.AddModelError(nameof(vm.SubnetAddress), "Format must be 0-255.0-255.0-255.0-255");
            }
            else if (!string.IsNullOrEmpty(vm.SubnetAddress) && (vm.SubnetAddress != vm.OriginalSubnetAddress) && (await LocationLogic.SubnetExist(vm.SubnetAddress)))
            {
                ModelState.AddModelError(nameof(vm.SubnetAddress), "The Subnet already exist in db");
            }

            if (ModelState.IsValid)
            {
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                await LocationLogic.Save(vm);
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
                vm.Cities = await CommonLogic.GetCities("fl");
                vm.States = await CommonLogic.GetStates("fl");
                vm.Users = await UserLogic.GetUsers();
                vm.LocationAmenities = await LocationLogic.GetLocationAmenities(vm.LocationAmenityIds);

                return View(vm);
            }
        }

        // Delete (GET)
        [HttpGet]
        [Authorize(Roles = "LocationsManage")]
        public async Task<ActionResult> Delete(int? Id)
        {
             ReferrerController = await GetReferrerControlerNameAsync();

            ViewBag.Warning = "";

            var vm = await LocationLogic.Get(Id ?? 0);

            if (vm == null)
            {
                return View("NotFound");
            }

            if ((Request.UrlReferrer != null) && (ReferrerController?.ToLower() != "account"))
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            if (vm.AssetCount > 0)
            {
                ViewBag.Warning = "You have Assets with this Location.";
            }
            else if (vm.BuildingCount > 0)
            {
                ViewBag.Warning = "You have to remove Buildings prior to deleting an Location.";
            }

            return View(vm);
        }

        // Delete (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "LocationsManage")]
        public async Task<ActionResult> Delete(LocationVM vm)
        {
            vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
            await LocationLogic.Delete(vm);
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