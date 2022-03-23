using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PagedList;
using PagedList.EntityFramework;
using Inventory.Data.Models;
using Inventory.Data.Services;
using Inventory.Web.Common;
using Inventory.Web.ViewModels;

namespace Inventory.Web.Logic
{
    public class RoomLogic : AssetBaseLogic, IRoomLogic
    {
        //Constructor
        public RoomLogic(InventoryDbContext db)
        {
            Db = db;
        }

        //Get List
        public async Task<IEnumerable<RoomVM>> GetList(int? locationId, int? buildingId, string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000)
        {
            int pageNumber = page;
            var vmList = new List<RoomVM>();

            //
            var query = Db.Rooms
                          .Include(r => r.RoomType)
                          .Include(x => x.RoomAmenities.Select(r => r.Amenity))
                          .Include(x => x.Building.Location)
                          .Where(x => x.IsDeleted == false);
            //
            if (locationId != null)
            {
                query = query.Where(x => x.Building.LocationId == locationId).AsQueryable();
            }

            if (buildingId != null)
            {
                query = query.Where(x => x.BuildingId == buildingId).AsQueryable();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => (string.IsNullOrEmpty(x.DisplayName) ? x.Name.Contains(searchString) : x.DisplayName.Contains(searchString))
                                      || (string.IsNullOrEmpty(x.Building.Location.Code) ? x.Building.Location.Name.Contains(searchString) : x.Building.Location.Code.Contains(searchString))
                                      || ((x.Building != null) 
                                          && (string.IsNullOrEmpty(x.Building.DisplayName) ? x.Building.Name.Contains(searchString) : x.Building.DisplayName.Contains(searchString)))
                                      || ((x.Building.Location != null) && (x.Building.Location.Code.Contains(searchString)))
                                      || ((x.Building.Location != null) 
                                          && (string.IsNullOrEmpty(x.Building.Location.DisplayName) ? x.Building.Location.Name.Contains(searchString) : x.Building.Location.DisplayName.Contains(searchString))));
            }

