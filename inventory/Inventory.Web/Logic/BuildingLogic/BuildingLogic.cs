using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PagedList;
using Inventory.Data.Models;
using Inventory.Data.Services;
using Inventory.Web.Common;
using Inventory.Web.ViewModels;

namespace Inventory.Web.Logic
{
    public class BuildingLogic : AssetBaseLogic, IBuildingLogic
    {
        //Constructor
        public BuildingLogic(InventoryDbContext db, ICommonLogic commonLogic)
        {
            CommonLogic = commonLogic;
            Db = db;
        }

        //Get List
        public async Task<IEnumerable<BuildingVM>> GetList(int? locationId, string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000)
        {
            int pageNumber = page;
            var vmList = new List<BuildingVM>();

            //
            var query = Db.Buildings
                          .Include(x => x.Location)
                          .Include(x => x.City)
                          .Include(x => x.Rooms)
                          .Where(x => x.IsDeleted == false);
            //

            if (locationId != null)
            {
                query = query.Where(x => x.LocationId == locationId).AsQueryable();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => (string.IsNullOrEmpty(x.DisplayName) ? x.Name.Contains(searchString) : x.DisplayName.Contains(searchString))
                                      || (string.IsNullOrEmpty(x.Location.Code) ? x.Location.Name.Contains(searchString) : x.Location.Code.Contains(searchString))
                                      || (x.Code.Contains(searchString))
                                      || (x.Location != null && x.Location.Code.Contains(searchString))
                                      || ((x.Location != null) && (string.IsNullOrEmpty(x.Location.DisplayName) ? x.Location.Name.Contains(searchString) : x.Location.DisplayName.Contains(searchString))));
            }

            var List = await query.ToListAsync();

            foreach (var item in List)
            {
                var vm = new BuildingVM()
                {
                    Id = item.Id,
                    PropertyId = item.PropertyId,
                    Name = item.Name,
                    DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,
                    Code = item.Code,
                    Description = item.Description,
                    Phone = item.Phone,
                    AddressLine1 = item.AddressLine1,
                    AddressLine2 = item.AddressLine2,
                    CityId = item.CityId,
                    City = item.City != null ? new CityVM()
                    {
                        Id = item.City.Id,
                        Name = item.City.Name,
                        Active = item.City.Active
                    } : new CityVM(),

                    StateId = item.StateId,
                    State = item.State != null ? new StateVM()
                    {
                        Id = item.State.Id,
                        Code = item.State.Code,
                        Name = item.State.Name,
                        Active = item.State.Active
                    } : new StateVM(),

                    PostalCode = item.PostalCode,

                    Longitude = item.Longitude,
                    Latitude = item.Latitude,

                    LocationId = item.LocationId,
                    Location = item.Location != null ? new LocationVM()
                    {
                        Id = item.Location.Id,
                        Code = item.Location.Code,
                        Name = item.Location.Name,
                        DisplayName = !string.IsNullOrEmpty(item.Location.DisplayName) ? item.Location.DisplayName : item.Location.Name,
                        Active = item.Location.Active
                    } : new LocationVM(),

                    RoomCount = item?.Rooms.Where(r => r.IsDeleted == false).Count(),

                    Active = item.Active,
                    ActiveStatus = item.Active == true ? "Active" : "InActive",
                    Notes = item.Notes
                };
                vmList.Add(vm);
            };

