using Microsoft.AspNet.Identity;
using Inventory.Web.Logic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Inventory.Web.Common
{
    public class AssetBaseMvcController : Controller
    {
        private ClaimsPrincipal prinicpal;
        protected ICommonLogic CommonLogic { get; set; }
        protected IInvoiceItemLogic InvoiceItemLogic { get; set; }
        protected IInvoiceLogic InvoiceLogic { get; set; }
        protected IAssetLogic AssetLogic { get; set; }
        protected IAssetListLogic AssetListLogic { get; set; }
        protected IUserAssetLogic UserAssetLogic { get; set; }
        protected ILocationLogic LocationLogic { get; set; }
        protected IBuildingLogic BuildingLogic { get; set; }
        protected IRoomLogic RoomLogic { get; set; }
        protected IUserLogic UserLogic { get; set; }
        protected IManuLogic ManuLogic { get; set; }
        protected IProductLogic ProductLogic { get; set; }
        protected ISupplierLogic SupplierLogic { get; set; }
        protected string AssetType { get; set; }
        protected string ReferrerController { get; set; }
        protected string AdApplicationName { get { return ConfigurationManager.AppSettings["ADApplicationName"].ToString(); } }
        protected int LoginUserId { get { return Convert.ToInt32(User.Identity.GetUserId()); } }
        protected List<string> ComputerTypeList = new List<string>() { "Desktop", "Laptop", "Server" };

        protected int pageSize = 50;
        protected readonly Regex RegexSerial = new Regex("^[a-zA-Z\\d-]+$"); //alpha numberic, including hypen
        protected readonly Regex RegexLicense = new Regex("^[a-zA-Z\\d-!@#$%^&*()-+=]+$");
        protected readonly Regex RegexMacAddress = new Regex("^([0-9a-fA-F]{2}(?:[:-]?[0-9a-fA-F]{2}){5})$");
        protected readonly Regex RegexLongitude = new Regex("^-?([1]?[1-7][1-9]|[1]?[1-8][0]|[1-9]?[0-9])\\.{1}\\d{1,6}");
        protected readonly Regex RegexLatitude = new Regex("^-?([1-8]?[1-9]|[1-9]0)\\.{1}\\d{1,6}");
        protected readonly Regex RegexDrawer = new Regex("^[0-9]{1,4}$");
        protected readonly Regex RegexIPAddress = new Regex("^(([1-9]?\\d|1\\d\\d|2[0-5][0-5]|2[0-4]\\d)\\.){3}([1-9]?\\d|1\\d\\d|2[0-5][0-5]|2[0-4]\\d)$");


        protected async Task<List<string>> GetRoles()
        {
            prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return await Task.FromResult(prinicpal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList());
        }

        protected async Task<int> GetUserLocId()
        {
            prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return await Task.FromResult(Convert.ToInt32(prinicpal.Claims.Where(c => c.Type == CustomClaimTypes.LocationId).Select(c => c.Value).SingleOrDefault()));
        }

        protected async Task<string> GetReferrerControlerNameAsync()
        {
            try
            {
                var fullUrl = this.Request.UrlReferrer?.ToString();
                string url = fullUrl;

                var request = new HttpRequest(null, url, null);
                var response = new HttpResponse(new StringWriter());
                var httpContext = new HttpContext(request, response);

                var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

                var values = routeData.Values;
                string controllerName = values["controller"]?.ToString();

                return await Task.FromResult(controllerName);
            }
            catch
            {
                return null;
            }
        }

    }


    public struct Parameters
    {
        public int? InvoiceItemId;
        public int? AssetId;
        public string AssetType;
        public string AssetCategory;
        public int? LocationId;
        public int? BuildingId;
        public int? RoomId;
        public int? ManuId;
        public int? ProductId;
        public bool? ShowConnectedAssets;
        public string LicenseType;
        public string SortOrder;
        public int ActiveStatus;
        public string SearchString;
        public int CurrentPage;
        public int PageSize;
        public int LoginUserId;
        public int? LoginUserLocId;
        public int? ManagedUserId;
        public bool? Export;
        public List<string> Roles;
        public int? AssetListId;
    }

}
