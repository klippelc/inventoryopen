using Inventory.Web.Common;
using System.Threading.Tasks;
using System.Web.Mvc;
using Inventory.Web.Logic;

namespace Inventory.Web.Controllers
{
    public class HomeController : AssetBaseMvcController
    {
        public HomeController(IInvoiceLogic invoiceLogic, IAssetLogic assetLogic)
        {
            InvoiceLogic = invoiceLogic;
            AssetLogic = assetLogic;
            AssetType = "Home";
            ViewBag.AssetType = AssetType;
        }


        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            var obj = await AssetLogic.GetAssetCounts();
            return View(obj);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "";
            return View();
        }


        [AllowAnonymous]
        public ActionResult NotFound(int Id)
        {
            ViewBag.Message = "Page Not Found.";
            ViewBag.ErrorType = "Error (" + Id.ToString() + ")";
            return View();
        }
    }
}