            switch (sortOrder)
            {
                case "id":
                    vmList = vmList.OrderBy(s => !string.IsNullOrEmpty(s.PropertyId) ? s.PropertyId : "9999").ToList();
                    break;

                case "id_desc":
                    vmList = vmList.OrderByDescending(s => !string.IsNullOrEmpty(s.PropertyId) ? s.PropertyId : "0000").ToList();
                    break;

                case "code":
                    vmList = vmList.OrderBy(s => string.IsNullOrEmpty(s.Code) ? "zzzzzzzzz" : s.Code).ToList();
                    break;

                case "code_desc":
                    vmList = vmList.OrderByDescending(s => s.Code ?? "").ToList();
                    break;

                case "displayname":
                    vmList = vmList.OrderBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz")
                                 .ThenBy(s => s.Location != null ? s.Location.DisplayName != null && s.Location.DisplayName != "" ? s.Location.DisplayName : s.Location.Name ?? "zzz" : "zzz").ToList();
                    break;

                case "displayname_desc":
                    vmList = vmList.OrderByDescending(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "")
                                 .ThenBy(s => s.Location != null ? s.Location.DisplayName != null && s.Location.DisplayName != "" ? s.Location.DisplayName : s.Location.Name ?? "zzz" : "zzz").ToList();
                    break;

                case "location":
                    vmList = vmList.OrderBy(s => s.Location != null ? s.Location.DisplayName != null && s.Location.DisplayName != "" ? s.Location.DisplayName : s.Location.Name ?? "zzz" : "zzz")
                                 .ThenBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "location_desc":
                    vmList = vmList.OrderByDescending(s => s.Location != null ? s.Location.DisplayName != null && s.Location.DisplayName != "" ? s.Location.DisplayName : s.Location.Name ?? "" : "")
                                 .ThenBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "locationcode":
                    vmList = vmList.OrderBy(s => s.Location != null ? s.Location.Code != null && s.Location.Code != "" ? s.Location.Code : "zzz" : "zzz")
                                   .ThenBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "locationcode_desc":
                    vmList = vmList.OrderByDescending(s => s.Location != null ? s.Location.Code != null && s.Location.Code != "" ? s.Location.Code : "" : "")
                                   .ThenBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "roomcount":
                    vmList = vmList.OrderBy(s => s.RoomCount).ToList();
                    break;

                case "roomcount_desc":
                    vmList = vmList.OrderByDescending(s => s.RoomCount).ToList();
                    break;

                default:
                    vmList = vmList.OrderBy(s => s.DisplayName != null && s.DisplayName != "" ? s.DisplayName : s.Name ?? "zzz")
                                  .ThenBy(s => s.Location != null ? s.Location.DisplayName != null && s.Location.DisplayName != "" ? s.Location.DisplayName : s.Location.Name ?? "zzz" : "zzz").ToList();
                    break;
            }

            var items = vmList.ToPagedList(pageNumber, pageSize);

            StaticPagedList<BuildingVM> list = new StaticPagedList<BuildingVM>(items, items.PageNumber, items.PageSize, items.TotalItemCount);

            return list;
        }

        //Get Buildings
        public async Task<IEnumerable<BuildingVM>> GetBuildings(int? locationId = null)
        {
            var vmBuildingList = new List<BuildingVM>();

            var query = Db.Buildings.Where(x => x.IsDeleted == false && x.Active == true).AsQueryable();

            if (locationId != null)
            {
                query = query.Where(x => x.LocationId == locationId).AsQueryable();
            }

            var buildingList = await query.OrderBy(r => r.Name).ToListAsync();

            foreach (var item in buildingList)
            {
                var vmBuilding = item != null ? new BuildingVM()
                {
                    Id = item.Id,
                    LocationPropertyId = item.Location?.PropertyId,
                    PropertyId = item.PropertyId,
                    Name = item.Name,
                    DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,
                    Description = item.Description,
                    Active = item.Active
                } : new BuildingVM();
                vmBuildingList.Add(vmBuilding);
            };
            return vmBuildingList.OrderBy(x => x.DisplayName ?? x.Name);
        }

        //Create
        public async Task<int> Create(BuildingVM vm)
        {
            var item = new Building()
            {
                PropertyId = vm.PropertyId,
                Name = vm.Name?.Trim(),
                DisplayName = vm.DisplayName?.Trim(),
                Description = vm.Description?.Trim(),
                Code = vm.Code?.Trim().ToUpper(),
                Phone = vm.Phone?.Trim(),
                AddressLine1 = vm.AddressLine1?.Trim(),
                AddressLine2 = vm.AddressLine2?.Trim(),
                CityId = vm.CityId,
                StateId = vm.StateId,
                PostalCode = vm.PostalCode,
                Longitude = vm.Longitude,
                Latitude = vm.Latitude,
                LocationId = vm.LocationId,
                Active = vm.Active,
                Notes = vm.Notes?.Trim(),
                IsDeleted = false,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.CreatedBy
            };

            var result = Db.Buildings.Add(item);
            await Db.SaveChangesAsync();

            return result.Id;
        }

