using Inventory.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Web.Logic
{
    public interface IUserLogic
    {
        Task<IEnumerable<UserVM>> GetList(string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000);

        Task<IEnumerable<UserVM>> GetUsers();

        Task<UserVM> GetUserRoles(string userName);

        Task<UserVM> Get(int Id);

        Task Save(UserVM vm);

        Task UpdateLastLogin(int userId);

        Task<IEnumerable<RoleVM>> GetRoles(IEnumerable<int> SelectedIds = null);

        Task<LocationVM> GetUserDefaultLocation(int userId);

        Task<IEnumerable<UserVM>> GetUsersByLocation(int locationId);
    }
}