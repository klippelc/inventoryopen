using Inventory.Web.Logic;
using System.Web.Http;

namespace Inventory.Web.Common
{
    public class AssetBaseApiController : ApiController
    {
        protected IInvoiceItemLogic InvoiceItemLogic { get; set; }
        protected IInvoiceLogic InvoiceLogic { get; set; }
        protected IAssetLogic AssetLogic { get; set; }
        protected ILocationLogic LocationLogic { get; set; }
        protected IBuildingLogic BuildingLogic { get; set; }
        protected IRoomLogic RoomLogic { get; set; }
        protected IUserLogic UserLogic { get; set; }
        protected IManuLogic ManuLogic { get; set; }
        protected IProductLogic ProductLogic { get; set; }
        protected ISupplierLogic SupplierLogic { get; set; }
    }
}