        //Get
        public async Task<BuildingVM> Get(int Id)
        {
            var item = await Db.Buildings
                               .Include(x => x.City)
                               .Include(x => x.State)
                               .Include(x => x.Location)
                               .Where(x => x.IsDeleted == false && x.Id == Id)
                               .FirstOrDefaultAsync();

            if (item == null)
            {
                return null;
            }

            var vm = new BuildingVM()
            {
                Id = item.Id,
                LocationPropertyId = item.Location?.PropertyId,
                PropertyId = item.PropertyId,
                OriginalPropertyId = item.PropertyId,
                Name = item.Name,
                OriginalName = item.Name,
                DisplayName = item.DisplayName,
                OriginalDisplayName = item.DisplayName,
                Code = item.Code,
                OriginalCode = item.Code,
                Description = item.Description,
                Phone = item.Phone,
                AddressLine1 = item.AddressLine1,
                AddressLine2 = item.AddressLine2,
                CityId = item.CityId,
                City = item.City != null ? new CityVM()
                {
                    Id = item.City.Id,
                    Name = item.City.Name,
                    Active = item.City.Active
                } : new CityVM(),

                StateId = item.StateId,
                State = item.State != null ? new StateVM()
                {
                    Id = item.State.Id,
                    Code = item.State.Code,
                    Name = item.State.Name,
                    Active = item.State.Active
                } : new StateVM(),

                PostalCode = item.PostalCode,

                Longitude = item.Longitude,
                Latitude = item.Latitude,

                LocationId = item.LocationId,
                OriginalLocationId = item.LocationId,
                Location = item.Location != null ? new LocationVM()
                {
                    Id = item.Location.Id,
                    PropertyId = item.PropertyId,
                    Code = item.Location.Code,
                    Name = item.Location.Name,
                    DisplayName = !string.IsNullOrEmpty(item.Location.DisplayName) ? item.Location.DisplayName : item.Location.Name,
                } : new LocationVM(),

                Active = item.Active,
                Notes = item.Notes,

                AssetCount = item.Assets.Count(x => x.IsDeleted == false),
                RoomCount = item.Rooms.Count(x => x.IsDeleted == false)
            };

            vm.Cities = await CommonLogic.GetCities("fl");
            vm.States = await CommonLogic.GetStates("fl");

            return vm;
        }

        //Save
        public async Task Save(BuildingVM vm)
        {
            var item = new Building()
            {
                Id = vm.Id,
                PropertyId = vm.PropertyId,
                Name = vm.Name?.Trim(),
                DisplayName = vm.DisplayName?.Trim(),
                Description = vm.Description?.Trim(),
                Code = vm.Code?.Trim().ToUpper(),
                Phone = vm.Phone?.Trim(),
                AddressLine1 = vm.AddressLine1?.Trim(),
                AddressLine2 = vm.AddressLine2?.Trim(),
                CityId = vm.CityId,
                StateId = vm.StateId,
                PostalCode = vm.PostalCode,
                Longitude = vm.Longitude,
                Latitude = vm.Latitude,
                LocationId = vm.LocationId,
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
                await UpdateAssetLocation(vm.LocationId, vm.Id, vm.ModifiedBy);
            }
        }

        //Delete
        public async Task Delete(BuildingVM vm)
        {
            var item = await Db.Buildings.FindAsync(vm.Id);

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

        //Check Code
        public async Task<bool> CodeExist(string code)
        {
            code = code?.Trim().ToLower();

            var item = await Db.Buildings.Where(x => x.IsDeleted == false
                                                && x.Code.Trim().ToLower() == code)
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

        //Check Property Id
        public async Task<bool> PropertyIdExist(string propertyId, int? locationId = 0)
        {
            var item = await Db.Buildings.Where(x => x.IsDeleted == false
                                                && x.LocationId == locationId
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

        //Update Location
        public async Task UpdateAssetLocation(int? locationId, int? buildingId, int? userId)
        {
            var items = await Db.Assets.Where(x => x.BuildingId == buildingId).ToListAsync();

            foreach (var item in items)
            {
                item.LocationId = locationId;
                item.ModifiedBy = userId;
                var entry = Db.Entry(item);
                entry.State = EntityState.Modified;
                Db.Entry(item).Property(x => x.CreatedBy).IsModified = false;
                Db.Entry(item).Property(x => x.DateAdded).IsModified = false;
            }
            await Db.SaveChangesAsync();
        }
    }
}