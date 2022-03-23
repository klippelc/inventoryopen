using Inventory.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Web.Logic
{
    public interface IBuildingLogic
    {
        Task<IEnumerable<BuildingVM>> GetList(int? locationId, string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000);

        Task<IEnumerable<BuildingVM>> GetBuildings(int? locationId = null);

        Task<int> Create(BuildingVM vm);

        Task<BuildingVM> Get(int Id);

        Task Save(BuildingVM vm);

        Task Delete(BuildingVM vm);

        Task<bool> CodeExist(string code);

        Task<bool> PropertyIdExist(string propertyId, int? locationId = 0);

        Task UpdateAssetLocation(int? locationId, int? buildingId, int? userId);
    }
}