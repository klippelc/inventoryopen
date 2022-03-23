using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Inventory.Web.Common;
using Inventory.Web.Logic;
using Inventory.Web.ViewModels;
using System;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Inventory.Web.Controllers
{
    public class HardwareController : AssetBaseMvcController
    {
        //Constructor
        public HardwareController(IAssetLogic assetLogic, ILocationLogic locationLogic,
            IBuildingLogic buildingLogic, IRoomLogic roomLogic, IUserLogic userLogic, ICommonLogic commonLogic)
        {
            AssetLogic = assetLogic;
            LocationLogic = locationLogic;
            BuildingLogic = buildingLogic;
            RoomLogic = roomLogic;
            UserLogic = userLogic;
            CommonLogic = commonLogic;
            AssetType = "Hardware";
            ViewBag.AssetType = AssetType;
        }

        //Get All
        [Authorize(Roles = "HardwareView, ManageParkAssets")]
        public async Task<ActionResult> Index(int? manuId, int? productId, int? assetId, bool? showConnectedAssets, string assetCategory, bool? showCategories, bool? showLocations, int? locationId, bool? showBuildings, int? buildingId, bool? showRooms, int? roomId, string sortOrder, string currentSearch, string searchString, int? invoiceItemId, int? assetListId, int? activeStatus, int? pSize, int page = 1)
        {
            ModelState.Remove("SearchString");
            searchString = searchString?.Trim();

            ViewBag.AssetId = assetId;
            ViewBag.ShowConnectedAssets = showConnectedAssets;
            ViewBag.ShowLocations = locationId != null ? true : showLocations != null ? showLocations : false;
            ViewBag.ShowBuildings = buildingId != null ? true : showBuildings != null ? showBuildings : false;
            ViewBag.ShowRooms = roomId != null ? true : showRooms != null ? showRooms : false;
            ViewBag.ShowCategories = assetCategory != null ? true : showCategories != null ? showCategories : false;
            ViewBag.Location = locationId;
            ViewBag.Building = buildingId;
            ViewBag.Room = roomId;
            ViewBag.AssetCategory = assetCategory;
            ViewBag.InvoiceItemId = invoiceItemId;
            ViewBag.AssetListId = assetListId;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ActiveStatus = activeStatus ?? 100;
            ViewBag.Manufacturer = manuId;
            ViewBag.Product = productId;
            ViewBag.ConnectedAssetSortOrder = sortOrder == "connected" ? "connected" : "connected";
            ViewBag.NameSortOrder = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.InvoiceSortOrder = sortOrder == "invoice" ? "invoice_desc" : "invoice";
            ViewBag.ManuSortOrder = sortOrder == "manufacturer" ? "manufacturer_desc" : "manufacturer";
            ViewBag.ProductSortOrder = sortOrder == "product" ? "product_desc" : "product";
            ViewBag.DrawerSortOrder = sortOrder == "drawer" ? "drawer_desc" : "drawer";
            ViewBag.AssetSortOrder = sortOrder == "assettag" ? "assettag_desc" : "assettag";
            ViewBag.CategorySortOrder = sortOrder == "category" ? "category_desc" : "category";
            ViewBag.SerialSortOrder = sortOrder == "serial" ? "serial_desc" : "serial";
            ViewBag.AssignedUserSortOrder = sortOrder == "assigneduser" ? "assigneduser_desc" : "assigneduser";
            ViewBag.LocationSortOrder = sortOrder == "location" ? "location_desc" : "location";
            ViewBag.BuildingSortOrder = sortOrder == "building" ? "building_desc" : "building";
            ViewBag.RoomSortOrder = sortOrder == "room" ? "room_desc" : "room";
            ViewBag.LicenseTypeSortOrder = sortOrder == "licensetype" ? "licensetype_desc" : "licensetype";
            ViewBag.LicenseSortOrder = sortOrder == "license" ? "license_desc" : "license";
            ViewBag.AssignedAssetSortOrder = sortOrder == "assignedasset" ? "assignedasset_desc" : "assignedasset";
            ViewBag.StatusSortOrder = sortOrder == "status" ? "status_desc" : "status";
            ViewBag.SNFSortOrder = sortOrder == "snf" ? "snf_desc" : "snf";

            if (!string.IsNullOrEmpty(searchString)) //if its not empty then go to page 1, meaning new search
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
                ManuId = manuId,
                ProductId = productId,
                ShowConnectedAssets = showConnectedAssets,
                ActiveStatus = activeStatus ?? 100,
                AssetType = AssetType,
                AssetCategory = assetCategory,
                LocationId = locationId,
                BuildingId = buildingId,
                RoomId = roomId,
                SortOrder = sortOrder,
                SearchString = searchString,
                CurrentPage = page,
                PageSize = pSize ?? pageSize,
                LoginUserId = LoginUserId,
                LoginUserLocId = await GetUserLocId(),
                Roles = await GetRoles(),
                AssetListId = assetListId
            };

            var vmAssetList = await AssetLogic.GetAssetList(parameters);

            return View(vmAssetList);
        }

        [Authorize(Roles = "HardwareView, ManageParkAssets")]
        public async Task<ActionResult> ExportToExcel(int? manuId, int? productId, int? assetId, bool? showConnectedAssets, string assetCategory, int? locationId, int? buildingId, int? roomId, string sortOrder, string currentSearch, string searchString, int? invoiceItemId, int? assetListId, int? activeStatus)
        {
            var parameters = new Parameters
            {
                InvoiceItemId = invoiceItemId,
                AssetId = assetId,
                ManuId = manuId,
                ProductId = productId,
                ShowConnectedAssets = showConnectedAssets,
                ActiveStatus = activeStatus ?? 100,
                AssetType = AssetType,
                AssetCategory = assetCategory,
                LocationId = locationId,
                BuildingId = buildingId,
                RoomId = roomId,
                SortOrder = sortOrder,
                SearchString = string.IsNullOrEmpty(searchString) ? currentSearch : searchString,
                LoginUserId = LoginUserId,
                LoginUserLocId = await GetUserLocId(),
                Roles = await GetRoles(),
                Export = true,
                AssetListId = assetListId
            };

            var vmAssetList = await AssetLogic.GetAssetList(parameters);
            var itemList = vmAssetList.Select(x => x.Assets).FirstOrDefault();
            var list = itemList.Select(x => new { 
                x.Id, x.Name, 
                Make = x.ManufacturerName, 
                Model = x.ProductName, 
                x.AssetTag, 
                x.Serial,
                x.Description,
                x.Drawer,
                x.MacAddress, 
                x.IPAddress, 
                Category = x.InvoiceItem?.AssetCategory.Name, 
                Location = x.LocationCode, 
                Building = x.Building.DisplayName, 
                Room = x.Room.DisplayName, 
                AssignedUser = x.AssignedUser.Name, 
                ConnectedAssets = (x.ConnectedAssetId != null) ? x.ConnectedAsset.Serial : x.ConnectedAssetSerials,
                PONumber = x.InvoiceItem?.Invoice?.PONumber, 
                Status = x.AssetStatusName, 
                SurplusDate = x.SurplusDate,
                SurplusNumber = x.SNFnumber,
                LastLoginDate = x.LastLoginDate,
                LastBootUpDate = x.LastBootDate,
                ConfirmedDate = x.AssetConfirmedDate,
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
            var FileName = "Hardware_" + DateTime.Now.ToString("MM/dd/yyyy") + ".xls";
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
        [Authorize(Roles = "HardwareEdit, ManageParkAssets")]
        public async Task<JsonResult> BulkEdit(string assets)
        {
            var LoginUserLocId = await GetUserLocId();
            var Roles = await GetRoles();
            bool isAdmin = false;

            if (Roles.Contains("HardwareEdit") && (AssetType == "Hardware"))
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
        [Authorize(Roles = "HardwareEdit, ManageParkAssets")]
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
        [Authorize(Roles = "HardwareView, ManageParkAssets")]
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
        [Authorize(Roles = "HardwareEdit")]
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
            vm.ComputerTypeAssets = vm.Assets?.Append(vm.ConnectedAsset).Where(x => x.Id > 0).DistinctBy(x => x.Id).OrderBy(x => x.AssetNameOrSerial);


            return View(vm);
        }

        //Update (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HardwareEdit")]
        public async Task<ActionResult> Edit(AssetVM vm)
        {
            var status = await AssetLogic.GetAssetStatus(vm.StatusId ?? 0);

            ModelState.Remove("ExpirationDate");
            vm.ExpirationDate = vm.ExpirationDate;

            //Asset Name
            ModelState.Remove("Name");
            if (ComputerTypeList.Contains(vm.AssetCategoryName))
            {
                vm.Name = vm.Name.RemoveSpaces()?.ToUpper();
            }
            else
            {
                vm.Name = vm.Name.RemoveSpaces();
            }

            bool nameExist = false;
            if (!string.IsNullOrEmpty(vm.Name) && vm.Name.RemoveSpaces()?.ToUpper() != vm.OriginalName.RemoveSpaces()?.ToUpper())
            {
                nameExist = await CommonLogic.NameExist(vm.Name, AssetType);
            }
            
            //Serial Check
            bool serialExist = false;
            ModelState.Remove("Serial");
            vm.Serial = vm.Serial.RemoveSpaces()?.ToUpper();
            if (!string.IsNullOrEmpty(vm.Serial) && vm.Serial != vm.OriginalSerial.RemoveSpaces()?.ToUpper())
            {
                serialExist = await AssetLogic.CheckSerial(vm.Serial, vm.ManuId ?? 0);
            }

            //AssetTag Check
            bool assetTagExist = false;
            ModelState.Remove("AssetTag");
            vm.AssetTag = vm.AssetTag.RemoveSpaces();
            if (!string.IsNullOrEmpty(vm.AssetTag) && vm.AssetTag != vm.OriginalAssetTag)
            {
                assetTagExist = await AssetLogic.CheckAssetTag(vm.AssetTag);
            }

            //MacAddress Check
            bool macAddressExist = false;
            ModelState.Remove("MacAddress");
            vm.MacAddress = vm.MacAddress.RemoveSpaces()?.ToUpper();
            if (!string.IsNullOrEmpty(vm.MacAddress) && vm.MacAddress != vm.OriginalMacAddress.RemoveSpaces()?.ToUpper())
            {
                macAddressExist = await AssetLogic.CheckMacAddress(vm.MacAddress);
            }

            //Drawer Check
            bool drawerExist = false;
            ModelState.Remove("Drawer");
            vm.Drawer = vm.Drawer.RemoveSpaces();
            if (!string.IsNullOrEmpty(vm.Drawer) && vm.Drawer != vm.OriginalDrawer.RemoveSpaces())
            {
                drawerExist = await AssetLogic.CheckDrawer(vm.Drawer);
            }
            ////

            //IPAddress Check
            bool ipaddressExist = false;
            ModelState.Remove("IPAddress");
            vm.IPAddress = vm.IPAddress.RemoveSpaces();
            if (!string.IsNullOrEmpty(vm.IPAddress) && vm.IPAddress != vm.OriginalIPAddress.RemoveSpaces())
            {
                ipaddressExist = await AssetLogic.CheckIPAddress(vm.IPAddress);
            }
            ////

            if (nameExist)
            {
                ModelState.AddModelError(nameof(vm.Name), "The Name already exist in db");
            }
            else if (drawerExist)
            {
                ModelState.AddModelError(nameof(vm.Drawer), "The Drawer Number already exist in db");
            }
            else if (string.IsNullOrEmpty(vm.AssetTag))
            {
                ModelState.AddModelError(nameof(vm.AssetTag), "The Asset Tag is required");
            }
            else if (vm.AssetTag.Length < 4 || vm.AssetTag.Length == 5 || vm.AssetTag.Length > 6)
            {
                ModelState.AddModelError(nameof(vm.AssetTag), "Asset Tag must be 4 or 6 digits");
            }
            else if (assetTagExist)
            {
                ModelState.AddModelError(nameof(vm.AssetTag), "Asset Tag already exist in db");
            }
            else if (!string.IsNullOrEmpty(vm.Serial) && (vm.Serial.Length < 5 || vm.Serial.Length > 30))
            {
                ModelState.AddModelError(nameof(vm.Serial), "Serial Number must be greater than 4");
            }
            else if (!string.IsNullOrEmpty(vm.Serial) && vm.Serial.InValidItem(RegexSerial))
            {
                ModelState.AddModelError(nameof(vm.Serial), "Serial Number is not in the correct format");
            }
            else if (serialExist)
            {
                ModelState.AddModelError(nameof(vm.Serial), "Serial Number already exist in db");
            }
            else if (!string.IsNullOrEmpty(vm.MacAddress) && vm.MacAddress.InValidItem(RegexMacAddress))
            {
                ModelState.AddModelError(nameof(vm.MacAddress), "MAC Address format: xx:yy:zz:11:22:33");
            }
            else if (ipaddressExist)
            {
                ModelState.AddModelError(nameof(vm.IPAddress), "The IP Address already exist in db");
            }
            else if (macAddressExist)
            {
                ModelState.AddModelError(nameof(vm.MacAddress), "Mac Address already exist in db");
            }
            else if (vm.SurplusDate == null && status.Name == "Surplus")
            {
                ModelState.AddModelError(nameof(vm.SNFnumber), "The Surplus Date is required");
            }
            else if (vm.SNFnumber == null && status.Name == "Surplus")
            {
                ModelState.AddModelError(nameof(vm.SNFnumber), "The Surplus Number is required");
            }
            else if (vm.LocationId == null && status.Group == "Active")
            {
                ModelState.AddModelError(nameof(vm.LocationId), "The Location is required");
            }
            else if ((vm.LocationId != vm.OriginalLocationId) && (await CommonLogic.IsDeleted(vm.LocationId, "Location")))
            {
                vm.LocationId = null;
                ModelState.AddModelError(nameof(vm.LocationId), "The Location was recently deleted");
            }
            else if ((vm.BuildingId != vm.OriginalBuildingId) && (await CommonLogic.IsDeleted(vm.BuildingId, "Building")))
            {
                vm.BuildingId = null;
                ModelState.AddModelError(nameof(vm.BuildingId), "The Building was recently deleted");
            }
            else if ((vm.RoomId != vm.OriginalRoomId) && (await CommonLogic.IsDeleted(vm.RoomId, "Room")))
            {
                vm.RoomId = null;
                ModelState.AddModelError(nameof(vm.RoomId), "The Room was recently deleted");
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
                vm.Statuses = await AssetLogic.GetAssetStatuses(AssetType, null);
                vm.Locations = await LocationLogic.GetLocations();
                vm.Buildings = await BuildingLogic.GetBuildings(vm.LocationId ?? 0);
                vm.Rooms = await RoomLogic.GetRooms(vm.BuildingId ?? 0);
                vm.Users = await UserLogic.GetUsers();
                vm.Assets = await AssetLogic.GetComputerTypeAssets(null);

                vm.Locations = vm.Locations?.Append(vm.Location)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);
                vm.Buildings = vm.Buildings?.Append(vm.Building)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);
                vm.Rooms = vm.Rooms?.Append(vm.Room)?.Where(x => x.Id > 0).DistinctBy(x => x.Id);
                vm.Users = vm.Users?.Append(vm.AssignedUser)?.Where(x => x.Id > 0).DistinctBy(x => x.Id).OrderBy(x => x.NameWithUserName);
                vm.ComputerTypeAssets = vm.Assets?.Append(vm.ConnectedAsset).Where(x => x.Id > 0).DistinctBy(x => x.Id).OrderBy(x => x.AssetNameOrSerial);

                return View(vm);
            }
        }

        // Delete (GET)
        [HttpGet]
        [Authorize(Roles = "HardwareDelete")]
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
        [Authorize(Roles = "HardwareDelete")]
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