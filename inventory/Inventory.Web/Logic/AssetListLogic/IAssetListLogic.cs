using Inventory.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Web.Logic
{
    public interface IAssetListLogic
    {
        Task<IEnumerable<AssetListVM>> GetList(int userId, string sortOrder = "", string currentFilter = "", string searchString = "", int page = 1, int pageSize = 10000000);

        Task<AssetListVM> GetBlankAssetList();

        Task<int> Create(AssetListVM vm);

        Task<AssetListVM> Get(int Id, int userId = 0);

        Task Save(AssetListVM vm);

        Task Delete(AssetListVM vm);

        Task<IEnumerable<AssetListVM>> GetUserLists(int userId, string assetType, bool showShared = false);

        Task<IEnumerable<AssetListVM>> GetUserList(int listId, int userId = 0);

        Task<IEnumerable<AssetUserListVM>> GetAssetUserList(int listId);

        Task<bool> AddAssetsToUserList(AssetListVM vm);

        Task<bool> RemoveAssetsFromUserList(AssetListVM vm);

        Task<bool> RemoveAssetsFromUserList(int listId, int userId = 0);

        Task<bool> RemoveAssetFromAllList(int assetId);

    }
}
