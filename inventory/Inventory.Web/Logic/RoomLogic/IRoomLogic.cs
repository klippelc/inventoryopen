using Inventory.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Web.Logic
{
    public interface IRoomLogic
    {
        Task<IEnumerable<RoomVM>> GetList(int? locationId, int? buildingId, string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000);

        Task<IEnumerable<RoomVM>> GetRooms(int? buildingId);

        Task<int> Create(RoomVM vm);

        Task<RoomVM> Get(int Id);

        Task Save(RoomVM vm);

        Task Delete(RoomVM vm);

        Task<IEnumerable<RoomTypeVM>> GetRoomTypes();

        Task<IEnumerable<AmenityVM>> GetRoomAmenities(IEnumerable<int> SelectedIds = null);

        Task<bool> PropertyIdExist(string propertyId, int? buildingId = 0);
    }
}