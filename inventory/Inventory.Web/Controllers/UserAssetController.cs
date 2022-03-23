using Inventory.Web.Common;
using Inventory.Web.Logic;
using System;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Inventory.Web.ViewModels;

namespace Inventory.Web.Controllers
{
    public class UserAssetController : AssetBaseMvcController
    {
        //Constructor
        public UserAssetController(IUserAssetLogic userAssetLogic, IAssetLogic assetLogic, ILocationLogic locationLogic,
            IBuildingLogic buildingLogic, IRoomLogic roomLogic, IUserLogic userLogic, ICommonLogic commonLogic)
        {
            UserAssetLogic = userAssetLogic;
            AssetLogic = assetLogic;
            LocationLogic = locationLogic;
            BuildingLogic = buildingLogic;
            RoomLogic = roomLogic;
            UserLogic = userLogic;
            CommonLogic = commonLogic;
            ViewBag.PageName = "My Assets";
        }

        //Get All
        [Authorize(Roles = "UserAssetView")]
        public async Task<ActionResult> Index(int? muId, int? manuId, int? productId, int? assetId, bool? showConnectedAssets, string assetCategory, string sortOrder, int? activeStatus, string currentSearch, string searchString, int? invoiceItemId, int page = 1)
        {
            ModelState.Remove("SearchString");
            searchString = searchString?.Trim();


            ViewBag.AssetId = assetId;
            ViewBag.ShowConnectedAssets = showConnectedAssets;
            ViewBag.AssetCategory = assetCategory;
            ViewBag.InvoiceItemId = invoiceItemId;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ActiveStatus = activeStatus;
            ViewBag.Manufacturer = manuId;
            ViewBag.Product = productId;
            ViewBag.ManagedUserId = muId;
            ViewBag.ConnectedAssetSortOrder = sortOrder == "connected" ? "connected" : "connected";
            ViewBag.NameSortOrder = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.InvoiceSortOrder = sortOrder == "invoice" ? "invoice_desc" : "invoice";
            ViewBag.ManuSortOrder = sortOrder == "manufacturer" ? "manufacturer_desc" : "manufacturer";
            ViewBag.ProductSortOrder = sortOrder == "product" ? "product_desc" : "product";
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

            if (!string.IsNullOrEmpty(searchString)) 
            {
                page = 1;
            }
            else
            {
                searchString = currentSearch;
            }

            var parameters = new Parameters
            {
                InvoiceItemId = invoiceItemId,
                AssetId = assetId,
                ManuId = manuId,
                ProductId = productId,
                ShowConnectedAssets = showConnectedAssets,
                ActiveStatus = activeStatus ?? 3,
                AssetType = "Hardware",
                AssetCategory = assetCategory,
                SortOrder = sortOrder,
                SearchString = searchString,
                CurrentPage = page,
                PageSize = pageSize,
                LoginUserId = LoginUserId,
                ManagedUserId = muId
            };

            

            ViewBag.CurrentSearch = searchString;

            var vmAssetList = await UserAssetLogic.GetUserAssetList(parameters);

            return View(vmAssetList);
        }

        [Authorize(Roles = "UserAssetView")]
        public async Task<ActionResult> ExportToExcel(int? muId, int? manuId, int? productId, int? assetId, bool? showConnectedAssets, string assetCategory, string sortOrder, int? activeStatus, string currentSearch, string searchString, int? invoiceItemId, int page = 1)
        {
            var parameters = new Parameters
            {
                InvoiceItemId = invoiceItemId,
                AssetId = assetId,
                ManuId = manuId,
                ProductId = productId,
                ShowConnectedAssets = showConnectedAssets,
                ActiveStatus = activeStatus ?? 3,
                AssetType = "Hardware",
                AssetCategory = assetCategory,
                SortOrder = sortOrder,
                SearchString = string.IsNullOrEmpty(searchString) ? currentSearch : searchString,
                CurrentPage = page,
                PageSize = pageSize,
                LoginUserId = LoginUserId,
                ManagedUserId = muId,
                Export = true
            };

            var vmAssetList = await UserAssetLogic.GetUserAssetList(parameters);
            var itemList = vmAssetList.Select(x => x.Assets).FirstOrDefault();
            var list = itemList.Select(x => new {
                x.Id,
                x.Name,
                Make = x.ManufacturerName,
                Model = x.ProductName,
                x.Description,
                x.Drawer,
                x.AssetTag,
                x.Serial,
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
            var FileName = "MyAssets_" + DateTime.Now.ToString("MM/dd/yyyy") + ".xls";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            //

            return View("Export");
        }

        //Details
        [Authorize(Roles = "UserAssetView")]
        public async Task<ActionResult> Details(int? Id)
        {
            var vm = await UserAssetLogic.GetUserAsset(Id ?? 0, LoginUserId, "Hardware");

            if (vm == null)
            {
                return View("NotFound");
            }

            if (Request.UrlReferrer != null)
            {
                vm.PreviousUrl = Request.UrlReferrer?.ToString();
            }

            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "UserAssetView")]
        public async Task<JsonResult> ConfirmAsset(int? Id)
        {
            var vm = await UserAssetLogic.GetUserAsset(Id ?? 0, LoginUserId, "Hardware");

            var assetInfo = vm.AssetCategoryName + " - " + vm.ManufacturerName + " / " + vm.ProductName;

            if (!string.IsNullOrEmpty(vm.AssetTag))
            {
                assetInfo = assetInfo + " - (AssetTag: " + vm.AssetTag + ")";
            }
            else
            {
                assetInfo = assetInfo + " - (Serial: " + (!string.IsNullOrEmpty(vm.Serial) ? vm.Serial : "n/a") + ")";
            }

            if ((vm.AssetConfirmedDate != null) && (!(vm.HasItBeen1Year ?? true)))
            {
                return Json(new { IsCreated = false, Message = "Has not been more than a year yet" }, JsonRequestBehavior.AllowGet);
            }

            if (vm.AssignedUserId != LoginUserId)
            {
                return Json(new { IsCreated = false, Message = "Assigned User not Matching Login" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { AssetInfo = assetInfo, AssetId = vm.Id, UserId = LoginUserId, IsCreated = true, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        //Confirm
        [HttpPost]
        [Authorize(Roles = "UserAssetView")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ConfirmAsset(UserAssetConfirmationVM vm)
        {
            if (vm.UserId != LoginUserId)
            {
                return Json(new { Confirmed = false, Message = "Assigned User not Matching Login" }, JsonRequestBehavior.AllowGet);
            }
            var result = await UserAssetLogic.ConfirmAsset(vm, LoginUserId);

            return Json(new { Confirmed = result, Message = "" }, JsonRequestBehavior.AllowGet);

        }

    }
}
