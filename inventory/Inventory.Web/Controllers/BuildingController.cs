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
    [Authorize(Roles = "BuildingsManage")]
    public class BuildingController : AssetBaseMvcController
    {
        //Constructor
        public BuildingController(IBuildingLogic buildingLogic, ICommonLogic commonLogic, ILocationLogic locationLogic)
        {
            BuildingLogic = buildingLogic;
            CommonLogic = commonLogic;
            LocationLogic = locationLogic;
            AssetType = "Building";
            ViewBag.AssetType = AssetType;
            ViewBag.SettingsType = AssetType;
        }

        //Get All
        [Authorize(Roles = "BuildingsManage")]
        public async Task<ActionResult> Index(int? locationId, string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            ModelState.Remove("SearchString");
            searchString = searchString?.Trim();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.LocationId = locationId;
            ViewBag.IdSortOrder = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.CodeSortOrder = sortOrder == "code" ? "code_desc" : "code";
            ViewBag.DisplayNameSortOrder = sortOrder == "displayname" ? "displayname_desc" : "displayname";
            ViewBag.RoomCountSortOrder = sortOrder == "roomcount" ? "roomcount_desc" : "roomcount";
            ViewBag.LocationSortOrder = sortOrder == "location" ? "location_desc" : "location";
            ViewBag.LocationCodeSortOrder = sortOrder == "locationcode" ? "locationcode_desc" : "locationcode";

            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var vmAssetList = await BuildingLogic.GetList(locationId, sortOrder, currentFilter, searchString, AssetType, page, pageSize);

            return View(vmAssetList);
        }

        [Authorize(Roles = "BuildingsManage")]
        public async Task<ActionResult> ExportToExcel(int? locationId, string sortOrder, string currentFilter, string searchString, int page = 1)
        {
            var search = !string.IsNullOrEmpty(searchString) ? searchString : currentFilter;
            var vmList = await BuildingLogic.GetList(locationId, sortOrder, currentFilter, search, AssetType, 1, 99999);

            var list = vmList.Select(x => new {
                LocationCode = x.Location.Code,
                Location = x.Location.DisplayName,
                BuildingPID = x.PropertyId,
                BuildingCode = x.Code,
                Building = x.DisplayName,
                x.RoomCount,
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
            var FileName = "Buildings_" + DateTime.Now.ToString("MM/dd/yyyy") + ".xls";
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
        [Authorize(Roles = "BuildingsManage")]
        public async Task<ActionResult> Create(string previousUrl)
        {
            var vm = new BuildingVM
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
            vm.Locations = await LocationLogic.GetLocations();

            return View(vm);
        }

        //Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "BuildingsManage")]
        public async Task<ActionResult> Create(BuildingVM vm)
        {
            if (!string.IsNullOrEmpty(vm.PropertyId) && await BuildingLogic.PropertyIdExist(vm.PropertyId?.Trim(), vm.LocationId))
            {
                ModelState.AddModelError(nameof(vm.PropertyId), "The Property Id already exist for this location");
            }
            else if (string.IsNullOrEmpty(vm.Name))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name is required");
            }
            else if (await CommonLogic.NameExist(vm.Name?.Trim(), AssetType, 0, vm.LocationId ?? 0))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name already exist in db");
            }
            else if (!string.IsNullOrEmpty(vm.DisplayName) && (await CommonLogic.NameExist(vm.DisplayName?.Trim(), AssetType, 0, vm.LocationId ?? 0)))
            {
                ModelState.AddModelError(nameof(vm.DisplayName), "The Name already exist in db");
            }
            else if (!string.IsNullOrEmpty(vm.Code) && (await BuildingLogic.CodeExist(vm.Code)))
            {
                ModelState.AddModelError(nameof(vm.Code), "The Code already exist in db");
            }
            else if ((vm.LocationId != null) && (await CommonLogic.IsDeleted(vm.LocationId, "Location")))
            {
                vm.LocationId = null;
                ModelState.AddModelError(nameof(vm.LocationId), "The Location was recently deleted");
            }
            else if (vm.Longitude != null && vm.Longitude.InValidItem(RegexLongitude))
            {
                ModelState.AddModelError(nameof(vm.Longitude), "Longitude is not in the correct format");
            }
            else if (vm.Latitude != null && vm.Latitude.InValidItem(RegexLatitude))
            {
                ModelState.AddModelError(nameof(vm.Latitude), "Latitude is not in the correct format");
            }

            if (ModelState.IsValid)
            {
                vm.CreatedBy = Convert.ToInt32(User.Identity.GetUserId());
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                var itemId = await BuildingLogic.Create(vm);
                TempData["Message"] = "You have created an " + AssetType + "!";
                TempData["AssetCreated"] = ViewBag.AssetType;
                return RedirectToAction("Details", new { id = itemId, previousUrl = vm.PreviousUrl });
            }

            vm.States = await CommonLogic.GetStates("fl");
            vm.Cities = await CommonLogic.GetCities("fl");
            vm.Locations = await LocationLogic.GetLocations();

            return View(vm);
        }

        //Details
        [Authorize(Roles = "BuildingsManage")]
        public async Task<ActionResult> Details(int? Id, string previousUrl)
        {
            var vm = await BuildingLogic.Get(Id ?? 0);
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
        [Authorize(Roles = "BuildingsManage")]
        public async Task<ActionResult> Edit(int? Id, string previousUrl)
        {
            var vm = await BuildingLogic.Get(Id ?? 0);
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

            if (vm.AssetCount > 0)
            {
                ViewBag.Warning = "You have Assets associated to this Building.";
            }

            vm.States = await CommonLogic.GetStates("fl");
            vm.Cities = await CommonLogic.GetCities("fl");
            vm.Locations = await LocationLogic.GetLocations();
            vm.Locations = vm.Locations?.Append(vm.Location)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);


            return View(vm);
        }

        //Update (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "BuildingsManage")]
        public async Task<ActionResult> Edit(BuildingVM vm)
        {
            if (!string.IsNullOrEmpty(vm.PropertyId) && vm.PropertyId?.Trim() != vm.OriginalPropertyId?.Trim() && await BuildingLogic.PropertyIdExist(vm.PropertyId?.Trim(), vm.LocationId))
            {
                ModelState.AddModelError(nameof(vm.PropertyId), "The Property Id already exist for this location");
            }
            else if (string.IsNullOrEmpty(vm.Name))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name is required");
            }
            else if ((vm.LocationId != vm.OriginalLocationId) && (await CommonLogic.NameExist(vm.Name, AssetType, 0, vm.LocationId ?? 0)))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name already exist in db");
            }
            else if ((vm.LocationId != vm.OriginalLocationId) && (!string.IsNullOrEmpty(vm.DisplayName)) && (await CommonLogic.NameExist(vm.DisplayName, AssetType, 0, vm.LocationId ?? 0)))
            {
                ModelState.AddModelError(nameof(vm.DisplayName), "The Name already exist in db");
            }
            else if ((vm.Name?.ToLower().Trim() != vm.OriginalName?.ToLower().Trim()) && (vm.Name?.ToLower().Trim() != vm.OriginalDisplayName?.ToLower().Trim()) && (await CommonLogic.NameExist(vm.Name, AssetType, 0, vm.LocationId ?? 0)))
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name already exist in db");
            }
            else if ((vm.DisplayName?.ToLower().Trim() != vm.OriginalDisplayName?.ToLower().Trim()) && (vm.DisplayName?.ToLower().Trim() != vm.OriginalName?.ToLower().Trim()) && (await CommonLogic.NameExist(vm.DisplayName, AssetType, 0, vm.LocationId ?? 0)))
            {
                ModelState.AddModelError(nameof(vm.DisplayName), "The Display Name already exist in db");
            }
            else if ((vm.Code != vm.OriginalCode) && (await BuildingLogic.CodeExist(vm.Code)))
            {
                ModelState.AddModelError(nameof(vm.Code), "The Code already exist in db");
            }
            else if ((vm.LocationId != vm.OriginalLocationId) && (await CommonLogic.IsDeleted(vm.LocationId, "Location")))
            {
                vm.LocationId = null;
                ModelState.AddModelError(nameof(vm.LocationId), "The Location was recently deleted");
            }
            else if (vm.Longitude != null && vm.Longitude.InValidItem(RegexLongitude))
            {
                ModelState.AddModelError(nameof(vm.Longitude), "Longitude is not in the correct format");
            }
            else if (vm.Latitude != null && vm.Latitude.InValidItem(RegexLatitude))
            {
                ModelState.AddModelError(nameof(vm.Latitude), "Latitude is not in the correct format");
            }

            if (ModelState.IsValid)
            {
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                await BuildingLogic.Save(vm);
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
                vm.States = await CommonLogic.GetStates("fl");
                vm.Cities = await CommonLogic.GetCities("fl");
                vm.Locations = await LocationLogic.GetLocations();
                vm.Locations = vm.Locations?.Append(vm.Location)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);

                return View(vm);
            }
        }

        // Delete (GET)
        [HttpGet]
        [Authorize(Roles = "BuildingsManage")]
        public async Task<ActionResult> Delete(int? Id)
        {
             ReferrerController = await GetReferrerControlerNameAsync();

            ViewBag.Warning = "";

            var vm = await BuildingLogic.Get(Id ?? 0);

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
                ViewBag.Warning = "You have Assets with this Building.";
            }
            else if (vm.RoomCount > 0)
            {
                ViewBag.Warning = "You have to remove Rooms prior to deleting an Building.";
            }

            return View(vm);
        }

        // Delete (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "BuildingsManage")]
        public async Task<ActionResult> Delete(BuildingVM vm)
        {
            vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
            await BuildingLogic.Delete(vm);
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