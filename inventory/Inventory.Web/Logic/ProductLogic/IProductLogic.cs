using Inventory.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Web.Logic
{
    public interface IProductLogic
    {
        Task<IEnumerable<ProductVM>> GetList(int? manuId, string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000);

        Task<IEnumerable<ProductVM>> GetProducts(int? manuId, int? assetTypeId, int? assetCategoryId);

        Task<int> Create(ProductVM vm);

        Task<ProductVM> Get(int Id);

        Task Save(ProductVM vm);

        Task Delete(ProductVM vm);
    }
}