using Inventory.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Web.Logic
{
    public interface IManuLogic
    {
        Task<IEnumerable<ManufacturerVM>> GetList(string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000);

        Task<IEnumerable<ManufacturerVM>> GetManufacturers(int? assetTypeId, int? assetCategoryId);

        Task<int> Create(ManufacturerVM vm);

        Task<ManufacturerVM> Get(int Id);

        Task Save(ManufacturerVM vm);

        Task Delete(ManufacturerVM vm);
    }
}