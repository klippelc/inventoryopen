using Inventory.Web.Common;
using Inventory.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Web.Logic
{ 
    public interface IUserAssetLogic
    {
        Task<IEnumerable<PagedListVM>> GetUserAssetList(Parameters parameters);

        Task<AssetVM> GetUserAsset(int Id, int loginUserId, string assetType);

        Task<bool> ConfirmAsset(UserAssetConfirmationVM vm, int loginUserId);
    }
}
