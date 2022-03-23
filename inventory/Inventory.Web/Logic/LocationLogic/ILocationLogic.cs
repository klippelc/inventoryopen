using Inventory.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Web.Logic
{
    public interface ILocationLogic
    {
        Task<IEnumerable<LocationVM>> GetList(string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000);

        Task<IEnumerable<LocationVM>> GetLocations();

        Task<int> Create(LocationVM vm);

        Task<LocationVM> Get(int Id);

        Task Save(LocationVM vm);

        Task Delete(LocationVM vm);

        Task<IEnumerable<AmenityVM>> GetLocationAmenities(IEnumerable<int> SelectedIds = null);

        Task<bool> CodeExist(string code);

        Task<bool> PropertyIdExist(string propertyId);

        Task<bool> SubnetExist(string subnet);

        Task<string> AliasExist(int? locId, string alias);

        Task ManageAliases(int locationId, string originalAliases, string currentAliases);
    }
}