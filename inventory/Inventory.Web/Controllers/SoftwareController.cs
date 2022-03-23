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
    public class SoftwareController : AssetBaseMvcController
    {
        //Constructor
        public SoftwareController(IAssetLogic assetLogic, ILocationLogic locationLogic,
            IBuildingLogic buildingLogic, IRoomLogic roomLogic, IUserLogic userLogic, ICommonLogic commonLogic)
        {
            AssetLogic = assetLogic;
            LocationLogic = locationLogic;
            BuildingLogic = buildingLogic;
            RoomLogic = roomLogic;
            UserLogic = userLogic;
            CommonLogic = commonLogic;
            AssetType = "Software";
            ViewBag.AssetType = AssetType;
        }

        //Get All
        [Authorize(Roles = "SoftwareView, ManageParkAssets")]
        public async Task<ActionResult> Index(int? assetId, bool? showConnectedAssets, string assetCategory, string licenseType, string sortOrder, string currentSearch, string searchString, int? invoiceItemId, int? assetListId, int? activeStatus, int page = 1)
        {
            ModelState.Remove("SearchString");
            searchString = searchString?.Trim();

            ViewBag.AssetId = assetId;
            ViewBag.ShowConnectedAssets = showConnectedAssets;
            ViewBag.AssetCategory = assetCategory;
            ViewBag.LicenseType = licenseType;
            ViewBag.InvoiceItemId = invoiceItemId;
            ViewBag.AssetListId = assetListId;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ActiveStatus = activeStatus;
            ViewBag.InvoiceSortOrder = sortOrder == "invoice" ? "invoice_desc" : "invoice";
            ViewBag.ManuSortOrder = sortOrder == "manufacturer" ? "manufacturer_desc" : "manufacturer";
            ViewBag.ProductSortOrder = sortOrder == "product" ? "product_desc" : "product";
            ViewBag.AssetSortOrder = sortOrder == "assettag" ? "assettag_desc" : "assettag";
            ViewBag.CategorySortOrder = sortOrder == "category" ? "category_desc" : "category";
            ViewBag.SerialSortOrder = sortOrder == "serial" ? "serial_desc" : "serial";
            ViewBag.AssignedUserSortOrder = sortOrder == "assigneduser" ? "assigneduser_desc" : "assigneduser";
            ViewBag.AssignedLocationSortOrder = sortOrder == "assignedlocation" ? "assignedlocation_desc" : "assignedlocation";
            ViewBag.LicenseTypeSortOrder = sortOrder == "licensetype" ? "licensetype_desc" : "licensetype";
            ViewBag.LicenseSortOrder = sortOrder == "license" ? "license_desc" : "license";
            ViewBag.AssignedAssetSortOrder = sortOrder == "assignedasset" ? "assignedasset_desc" : "assignedasset";
            ViewBag.StatusSortOrder = sortOrder == "status" ? "status_desc" : "status";

            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentSearch;
            }

            ViewBag.CurrentSearch = searchString;

            var parameters = new Parameters
            {
                InvoiceItemId = invoiceItemId,
                AssetId = assetId,
                ShowConnectedAssets = showConnectedAssets,
                ActiveStatus = activeStatus ?? 0,
                AssetType = AssetType,
                LicenseType = licenseType,
                AssetCategory = assetCategory,
                SortOrder = sortOrder,
                SearchString = searchString,
                LoginUserId = LoginUserId,
                CurrentPage = page,
                PageSize = pageSize,
                LoginUserLocId = await GetUserLocId(),
                Roles = await GetRoles(),
                AssetListId = assetListId
            };

            var vmAssetList = await AssetLogic.GetAssetList(parameters);

            return View(vmAssetList);
        }

        [Authorize(Roles = "SoftwareView, ManageParkAssets")]
        public async Task<ActionResult> ExportToExcel(int? assetId, bool? showConnectedAssets, string assetCategory, string licenseType, string sortOrder, string currentSearch, string searchString, int? invoiceItemId, int? assetListId, int? activeStatus, int page = 1)
        {
            var parameters = new Parameters
            {
                InvoiceItemId = invoiceItemId,
                AssetId = assetId,
                ShowConnectedAssets = showConnectedAssets,
                ActiveStatus = activeStatus ?? 0,
                AssetType = AssetType,
                LicenseType = licenseType,
                AssetCategory = assetCategory,
                SortOrder = sortOrder,
                SearchString = string.IsNullOrEmpty(searchString) ? currentSearch : searchString,
                LoginUserId = LoginUserId,
                Export = true,
                LoginUserLocId = await GetUserLocId(),
                Roles = await GetRoles(),
                AssetListId = assetListId
            };

            var vmAssetList = await AssetLogic.GetAssetList(parameters);
            var itemList = vmAssetList.Select(x => x.Assets).FirstOrDefault();
            var list = itemList.Select(x => new {
                x.Id,
                Manufacturer = x.ManufacturerName,
                Product = x.ProductName,
                LicenseType = x.LicenseTypeName,
                License = !string.IsNullOrEmpty(x.LicenseKeyMulti) ? x.LicenseKeyMulti : x.LicenseKeySingle,
                Category = x.InvoiceItem?.AssetCategory.Name,
                AssignedAsset = x.AssignedAsset.Serial,
                AssignedUser = x.AssignedUser.Name,
                PONumber = x.InvoiceItem?.Invoice?.PONumber,
                Status = x.AssetStatusName,
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
            var FileName = "Software_" + DateTime.Now.ToString("MM/dd/yyyy") + ".xls";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            //

            return View("Export");
        }

        //GET Bulk Assets (GET)
        [HttpGet]
        [Authorize(Roles = "SoftwareEdit, ManageParkAssets")]
        public async Task<JsonResult> BulkEdit(string assets)
        {
            var LoginUserLocId = await GetUserLocId();
            var Roles = await GetRoles();
            bool isAdmin = false;

            if (Roles.Contains("SoftwareEdit") && (AssetType == "Software"))
            {
                isAdmin = true;
            }

            var vm = await AssetLogic.GetBulkAssets(assets, AssetType, LoginUserLocId, isAdmin);

            var jsonResult = Json(JsonConvert.SerializeObject(vm), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        //POST Bulk Assets (POST)
        [HttpPost]
        [Authorize(Roles = "SoftwareEdit, ManageParkAssets")]
        public async Task<JsonResult> BulkEdit(AssetBulkVM vm)
        {
            IDictionary<string, string> errors = new Dictionary<string, string>();

            if (ModelState.IsValid)
            {
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                var bulkasset = await AssetLogic.UpdateBulkAssets(vm, AssetType);
                return Json(new { IsUpdated = true, Content = bulkasset }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                IsUpdated = false,
                errors,
                Content = vm
            }, JsonRequestBehavior.AllowGet);
        }

        //Details
        [Authorize(Roles = "SoftwareView, ManageParkAssets")]
        public async Task<ActionResult> Details(int? Id)
        {
            var vm = await AssetLogic.GetAsset(Id ?? 0, AssetType);
             ReferrerController = await GetReferrerControlerNameAsync();

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

        //Update (GET)
        [HttpGet]
        [Authorize(Roles = "SoftwareEdit")]
        public async Task<ActionResult> Edit(int? Id, string previousUrl)
        {
            var vm = await AssetLogic.GetAsset(Id ?? 0, AssetType);
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

            vm.Locations = vm.Locations?.Append(vm.Location)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);
            vm.Buildings = vm.Buildings?.Append(vm.Building)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);
            vm.Rooms = vm.Rooms?.Append(vm.Room)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);
            vm.Users = vm.Users?.Append(vm.AssignedUser)?.Where(x => x.Id > 0).DistinctBy(x => x.Id).OrderBy(x => x.Name);
            vm.Assets = vm.Assets?.Append(vm.AssignedAsset).Where(x => x.Id > 0).DistinctBy(x => x.Id).OrderBy(x => x.SerialandAssignedUser);

            return View(vm);
        }

        //Update (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SoftwareEdit")]
        public async Task<ActionResult> Edit(AssetVM vm)
        {
            var licenseTypeName = await AssetLogic.GetAssetLicenseType(vm.InvoiceItemId);

            ModelState.Remove("ExpirationDate");
            vm.ExpirationDate = vm.ExpirationDate;

            //License Check
            string existLicense = null;
            ModelState.Remove("LicenseKeyMulti");
            vm.LicenseKeyMulti = vm.LicenseKeyMulti.RemoveSpaces();

            if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && vm.LicenseKeyMulti != vm.OriginalLicenseKeyMulti.RemoveSpaces())
            {
                existLicense = await AssetLogic.CheckLicenses(vm.LicenseKeyMulti.ToStringList(), vm.LicenseTypeName);
            }

            if (licenseTypeName == "Hardware-Multi")
            {
                if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && (vm.LicenseKeyMulti.Length < 5 || vm.LicenseKeyMulti.Length > 30))
                {
                    ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "Please check the License Key");
                }
                else if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && vm.LicenseKeyMulti.InValidItem(RegexLicense))
                {
                    ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "License Key is not in the correct format");
                }
                else if (!string.IsNullOrEmpty(vm.LicenseKeyMulti) && !string.IsNullOrEmpty(existLicense))
                {
                    ModelState.AddModelError(nameof(vm.LicenseKeyMulti), "License Key already exist in db");
                }
            }
            if ((licenseTypeName != "User") && (vm.AssignedAssetId != vm.OriginalAssignedAssetId) && (await CommonLogic.IsDeleted(vm.AssignedAssetId, "Asset")))
            {
                ModelState.AddModelError(nameof(vm.AssignedAssetId), "Asset was recently deleted");
            }

            if (ModelState.IsValid)
            {
                vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
                await AssetLogic.SaveAsset(vm);
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
                vm.Statuses = await AssetLogic.GetAssetStatuses(AssetType, vm.LicenseTypeName);
                vm.Locations = await LocationLogic.GetLocations();
                vm.Buildings = await BuildingLogic.GetBuildings(vm.LocationId ?? 0);
                vm.Rooms = await RoomLogic.GetRooms(vm.BuildingId ?? 0);
                vm.Users = await UserLogic.GetUsers();

                vm.Locations = vm.Locations?.Append(vm.Location)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);
                vm.Buildings = vm.Buildings?.Append(vm.Building)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);
                vm.Rooms = vm.Rooms?.Append(vm.Room)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);
                vm.Users = vm.Users?.Append(vm.AssignedUser)?.Where(x => x.Id > 0).DistinctBy(x => x.Id).OrderBy(x => x.Name);

                vm.Assets = await AssetLogic.GetComputerTypeAssets(null);
                vm.Assets = vm.Assets?.Append(vm.AssignedAsset).Where(x => x.Id > 0).DistinctBy(x => x.Id).OrderBy(x => x.SerialandAssignedUser);

                return View(vm);
            }
        }

        // Delete (GET)
        [HttpGet]
        [Authorize(Roles = "SoftwareDelete")]
        public async Task<ActionResult> Delete(int? Id)
        {
            var vm = await AssetLogic.GetAsset(Id ?? 0, AssetType);
            ReferrerController = await GetReferrerControlerNameAsync();

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
        [Authorize(Roles = "SoftwareDelete")]
        public async Task<ActionResult> Delete(AssetVM vm)
        {
            vm.ModifiedBy = Convert.ToInt32(User.Identity.GetUserId());
            await AssetLogic.DeleteAsset(vm);
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