using Inventory.Web.Common;
using Inventory.Web.Logic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Inventory.Web.API
{
    [RoutePrefix("api")]
    public class InventoryController : AssetBaseApiController
    {
        public InventoryController(IAssetLogic assetLogic)
        {
            AssetLogic = assetLogic;
        }

        [Route("Assets")]
        public async Task<IHttpActionResult> GetAll()
        {
            var vmList = await AssetLogic.GetAssets(null, "active");
            return Json(vmList);
        }

        [Route("Asset/{Id:int}")]
        public async Task<IHttpActionResult> Get(int? Id)
        {
            var vm = await AssetLogic.GetAsset(Id ?? 0, "Hardware");

            return Json(vm);
        }
    }
}