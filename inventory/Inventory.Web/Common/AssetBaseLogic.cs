using Inventory.Data.Services;
using Inventory.Web.Logic;
using System.Collections.Generic;

namespace Inventory.Web.Common
{
    public class AssetBaseLogic
    {
        protected InventoryDbContext Db { get; set; }
        protected ICommonLogic CommonLogic { get; set; }
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
        protected IAssetListLogic AssetListLogic { get; set; }

        protected List<string> ComputerTypeList = new List<string>() { "Desktop", "Laptop", "Server" };

    }
}