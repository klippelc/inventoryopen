using Inventory.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Web.Logic
{
    public interface ISupplierLogic
    {
        Task<IEnumerable<SupplierVM>> GetList(string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000);

        Task<IEnumerable<SupplierVM>> GetSuppliers();

        Task<int> Create(SupplierVM vm);

        Task<SupplierVM> Get(int Id);

        Task Save(SupplierVM vm);

        Task Delete(SupplierVM vm);
    }
}