using Inventory.Data.Models;
using Inventory.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Web.Logic
{
    public interface ICommonLogic
    {
        Task<bool> NameExist(string name, string itemType, int manuId = 0, int LocationId = 0, int buildingId = 0, int userId = 0);

        Task<IEnumerable<AssetTypeVM>> GetAssetTypes();

        Task<bool> IsDeleted(int? Id, string itemType);

        Task<IEnumerable<CityVM>> GetCities(string stateCode);

        Task<IEnumerable<StateVM>> GetStates(string stateCode);
    }
}