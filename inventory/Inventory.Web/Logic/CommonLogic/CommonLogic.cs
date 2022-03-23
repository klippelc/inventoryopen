using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inventory.Data.Services;
using Inventory.Web.Common;
using Inventory.Web.ViewModels;

namespace Inventory.Web.Logic
{
    public class CommonLogic : AssetBaseLogic, ICommonLogic
    {
        //Constructor
        public CommonLogic(InventoryDbContext db)
        {
            Db = db;

            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var LoginUserLocId = Convert.ToInt32(prinicpal.Claims.Where(c => c.Type == CustomClaimTypes.LocationId).Select(c => c.Value).SingleOrDefault());
            var Roles = prinicpal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            var claims = prinicpal.Claims.ToList();
        }

        //Check Name
        public async Task<bool> NameExist(string name, string itemType, int manuId = 0, int locationId = 0, int buildingId = 0, int userId = 0)
        {
            dynamic item;

            name = name?.Trim().ToLower();

            if (string.IsNullOrEmpty(name))
                return false;


            if (itemType == "Hardware")
            {
                item = await Db.Assets.Where(x => x.IsDeleted == false && x.Name.Trim().ToLower() == name).FirstOrDefaultAsync();
            }
            else if (itemType == "Manufacturer")
            {
                item = await Db.Manufacturers.Where(x => x.IsDeleted == false && (x.Name.Trim().ToLower() == name || x.DisplayName.Trim().ToLower() == name)).FirstOrDefaultAsync();
            }

            else if (itemType == "Product")
            {
                item = await Db.Products
                               .Where(x => x.IsDeleted == false 
                                      && x.ManuId == manuId && (x.Name.Trim().ToLower() == name 
                                          || x.DisplayName.Trim().ToLower() == name))
                                          .FirstOrDefaultAsync();
            }
            else if (itemType == "Supplier")
            {
                item = await Db.Suppliers.Where(x => x.IsDeleted == false && (x.Name.Trim().ToLower() == name || x.DisplayName.Trim().ToLower() == name)).FirstOrDefaultAsync();
            }
            else if (itemType == "Location")
            {
                item = await Db.Locations
                               .Include(x => x.LocationAliases)
                               .Where(x => x.Id != locationId 
                                      && (x.DisplayName != null && x.DisplayName != "" && x.DisplayName.ToLower().Trim() == name)
                                      || (x.Name != null && x.Name != "" && x.Name.ToLower() == name)
                                      || (x.LocationAliases.Any(l => l.LocationId != locationId && l.Name != null && l.Name != "" && l.Name.Trim().ToLower() == name)))
                               .FirstOrDefaultAsync();

            }
            else if (itemType == "Building")
            {
                item = await Db.Buildings.Where(x => x.IsDeleted == false && x.LocationId == locationId 
                                                &&(x.Name.Trim().ToLower() == name || x.DisplayName.Trim().ToLower() == name))
                                         .FirstOrDefaultAsync();
            }
            else if (itemType == "Room")
            {
                item = await Db.Rooms.Where(x => x.IsDeleted == false && x.BuildingId == buildingId 
                                            && (x.Name.Trim().ToLower() == name || x.DisplayName.Trim().ToLower() == name))
                                     .FirstOrDefaultAsync();
            }
            else if (itemType == "AssetList")
            {
                item = await Db.AssetLists.Where(x => x.UserId == userId && x.Name.Trim().ToLower() == name).FirstOrDefaultAsync();
            }
            else
            {
                return true;
            }

            if (item != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Get Asset Types
        public async Task<IEnumerable<AssetTypeVM>> GetAssetTypes()
        {
            var vmTypeList = new List<AssetTypeVM>();
            var typeList = await Db.AssetTypes.Where(x => x.Active == true).OrderBy(r => r.Name).ToListAsync();

            foreach (var item in typeList)
            {
                var vmType = item != null ? new AssetTypeVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Active = item.Active
                } : new AssetTypeVM();
                vmTypeList.Add(vmType);
            };
            return vmTypeList.OrderBy(x => x.Name);
        }

        //Delete
        public async Task<bool> IsDeleted(int? Id, string itemType)
        {
            dynamic item;

            itemType = itemType?.Trim().ToLower();
            Id = Id ?? 0;

            if (itemType == "manufacturer")
            {
                item = await Db.Manufacturers
                               .Where(x => x.Id == Id && x.IsDeleted == true)
                               .FirstOrDefaultAsync();
            }
            else if (itemType == "product")
            {
                item = await Db.Products
                               .Where(x => x.Id == Id && x.IsDeleted == true)
                               .FirstOrDefaultAsync();
            }
            else if (itemType == "supplier")
            {
                item = await Db.Suppliers
                               .Where(x => x.Id == Id && x.IsDeleted == true)
                               .FirstOrDefaultAsync();
            }
            else if (itemType == "location")
            {
                item = await Db.Locations
                               .Where(x => x.Id == Id && x.IsDeleted == true)
                               .FirstOrDefaultAsync();
            }
            else if (itemType == "building")
            {
                item = await Db.Buildings
                               .Where(x => x.Id == Id && x.IsDeleted == true)
                               .FirstOrDefaultAsync();
            }
            else if (itemType == "room")
            {
                item = await Db.Rooms
                               .Where(x => x.Id == Id && x.IsDeleted == true)
                               .FirstOrDefaultAsync();
            }
            else if (itemType == "asset")
            {
                item = await Db.Assets
                               .Where(x => x.Id == Id && x.IsDeleted == true)
                               .FirstOrDefaultAsync();
            }

            else
            {
                return true;
            }


            if (item != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Get Cities
        public async Task<IEnumerable<CityVM>> GetCities(string stateCode)
        {

            var vmCityList = new List<CityVM>();

            var query = Db.Cities.Where(x => x.Active == true);

            if (!string.IsNullOrEmpty(stateCode))
            {
                var StateCode = stateCode.RemoveSpaces()?.ToLower();
                query = query.Where(x => x.State.Code.ToLower() == StateCode);
            }

            var cities = await query.OrderBy(r => r.Name).ToListAsync();

            foreach (var item in cities)
            {
                var vmCity = item != null ? new CityVM()
                {

                    Id = item.Id,
                    Name = item.Name,
                    Active = item.Active

                } : new CityVM();

                vmCityList.Add(vmCity);
            };
            return vmCityList.OrderBy(x => x.Name);
        }

        //Get States
        public async Task<IEnumerable<StateVM>> GetStates(string stateCode)
        {

            var vmStateList = new List<StateVM>();

            var query = Db.States.Where(x => x.Active == true);

            if (!string.IsNullOrEmpty(stateCode))
            {
                var StateCode = stateCode.RemoveSpaces()?.ToLower();
                query = query.Where(x => x.Code.ToLower() == StateCode);
            }

            var states = await query.OrderBy(r => r.Name).ToListAsync();

            foreach (var item in states)
            {
                var vmState = item != null ? new StateVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Code = item.Code,
                    Active = item.Active

                } : new StateVM();

                vmStateList.Add(vmState);
            };
            return vmStateList.OrderBy(x => x.Name);
        }

    }
}