            switch (sortOrder)
            {
                case "id":
                    query = query.OrderBy(s => !string.IsNullOrEmpty(s.PropertyId) ? s.PropertyId : "9999");
                    break;

                case "id_desc":
                    query = query.OrderByDescending(s => !string.IsNullOrEmpty(s.PropertyId) ? s.PropertyId : "0000");
                    break;

                case "room":
                    query = query.OrderBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz")
                                 .ThenBy(s => s.Building != null ? s.Building.DisplayName != null && s.Building.DisplayName != "" ? s.Building.DisplayName : s.Building.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.Building != null && s.Building.Location != null ? s.Building.Location.DisplayName != null && s.Building.Location.DisplayName != "" ? s.Building.Location.DisplayName : s.Building.Location.Name ?? "zzz" : "zzz");
                    break;

                case "room_desc":
                    query = query.OrderByDescending(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "")
                                 .ThenBy(s => s.Building != null ? s.Building.DisplayName != null && s.Building.DisplayName != "" ? s.Building.DisplayName : s.Building.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.Building != null && s.Building.Location != null ? s.Building.Location.DisplayName != null && s.Building.Location.DisplayName != "" ? s.Building.Location.DisplayName : s.Building.Location.Name ?? "zzz" : "zzz");
                    break;


                case "roomtype":
                    query = query.OrderBy(s => s.RoomType != null && s.RoomType.Name != "" ? s.RoomType.Name : "zzz")
                                 .ThenBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz")
                                 .ThenBy(s => s.Building != null ? s.Building.DisplayName != null && s.Building.DisplayName != "" ? s.Building.DisplayName : s.Building.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.Building != null && s.Building.Location != null ? s.Building.Location.DisplayName != null && s.Building.Location.DisplayName != "" ? s.Building.Location.DisplayName : s.Building.Location.Name ?? "zzz" : "zzz");
                    break;

                case "roomtype_desc":
                    query = query.OrderByDescending(s => s.RoomType != null && s.RoomType.Name != "" ? s.RoomType.Name : "zzz")
                                 .ThenBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz")
                                 .ThenBy(s => s.Building != null ? s.Building.DisplayName != null && s.Building.DisplayName != "" ? s.Building.DisplayName : s.Building.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.Building != null && s.Building.Location != null ? s.Building.Location.DisplayName != null && s.Building.Location.DisplayName != "" ? s.Building.Location.DisplayName : s.Building.Location.Name ?? "zzz" : "zzz");
                    break;

                case "building":
                    query = query.OrderBy(s => s.Building != null ? s.Building.DisplayName != null && s.Building.DisplayName != "" ? s.Building.DisplayName : s.Building.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz");
                    break;

                case "building_desc":
                    query = query.OrderByDescending(s => s.Building != null ? s.Building.DisplayName != null && s.Building.DisplayName != "" ? s.Building.DisplayName : s.Building.Name ?? "" : "")
                                 .ThenBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz");
                    break;

                case "location":
                    query = query.OrderBy(s => s.Building != null && s.Building.Location != null ? s.Building.Location.DisplayName != null && s.Building.Location.DisplayName != "" ? s.Building.Location.DisplayName : s.Building.Location.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.Building != null ? s.Building.DisplayName != null && s.Building.DisplayName != "" ? s.Building.DisplayName : s.Building.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz");
                    break;

                case "location_desc":
                    query = query.OrderByDescending(s => s.Building != null && s.Building.Location != null ? s.Building.Location.DisplayName != null && s.Building.Location.DisplayName != "" ? s.Building.Location.DisplayName : s.Building.Location.Name ?? "" : "")
                                 .ThenBy(s => s.Building != null ? s.Building.DisplayName != null && s.Building.DisplayName != "" ? s.Building.DisplayName : s.Building.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz");
                    break;

                case "locationcode":
                    query = query.OrderBy(s => s.Building != null && s.Building.Location != null ? s.Building.Location.Code != null && s.Building.Location.Code != "" ? s.Building.Location.Code : "zzz" : "zzz")
                                 .ThenBy(s => s.Building != null ? s.Building.DisplayName != null && s.Building.DisplayName != "" ? s.Building.DisplayName : s.Building.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz");
                    break;

                case "locationcode_desc":
                    query = query.OrderByDescending(s => s.Building != null && s.Building.Location != null ? s.Building.Location.Code != null && s.Building.Location.Code != "" ? s.Building.Location.DisplayName : "" : "")
                                 .ThenBy(s => s.Building != null ? s.Building.DisplayName != null && s.Building.DisplayName != "" ? s.Building.DisplayName : s.Building.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz");
                    break;

                default:
                    query = query.OrderBy(s => s.Building != null && s.Building.Location != null ? s.Building.Location.Code != null && s.Building.Location.Code != "" ? s.Building.Location.Code : "zzz" : "zzz")
                                 .ThenBy(s => s.Building != null ? s.Building.DisplayName != null && s.Building.DisplayName != "" ? s.Building.DisplayName : s.Building.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz");
                    break;
            }

            var List = await query.ToPagedListAsync(page, pageSize);

            var items = (IPagedList)List;

            foreach (var item in List)
            {
                var vm = new RoomVM()
                {
                    Id = item.Id,
                    PropertyId = item.PropertyId,
                    Name = item.Name,
                    DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,
                    Description = item.Description,
                    Capacity = item.Capacity,

                    RoomTypeId = item.RoomTypeId,
                    RoomType = item.RoomType != null ? new RoomTypeVM()
                    {
                        Id = item.RoomType.Id,
                        Name = item.RoomType.Name,
                    } : new RoomTypeVM(),

                    RoomAmenities = item.RoomAmenities != null ? item.RoomAmenities.Where(x => x.IsDeleted == false
                                          && x.Amenity != null
                                          && x.RoomId == item.Id)
                    .Select(p => new AmenityVM
                    {
                        Id = p.Amenity.Id,
                        Name = p.Amenity.Name,
                        TypeId = p.Amenity.TypeId,
                        Sequence = p.Amenity.Sequence,
                        IsChecked = true,
                        Checked = "Checked",
                    }).OrderBy(x => x.Sequence).ToList() : new List<AmenityVM>(),

                    Longitude = item.Longitude,
                    Latitude = item.Latitude,

                    BuildingId = item.BuildingId,
                    Building = item.Building != null ? new BuildingVM()
                    {
                        Id = item.Building.Id,
                        Name = item.Building.Name,
                        DisplayName = !string.IsNullOrEmpty(item.Building.DisplayName) ? item.Building.DisplayName : item.Building.Name,
                        Active = item.Building.Active,
                        LocationId = item.Building.LocationId,
                        Location = item.Building.Location != null ? new LocationVM()
                        {
                            Id = item.Building.Location.Id,
                            Code = item.Building.Location.Code,
                            Name = item.Building.Location.Name,
                            DisplayName = !string.IsNullOrEmpty(item.Building.Location.DisplayName) ? item.Building.Location.DisplayName : item.Building.Location.Name,
                            Active = item.Building.Location.Active
                        } : new LocationVM(),
                    } : new BuildingVM(),


                    Active = item.Active,
                    ActiveStatus = item.Active == true ? "Active" : "InActive",
                    Notes = item.Notes
                };
                vm.Location = vm.Building?.Location;
                vm.LocationId = vm.Building?.LocationId;
                vm.LocationName = vm.Building?.Location.Name;
                vmList.Add(vm);
            };

            StaticPagedList<RoomVM> list = new StaticPagedList<RoomVM>(vmList, items.PageNumber, items.PageSize, items.TotalItemCount);

            return list;
        }

        //Get Rooms
        public async Task<IEnumerable<RoomVM>> GetRooms(int? buildingId)
        {
            var vmRoomList = new List<RoomVM>();

            var query = Db.Rooms
                          .Include(r => r.RoomType)
                          .Include(x => x.RoomAmenities.Select(r => r.Amenity))
                          .Where(x => x.IsDeleted == false && x.Active == true)
                          .AsQueryable();

            if (buildingId != null)
            {
                query = query.Where(x => x.BuildingId == buildingId).AsQueryable();
            }

            var roomList = await query.OrderBy(r => r.Name).ToListAsync();

            foreach (var item in roomList)
            {
                var vmRoom = item != null ? new RoomVM()
                {
                    Id = item.Id,
                    LocationPropertyId = item.Building?.Location?.PropertyId,
                    BuildingPropertyId = item.Building?.PropertyId,
                    PropertyId = item.PropertyId,
                    Name = item.Name,
                    DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,
                    Description = item.Description,
                    Capacity = item.Capacity,

                    RoomTypeId = item.RoomTypeId,
                    RoomType = item.RoomType != null ? new RoomTypeVM()
                    {
                        Id = item.RoomType.Id,
                        Name = item.RoomType.Name,
                    } : new RoomTypeVM(),
                    Active = item.Active,

                    RoomAmenities = item.RoomAmenities != null ? item.RoomAmenities.Where(x => x.IsDeleted == false
                                          && x.Amenity != null
                                          && x.RoomId == item.Id)
                    .Select(p => new AmenityVM
                    {
                        Id = p.Amenity.Id,
                        Name = p.Amenity.Name,
                        Sequence = p.Amenity.Sequence,
                        IsChecked = true,
                        Checked = "Checked"
                    }).OrderBy(x => x.Sequence).ToList() : new List<AmenityVM>()
                } : new RoomVM();
                vmRoomList.Add(vmRoom);
            };
            return vmRoomList.OrderBy(x => x.DisplayName ?? x.Name);
        }

        //Create
        public async Task<int> Create(RoomVM vm)
        {
            var item = new Room()
            {
                PropertyId = vm.PropertyId,
                Name = vm.Name?.Trim(),
                DisplayName = vm.DisplayName?.Trim(),
                Description = vm.Description?.Trim(),
                Capacity = vm.Capacity,
                RoomTypeId = vm.RoomTypeId,
                Longitude = vm.Longitude,
                Latitude = vm.Latitude,
                BuildingId = vm.BuildingId,
                Active = vm.Active,
                Notes = vm.Notes?.Trim(),
                IsDeleted = false,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.CreatedBy
            };

            var result = Db.Rooms.Add(item);
            await Db.SaveChangesAsync();

            foreach (var amenityId in vm.RoomAmenityIds ?? new List<int>())
            {
                var roomAmenity = await Db.RoomAmenities
                                          .Where(x => x.IsDeleted == false
                                                 && x.RoomId == result.Id
                                                 && x.AmenityId == amenityId)
                                          .FirstOrDefaultAsync();
                if (roomAmenity == null)
                {
                    RoomAmenity ra = new RoomAmenity()
                    {
                        RoomId = result.Id,
                        AmenityId = amenityId,
                        IsDeleted = false,
                        CreatedBy = vm.ModifiedBy,
                        ModifiedBy = vm.ModifiedBy
                    };
                    Db.RoomAmenities.Add(ra);
                }
            }
            await Db.SaveChangesAsync();

            return result.Id;
        }

        //Get
        public async Task<RoomVM> Get(int Id)
        {
            var item = await Db.Rooms
                               .Include(x => x.Building.Location)
                               .Include(x => x.RoomType)
                               .Include(x => x.RoomAmenities.Select(r => r.Amenity))
                               .Where(x => x.IsDeleted == false && x.Id == Id)
                               .FirstOrDefaultAsync();

            if (item == null)
            {
                return null;
            }

            var vm = new RoomVM()
            {
                Id = item.Id,
                LocationPropertyId = item.Building?.Location?.PropertyId,
                BuildingPropertyId = item.Building?.PropertyId,
                PropertyId = item.PropertyId,
                OriginalPropertyId = item.PropertyId,
                Name = item.Name,
                OriginalName = item.Name,
                DisplayName = item.DisplayName,
                OriginalDisplayName = item.DisplayName,
                Description = item.Description,
                Capacity = item.Capacity,

                RoomTypeId = item.RoomTypeId,
                RoomType = item.RoomType != null ? new RoomTypeVM()
                {
                    Id = item.RoomType.Id,
                    Name = item.RoomType.Name,
                } : new RoomTypeVM(),

                RoomAmenities = item.RoomAmenities != null ? item.RoomAmenities.Where(x => x.IsDeleted == false
                                                          && x.Amenity != null
                                                          && x.RoomId == item.Id)
                .Select(p => new AmenityVM
                {
                    Id = p.Amenity.Id,
                    Name = p.Amenity.Name,
                    Sequence = p.Amenity.Sequence,
                    IsChecked = true,
                    Checked = "Checked"
                }).OrderBy(x => x.Sequence).ToList() : new List<AmenityVM>(),

                Longitude = item.Longitude,
                Latitude = item.Latitude,

                BuildingId = item.BuildingId,
                Building = item.Building != null ? new BuildingVM()
                {
                    Id = item.Building.Id,
                    Name = item.Building.Name,
                    DisplayName = !string.IsNullOrEmpty(item.Building.DisplayName) ? item.Building.DisplayName : item.Building.Name,
                    LocationId = item.Building.LocationId,
                    Location = item.Building.Location != null ? new LocationVM()
                    {
                        Id = item.Building.Location.Id,
                        Name = item.Building.Location.Name,
                        DisplayName = !string.IsNullOrEmpty(item.Building.Location.DisplayName) ? item.Building.Location.DisplayName : item.Building.Location.Name,
                    } : new LocationVM(),
                } : new BuildingVM(),

                Active = item.Active,
                Notes = item.Notes,
                AssetCount = item.Assets.Count(x => x.IsDeleted == false)
            };

            vm.Location = vm.Building?.Location;
            vm.LocationId = vm.Building?.LocationId;
            vm.OriginalLocationId = vm.Building?.LocationId;
            vm.LocationName = vm.Building?.Location?.Name;
            vm.OriginalBuildingId = vm.BuildingId;
            vm.RoomTypeName = vm.RoomType.Name;
            vm.RoomAmenityIds = vm.RoomAmenities?.Select(x => x.Id).ToList();
            vm.OriginalRoomAmenityIds = vm.RoomAmenities?.Select(x => x.Id).ToList().ListToString();
            vm.RoomAmenityNames = vm.RoomAmenities?.Select(x => x.Name).ToList().ListToString();

            return vm;
        }

        //Save
        public async Task Save(RoomVM vm)
        {
            // Amenities
            var originalIds = vm.OriginalRoomAmenityIds.StringToIntList();
            var removed = originalIds?.Except(vm.RoomAmenityIds ?? new List<int>()).ToList();
            var added = vm.RoomAmenityIds?.Except(originalIds ?? new List<int>()).ToList();

            foreach (var amenityId in removed ?? new List<int>())
            {
                var roomAmenity = await Db.RoomAmenities
                                          .Where(x => x.IsDeleted == false && x.RoomId == vm.Id && x.AmenityId == amenityId)
                                          .FirstOrDefaultAsync();
                if (roomAmenity != null)
                {
                    roomAmenity.IsDeleted = true;
                    roomAmenity.ModifiedBy = vm.ModifiedBy;
                    Db.Entry(roomAmenity).State = EntityState.Modified;
                }
            }

            foreach (var amenityId in added ?? new List<int>())
            {
                var roomAmenity = await Db.RoomAmenities
                                          .Where(x => x.IsDeleted == false && x.RoomId == vm.Id && x.AmenityId == amenityId)
                                          .FirstOrDefaultAsync();
                if (roomAmenity == null)
                {
                    RoomAmenity ra = new RoomAmenity()
                    {
                        RoomId = vm.Id,
                        AmenityId = amenityId,
                        IsDeleted = false,
                        CreatedBy = vm.ModifiedBy,
                        ModifiedBy = vm.ModifiedBy
                    };
                    Db.RoomAmenities.Add(ra);
                }
            }
            //

            var item = new Room()
            {
                Id = vm.Id,
                PropertyId = vm.PropertyId,
                Name = vm.Name?.Trim(),
                DisplayName = vm.DisplayName?.Trim(),
                Description = vm.Description?.Trim(),
                Capacity = vm.Capacity,
                RoomTypeId = vm.RoomTypeId,
                Longitude = vm.Longitude,
                Latitude = vm.Latitude,
                BuildingId = vm.BuildingId,
                Active = vm.Active,
                Notes = vm.Notes?.Trim(),
                ModifiedBy = vm.ModifiedBy
            };

            var entry = Db.Entry(item);
            entry.State = EntityState.Modified;
            Db.Entry(item).Property(x => x.CreatedBy).IsModified = false;
            Db.Entry(item).Property(x => x.DateAdded).IsModified = false;
            await Db.SaveChangesAsync();

            if (vm.LocationId != vm.OriginalLocationId)
            {
                await BuildingLogic.UpdateAssetLocation(vm.LocationId, vm.BuildingId, vm.ModifiedBy);
            }

            if (vm.BuildingId != vm.OriginalBuildingId)
            {
                await UpdateAssetBuilding(vm.BuildingId, vm.Id, vm.ModifiedBy);
            }
        }

        //Delete
        public async Task Delete(RoomVM vm)
        {
            var item = await Db.Rooms.FindAsync(vm.Id);

            if (item != null)
            {
                item.IsDeleted = true;
                item.ModifiedBy = vm.ModifiedBy;
                var entry = Db.Entry(item);
                entry.State = EntityState.Modified;
                Db.Entry(item).Property(x => x.CreatedBy).IsModified = false;
                Db.Entry(item).Property(x => x.DateAdded).IsModified = false;
                await Db.SaveChangesAsync();
            }
        }

        //Get Room Types
        public async Task<IEnumerable<RoomTypeVM>> GetRoomTypes()
        {
            var vmRoomTypeList = new List<RoomTypeVM>();

            var roomTypes = await Db.RoomTypes
                                    .Where(x => x.IsDeleted == false)
                                    .OrderBy(r => r.Sequence).ToListAsync();

            foreach (var item in roomTypes)
            {
                var vmRoomType = item != null ? new RoomTypeVM()
                {
                    Id = item.Id,
                    Name = item.Name
                } : new RoomTypeVM();
                vmRoomTypeList.Add(vmRoomType);
            };
            return vmRoomTypeList.OrderBy(x => x.Sequence);
        }

        //Get Amenities
        public async Task<IEnumerable<AmenityVM>> GetRoomAmenities(IEnumerable<int> SelectedIds)
        {
            var amenities = await Db.Amenities
                                    .Where(x => x.Active == true && x.Type.Name == "Room")
                                    .OrderBy(r => r.Sequence).ToListAsync();

            var amenityList = new List<AmenityVM>();

            foreach (var item in amenities)
            {
                var vmAmenity = item != null ? new AmenityVM()
                {
                    Id = item.Id,
                    Name = item.Name
                } : new AmenityVM();

                if (SelectedIds != null && SelectedIds.Contains(vmAmenity.Id))
                {
                    vmAmenity.IsChecked = true;
                    vmAmenity.Checked = "Checked";
                }
                amenityList.Add(vmAmenity);
            }

            return amenityList.OrderBy(x => x.Sequence);
        }

        //Update Building
        private async Task UpdateAssetBuilding(int? buildingId, int? roomId, int? userId)
        {
            var items = await Db.Assets.Where(x => x.RoomId == roomId).ToListAsync();

            foreach (var item in items)
            {
                item.BuildingId = buildingId;
                item.ModifiedBy = userId;
                var entry = Db.Entry(item);
                entry.State = EntityState.Modified;
                Db.Entry(item).Property(x => x.CreatedBy).IsModified = false;
                Db.Entry(item).Property(x => x.DateAdded).IsModified = false;
            }
            await Db.SaveChangesAsync();
        }

        //Check Property Id
        public async Task<bool> PropertyIdExist(string propertyId, int? buildingId = 0)
        {
            var item = await Db.Rooms.Where(x => x.IsDeleted == false
                                                && x.BuildingId == buildingId
                                                && x.PropertyId == propertyId)
                                         .FirstOrDefaultAsync();

            if (item != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}