using System;
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
    public class LocationLogic : AssetBaseLogic, ILocationLogic
    {
        //Constructor
        public LocationLogic(InventoryDbContext db, ICommonLogic commonLogic, IUserLogic userLogic)
        {
            CommonLogic = commonLogic;
            UserLogic = userLogic;
            Db = db;
        }

        //Get List
        public async Task<IEnumerable<LocationVM>> GetList(string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000)
        {
            int pageNumber = page;
            var vmList = new List<LocationVM>();

            //
            var query = Db.Locations
                          .Include(x => x.LeadManager)
                          .Include(x => x.Buildings)
                          .Include(x => x.LocationAliases)
                          .Include(x => x.Buildings.Select(b => b.Rooms))
                          .Include(x => x.City)
                          .Where(x => x.IsDeleted == false);
            //

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => (string.IsNullOrEmpty(x.DisplayName) ? x.Name.Contains(searchString) : x.DisplayName.Contains(searchString))
                                      || (x.LocationAliases.Any(l => l.Name.Contains(searchString)))
                                      || (x.Code != null && x.Code.Contains(searchString))
                                      || (x.LeadManager != null && x.LeadManager.LastName.Contains(searchString))
                                      || (x.LeadManager != null && x.LeadManager.FirstName.Contains(searchString)));
            }

            var List = await query.ToListAsync();

            foreach (var item in List)
            {
                var vm = new LocationVM()
                {
                    Id = item.Id,
                    PropertyId = item.PropertyId,
                    Name = item.Name,
                    DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,

                    LocationAliases = item.LocationAliases != null ? item.LocationAliases.Where(x => x.LocationId == item.Id)
                    .Select(l => new LocationAliasVM
                    {
                        Id = l.Id,
                        Name = l.Name

                    }).OrderBy(x => x.Name).ToList() : new List<LocationAliasVM>(),

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
                    SubnetAddress = item.SubnetAddress,

                    LeadManagerId = item.LeadManagerId,
                    LeadManager = item.LeadManager != null ? new UserVM()
                    {
                        Id = item.LeadManager.Id,
                        Name = item.LeadManager.LastName + ", " + item.LeadManager.FirstName,
                        FirstName = item.LeadManager.FirstName,
                        LastName = item.LeadManager.LastName,
                    } : new UserVM(),

                    BuildingCount = item?.Buildings.Where(b => b.IsDeleted == false).Count(),
                    RoomCount = item?.Buildings.Where(b => b.IsDeleted == false).SelectMany(r => r?.Rooms).Where(r => r.IsDeleted == false).Count(),

                    Active = item.Active,
                    ActiveStatus = item.Active == true ? "Active" : "InActive",
                    Notes = item.Notes
                };

                vm.LocationAliasNames = vm.LocationAliases?.OrderBy(x => x.Name).Select(x => x.Name.Trim()).ToList().ListToString();

                vmList.Add(vm);
            };

            switch (sortOrder)
            {
                case "id":
                    vmList = vmList.OrderBy(s => !string.IsNullOrEmpty(s.PropertyId) ? s.PropertyId : "9999")
                                   .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "id_desc":
                    vmList = vmList.OrderByDescending(s => !string.IsNullOrEmpty(s.PropertyId) ? s.PropertyId : "0000")
                                   .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "code":
                    vmList = vmList.OrderBy(s => String.IsNullOrEmpty(s.Code) ? "zzzzzzzzz" : s.Code)
                                   .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "code_desc":
                    vmList = vmList.OrderByDescending(s => s.Code ?? "")
                                   .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "displayname":
                    vmList = vmList.OrderBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "displayname_desc":
                    vmList = vmList.OrderByDescending(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "").ToList();
                    break;

                case "alias":
                    vmList = vmList.OrderBy(s => (s.LocationAliases != null && s.LocationAliases.Count() > 0) ? s.LocationAliasNames : "zzzzz").ToList();
                    break;

                case "alias_desc":
                    vmList = vmList.OrderByDescending(s => (s.LocationAliases != null && s.LocationAliases.Count() > 0) ? s.LocationAliasNames : "").ToList();
                    break;

                case "leadmanager":
                    vmList = vmList.OrderBy(s => s.LeadManager != null ? s.LeadManager.LastName ?? "zzz" : "zzz")
                                   .ThenBy(s => s.LeadManager != null ? s.LeadManager.FirstName ?? "zzz" : "zzz")
                                   .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "leadmanager_desc":
                    vmList = vmList.OrderByDescending(s => s.LeadManager != null ? s.LeadManager.LastName : "")
                                   .ThenBy(s => s.LeadManager != null ? s.LeadManager.FirstName : "")
                                   .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "buildingcount":
                    vmList = vmList.OrderBy(s => s.BuildingCount)
                                   .ThenBy(s => s.RoomCount)
                                   .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "buildingcount_desc":
                    vmList = vmList.OrderByDescending(s => s.BuildingCount)
                                   .ThenBy(s => s.RoomCount)
                                   .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "roomcount":
                    vmList = vmList.OrderBy(s => s.RoomCount)
                                   .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                case "roomcount_desc":
                    vmList = vmList.OrderByDescending(s => s.RoomCount)
                                   .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;

                default:
                    vmList = vmList.OrderBy(s => String.IsNullOrEmpty(s.Code) ? "zzzzzzzzz" : s.Code)
                                   .ThenBy(s => (s.DisplayName != null && s.DisplayName != "") ? s.DisplayName : s.Name ?? "zzz").ToList();
                    break;
            }

            var items = vmList.ToPagedList(pageNumber, pageSize);
            StaticPagedList<LocationVM> list = new StaticPagedList<LocationVM>(items, items.PageNumber, items.PageSize, items.TotalItemCount);
            return list;
        }

        //Get Locations
        public async Task<IEnumerable<LocationVM>> GetLocations()
        {
            var vmLocationList = new List<LocationVM>();

            var locations = await Db.Locations
                                    .Include(x => x.LocationAmenities.Select(r => r.Amenity))
                                    .Where(x => x.IsDeleted == false && x.Active == true).OrderBy(r => r.Name).ToListAsync();

            foreach (var item in locations)
            {
                var vmLocation = item != null ? new LocationVM()
                {
                    Id = item.Id,
                    PropertyId = item.PropertyId,
                    Name = item.Name,
                    DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name,
                    Description = item.Description,
                    Active = item.Active,
                    SubnetAddress = item.SubnetAddress,

                    LocationAmenities = item.LocationAmenities != null ? item.LocationAmenities.Where(x => x.IsDeleted == false
                      && x.Amenity != null
                      && x.LocationId == item.Id)
                    .Select(p => new AmenityVM
                    {
                        Id = p.Amenity.Id,
                        Name = p.Amenity.Name,
                        Sequence = p.Amenity.Sequence,
                        IsChecked = true,
                        Checked = "Checked"
                    }).OrderBy(x => x.Sequence).ToList() : new List<AmenityVM>()
                } : new LocationVM();

                vmLocationList.Add(vmLocation);
            };
            return vmLocationList.OrderBy(x => x.DisplayName ?? x.Name);
        }

        //Create
        public async Task<int> Create(LocationVM vm)
        {
            var item = new Location()
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
                SubnetAddress = vm.SubnetAddress,
                LeadManagerId = vm.LeadManagerId,
                Active = vm.Active,
                Notes = vm.Notes?.Trim(),
                IsDeleted = false,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.CreatedBy
            };

            var result = Db.Locations.Add(item);
            await Db.SaveChangesAsync();

            foreach (var amenityId in vm.LocationAmenityIds ?? new List<int>())
            {
                var amenity = await Db.LocationAmenities
                                      .Where(x => x.IsDeleted == false
                                             && x.LocationId == result.Id
                                             && x.AmenityId == amenityId)
                                      .FirstOrDefaultAsync();

                if (amenity == null)
                {
                    LocationAmenity la = new LocationAmenity()
                    {
                        LocationId = result.Id,
                        AmenityId = amenityId,
                        IsDeleted = false,
                        CreatedBy = vm.ModifiedBy,
                        ModifiedBy = vm.ModifiedBy
                    };
                    Db.LocationAmenities.Add(la);
                }
            }

            // Location Alias
            var aliasNames = vm.LocationAliasNames?.Split(',').ToList();

            foreach (var an in aliasNames ?? new List<string>())
            {
                if (!string.IsNullOrEmpty(an))
                {
                    var alias = await Db.LocationAliases
                                        .Where(x => x.Name.Trim().ToLower() == an.Trim().ToLower())
                                        .FirstOrDefaultAsync();

                    if (alias == null)
                    {
                        LocationAlias la = new LocationAlias()
                        {
                            LocationId = result.Id,
                            Name = an.Trim()
                        };
                        Db.LocationAliases.Add(la);
                    }
                }
            }

            await Db.SaveChangesAsync();

            return result.Id;
        }

        //Get
        public async Task<LocationVM> Get(int Id)
        {
            var item = await Db.Locations
                               .Include(x => x.LocationAmenities.Select(r => r.Amenity))
                               .Include(x => x.LeadManager)
                               .Include(x => x.LocationAliases)
                               .Where(x => x.IsDeleted == false && x.Id == Id)
                               .FirstOrDefaultAsync();

            if (item == null)
            {
                return null;
            }

            var vm = new LocationVM()
            {
                Id = item.Id,
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
                SubnetAddress = item.SubnetAddress,
                OriginalSubnetAddress = item.SubnetAddress,

                LocationAmenities = item.LocationAmenities != null ? item.LocationAmenities.Where(x => x.IsDeleted == false
                                          && x.Amenity != null
                                          && x.LocationId == item.Id)
                .Select(p => new AmenityVM
                {
                    Id = p.Amenity.Id,
                    Name = p.Amenity.Name,
                    Sequence = p.Amenity.Sequence,
                    IsChecked = true,
                    Checked = "Checked"
                }).OrderBy(x => x.Sequence).ToList() : new List<AmenityVM>(),

                LocationAliases = item.LocationAliases != null ? item.LocationAliases.Where(x => x.LocationId == item.Id)
                .Select(l => new LocationAliasVM
                {
                    Id = l.Id,
                    Name = l.Name

                }).OrderBy(x => x.Name).ToList() : new List<LocationAliasVM>(),

                LeadManagerId = item.LeadManagerId,
                LeadManager = item.LeadManager != null ? new UserVM()
                {
                    Id = item.LeadManager.Id,
                    Name = item.LeadManager.FirstName + " " + item.LeadManager.LastName,
                } : new UserVM(),

                Active = item.Active,
                Notes = item.Notes,
                AssetCount = item.Assets.Count(x => x.IsDeleted == false),
                BuildingCount = item.Buildings.Count(x => x.IsDeleted == false),

                ModifiedBy = item.ModifiedBy,
                DateModified = item.DateModified
            };

            vm.Cities = await CommonLogic.GetCities("fl");
            vm.States = await CommonLogic.GetStates("fl");
            vm.Users = await UserLogic.GetUsers();
            vm.LocationAmenityIds = vm.LocationAmenities?.Select(x => x.Id).ToList();
            vm.OriginalLocationAmenityIds = vm.LocationAmenities?.Select(x => x.Id).ToList().ListToString();
            vm.LocationAmenityNames = vm.LocationAmenities?.Select(x => x.Name).ToList().ListToString();
            vm.LocationAliasNames = vm.LocationAliases?.OrderBy(x => x.Name).Select(x => x.Name.Trim()).ToList().ListToString();
            vm.OriginalLocationAliasNames = vm.LocationAliasNames;

            return vm;
        }

        //Save
        public async Task Save(LocationVM vm)
        {
            // Amenities
            var originalIds = vm.OriginalLocationAmenityIds.StringToIntList();
            var removed = originalIds?.Except(vm.LocationAmenityIds ?? new List<int>()).ToList();
            var added = vm.LocationAmenityIds?.Except(originalIds ?? new List<int>()).ToList();

            foreach (var amenityId in removed ?? new List<int>())
            {
                var locationAmenity = await Db.LocationAmenities
                                              .Where(x => x.IsDeleted == false
                                                     && x.LocationId == vm.Id
                                                     && x.AmenityId == amenityId)
                                              .FirstOrDefaultAsync();
                if (locationAmenity != null)
                {
                    locationAmenity.IsDeleted = true;
                    locationAmenity.ModifiedBy = vm.ModifiedBy;
                    Db.Entry(locationAmenity).State = EntityState.Modified;
                }
            }

            foreach (var amenityId in added ?? new List<int>())
            {
                var locationAmenity = await Db.LocationAmenities
                                              .Where(x => x.IsDeleted == false
                                                     && x.LocationId == vm.Id
                                                     && x.AmenityId == amenityId)
                                              .FirstOrDefaultAsync();

                if (locationAmenity == null)
                {
                    LocationAmenity la = new LocationAmenity()
                    {
                        LocationId = vm.Id,
                        AmenityId = amenityId,
                        IsDeleted = false,
                        CreatedBy = vm.ModifiedBy,
                        ModifiedBy = vm.ModifiedBy
                    };
                    Db.LocationAmenities.Add(la);
                }
            }
            //

            // Aliases
            if (vm.OriginalLocationAliasNames?.Length > 0 || vm.LocationAliasNames?.Length > 0)
            {
                await ManageAliases(vm.Id, vm.OriginalLocationAliasNames, vm.LocationAliasNames);
            }


            var item = new Location()
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
                SubnetAddress = vm.SubnetAddress,
                LeadManagerId = vm.LeadManagerId,
                Active = vm.Active,
                Notes = vm.Notes?.Trim(),
                ModifiedBy = vm.ModifiedBy,
            };

            var entry = Db.Entry(item);
            entry.State = EntityState.Modified;
            Db.Entry(item).Property(x => x.CreatedBy).IsModified = false;
            Db.Entry(item).Property(x => x.DateAdded).IsModified = false;
            await Db.SaveChangesAsync();
        }

        //Delete
        public async Task Delete(LocationVM vm)
        {
            var item = await Db.Locations.FindAsync(vm.Id);

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

        //Get Amenities
        public async Task<IEnumerable<AmenityVM>> GetLocationAmenities(IEnumerable<int> SelectedIds)
        {
            var amenities = await Db.Amenities
                                    .Where(x => x.Active == true && x.Type.Name == "Location")
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

        //Check Code
        public async Task<bool> CodeExist(string code)
        {
            code = code?.Trim().ToLower();

            if (string.IsNullOrEmpty(code))
                return false;

            var item = await Db.Locations.Where(x => x.IsDeleted == false
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
        public async Task<bool> PropertyIdExist(string propertyId)
        {
            var item = await Db.Locations.Where(x => x.IsDeleted == false
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

        //Check Subnet
        public async Task<bool> SubnetExist(string subnet)
        {
            var item = await Db.Locations.Where(x => x.IsDeleted == false
                                                && x.SubnetAddress == subnet)
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

        //Check Subnet
        public async Task<string> AliasExist(int? locId, string alias)
        {
            var query = Db.Locations
                          .Include(x => x.LocationAliases)
                          .Where(x => x.DisplayName.ToLower() == alias.ToLower()
                                 || x.Name.ToLower() == alias.ToLower()
                                 || x.LocationAliases.Any(l => l.Name.ToLower() == alias.ToLower()))
                          .Select(x => new { x.DisplayName, x.Name, x.Id, x.LocationAliases });

            if ((locId != null) && (locId > 0))
            {
                query = query.Where(x => x.Id != locId || x.LocationAliases.Any(l => l.LocationId != locId));
            }

            var item = await query.FirstOrDefaultAsync();

            if (item != null)
            {
                return !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : item.Name;
            }
            else
            {
                return "";
            }
        }

        //Add / Remove Aliases
        public async Task ManageAliases(int locationId, string originalAliases, string currentAliases)
        {
            var original = originalAliases?.Trim().Split(',').ToList();
            var current = currentAliases?.Trim().Split(',').Distinct().ToList();
            var added = current?.Except(original ?? new List<string>()).ToList();
            var removed = original?.Except(current ?? new List<string>()).ToList();

            if (removed?.Count > 0)
            {
                foreach (var alias in removed)
                {
                    var locationAlias = await Db.LocationAliases
                                                  .Where(x => x.Name.Trim().ToLower() == alias.Trim().ToLower())
                                                  .FirstOrDefaultAsync();
                    if (locationAlias != null)
                    {
                        Db.Entry(locationAlias).State = EntityState.Deleted;
                    }
                }

                await Db.SaveChangesAsync();
            }

            if (added?.Count > 0)

                foreach (var alias in added)
                {
                    var locationAlias = await Db.LocationAliases
                                                  .Where(x => x.Name.Trim().ToLower() == alias.Trim().ToLower())
                                                  .FirstOrDefaultAsync();

                    if (locationAlias == null)
                    {
                        LocationAlias la = new LocationAlias()
                        {
                            Name = alias.Trim(),
                            LocationId = locationId
                        };

                        Db.LocationAliases.Add(la);

                    }
                }

            await Db.SaveChangesAsync();
            }

        
        }
}