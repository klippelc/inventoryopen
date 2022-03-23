using PagedList;
using Inventory.Data.Models;
using Inventory.Data.Services;
using Inventory.Web.Common;
using Inventory.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Inventory.Web.Logic
{
    public class AssetListLogic : AssetBaseLogic, IAssetListLogic
    {

        //Constructor
        public AssetListLogic(InventoryDbContext db, ICommonLogic commonLogic)
        {
            Db = db;
            CommonLogic = commonLogic;
        }

        //Get List
        public async Task<IEnumerable<AssetListVM>> GetList(int userId, string sortOrder = "", string currentFilter = "", string searchString = "", int page = 1, int pageSize = 10000000)
        {
            int pageNumber = page;
            var vmList = new List<AssetListVM>();

            var query = Db.AssetLists
                          .Include(x => x.AssetType)
                          .Include(x => x.AssetUserLists)
                          .Where(x => x.UserId == userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => x.Name.Contains(searchString) || x.Description.Contains(searchString));
            }

            var List = await query.ToListAsync();

            foreach (var item in List)
            {
                var vm = new AssetListVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    AssetTypeName = item.AssetType.Name,
                    Shared = item.Shared ?? false,
                    ItemCount = item.AssetUserLists.Count(),
                };
                vmList.Add(vm);
            };

            switch (sortOrder)
            {
                case "id":
                    vmList = vmList.OrderBy(s => s.Id).ToList();
                    break;

                case "id_desc":
                    vmList = vmList.OrderByDescending(s => s.Id).ToList();
                    break;

                case "name":
                    vmList = vmList.OrderBy(s => s.Name).ToList();
                    break;

                case "name_desc":
                    vmList = vmList.OrderByDescending(s => s.Name).ToList();
                    break;

                case "description":
                    vmList = vmList.OrderBy(s => s.Description).ToList();
                    break;

                case "description_desc":
                    vmList = vmList.OrderByDescending(s => s.Description).ToList();
                    break;

                case "assettype":
                    vmList = vmList.OrderBy(s => s.AssetTypeName).ToList();
                    break;

                case "assettype_desc":
                    vmList = vmList.OrderByDescending(s => s.AssetTypeName).ToList();
                    break;

                case "shared":
                    vmList = vmList.OrderBy(s => s.Shared).ToList();
                    break;

                case "shared_desc":
                    vmList = vmList.OrderByDescending(s => s.Shared).ToList();
                    break;

                case "itemcount":
                    vmList = vmList.OrderBy(s => s.ItemCount).ToList();
                    break;

                case "itemcount_desc":
                    vmList = vmList.OrderByDescending(s => s.ItemCount).ToList();
                    break;

                default:
                    vmList = vmList.OrderBy(s => s.Name).ToList();
                    break;
            }

            var items = vmList.ToPagedList(pageNumber, pageSize);

            StaticPagedList<AssetListVM> list = new StaticPagedList<AssetListVM>(items, items.PageNumber, items.PageSize, items.TotalItemCount);

            return list;
        }

        //Get Blank Asset List
        public async Task<AssetListVM> GetBlankAssetList()
        {
            var vm = new AssetListVM
            {
                Shared = false,
                AssetTypes = await CommonLogic.GetAssetTypes()
            };

            return vm;
        }

        //Create
        public async Task<int> Create(AssetListVM vm)
        {
            var item = new AssetList()
            {
                Name = vm.Name?.Trim(),
                Description = vm.Description?.Trim(),
                AssetTypeId = vm.AssetTypeId,
                UserId = vm.UserId,
                Shared = vm.Shared,
                CreatedBy = vm.UserId,
                ModifiedBy = vm.UserId
            };

            var result = Db.AssetLists.Add(item);
            await Db.SaveChangesAsync();

            return result.Id;
        }

        //Get
        public async Task<AssetListVM> Get(int Id, int userId = 0)
        {
            var query = Db.AssetLists
                          .Include(x => x.AssetType)
                          .Include(x => x.AssetUserLists)
                          .Where(x => x.Id == Id);

            if (userId > 0)
            {
                query = query.Where(x => x.UserId == userId);
            }

            var item = await query.FirstOrDefaultAsync();

            if (item == null)
            {
                return null;
            }

            var vm = new AssetListVM()
            {
                Id = item.Id,
                Name = item.Name,
                UserId = item.UserId,
                OriginalName = item.Name,
                Description = item.Description,
                OriginalDescription = item.Description,
                AssetTypeId = item.AssetTypeId,
                OriginalAssetTypeId = item.AssetTypeId,
                AssetTypeName = item.AssetType.Name,
                Shared = item.Shared ?? false,
                ItemCount = item.AssetUserLists.Count(),
            };

            vm.AssetTypes = await CommonLogic.GetAssetTypes();
            Db.Entry(item).State = EntityState.Detached;

            return vm;
        }

        //Save
        public async Task Save(AssetListVM vm)
        {
            var assetList = await Get(vm.Id);

            var assetTypeId = ((assetList.ItemCount == 0) || (assetList.AssetTypeId == vm.AssetTypeId)) ? vm.AssetTypeId : assetList.AssetTypeId;
            
            var item = new AssetList()
            {
                Id = vm.Id,
                Name = vm.Name?.Trim(),
                Description = vm.Description?.Trim(),
                AssetTypeId = assetTypeId,
                Shared = vm.Shared,
                ModifiedBy = vm.ModifiedBy
            };

            var entry = Db.Entry(item);
            entry.State = EntityState.Modified;
            Db.Entry(item).Property(x => x.CreatedBy).IsModified = false;
            Db.Entry(item).Property(x => x.UserId).IsModified = false;
            Db.Entry(item).Property(x => x.DateAdded).IsModified = false;

            await Db.SaveChangesAsync();
        }

        //Delete
        public async Task Delete(AssetListVM vm)
        {
            var item = await Db.AssetLists
                               .Include(x => x.AssetUserLists)
                               .Where(x => x.Id == vm.Id && x.UserId == vm.UserId)
                               .FirstOrDefaultAsync();

            if (item != null)
            {
                if (item.AssetUserLists.Count() > 0)
                {
                    await RemoveAssetsFromUserList(vm.Id, vm.UserId ?? 0);
                }

                Db.AssetLists.Remove(item);
                await Db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AssetListVM>> GetUserLists(int userId, string assetType, bool showShared = false)
        {
            var list = new List<AssetListVM>();

            var query = Db.AssetLists
                          .Include(x => x.User)
                          .Include(x => x.AssetType);

            if (showShared == true)
            {
                query = query.Where(x => x.UserId == userId || x.Shared == true);
            }
            else 
            {
                query = query.Where(x => x.UserId == userId);
            }

            if (!string.IsNullOrEmpty(assetType))
            {
                query = query.Where(x => x.AssetType.Name.ToLower() == assetType.ToLower());
            }

            var AssetUserLists = await query.ToListAsync();

            foreach (var item in AssetUserLists)
            {
                var vm = new AssetListVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Shared = item.Shared,
                    UserId = item.UserId,
                    IsOwner = item.UserId == userId,
                    UserFullName = item?.User?.FirstName + " " + item?.User?.LastName,
                    ItemCount = 0
                };

                list.Add(vm);
            }

            return list;
        }

        public async Task<IEnumerable<AssetListVM>> GetUserList(int listId, int userId = 0)
        {
            var list = new List<AssetListVM>();

            var query = Db.AssetLists.Where(x => x.Id == listId);

            if (userId > 0)
            {
                query = query.Where(x => x.UserId == userId);
            }

            var AssetUserLists = await query.ToListAsync();

            foreach (var item in AssetUserLists)
            {
                var vm = new AssetListVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    UserId = item.UserId,
                    ItemCount = 0
                };

                list.Add(vm);
            }

            return list;
        }

        public async Task<IEnumerable<AssetUserListVM>> GetAssetUserList(int listId)
        {
            var list = new List<AssetUserListVM>();

            var query = Db.AssetUserLists
                          .Include(x => x.AssetList)
                          .Where(x => x.AssetListId == listId);

            var AssetUserLists = await query.ToListAsync();

            foreach (var item in AssetUserLists)
            {
                var vm = new AssetUserListVM()
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    AssetId = item.AssetId
                };

                list.Add(vm);
            }

            return list;
        }

        public async Task<bool> AddAssetsToUserList(AssetListVM vm)
        {
            var assetIds = vm.AssetIds.Split(',').Select(Int32.Parse).ToList();

            //Remove wrong assettypes in list.
            //Check if its a shared list

            var userAssetIds = await Db.AssetUserLists
                                       .Where(x => x.AssetListId == vm.Id && x.UserId == vm.UserId && assetIds.Contains(x.AssetId ?? 0))
                                       .Select(x => x.AssetId)
                                       .ToListAsync();

            var assetList = await Db.Assets
                                    .Where(x => x.IsDeleted == false
                                           && x.InvoiceItem.AssetType.Name.ToLower() == vm.AssetTypeName.ToLower()
                                           && assetIds.Contains(x.Id)
                                           && !userAssetIds.Contains(x.Id))
                                    .ToListAsync();

            AssetUserList au;

            foreach (var item in assetList)
            {
                au = new AssetUserList()
                {
                    AssetListId = vm.Id,
                    AssetId = item.Id,
                    UserId = vm.UserId,
                    CreatedBy = vm.UserId,
                    ModifiedBy = vm.UserId
                };

                Db.AssetUserLists.Add(au);
            }

            await Db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveAssetsFromUserList(AssetListVM vm)
        {
            var assetIds = vm.AssetIds.Split(',').Select(Int32.Parse).ToList();

            var userAssets = await Db.AssetUserLists
                                       .Where(x => x.AssetListId == vm.Id && x.UserId == vm.UserId && assetIds.Contains(x.AssetId ?? 0))
                                       .ToListAsync();

            foreach (var item in userAssets)
            {
                Db.AssetUserLists.Remove(item);
            }

            await Db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveAssetsFromUserList(int listId, int userId = 0)
        {
            var query = Db.AssetUserLists
                          .Where(x => x.AssetListId == listId);

            if (userId > 0)
            {
                query = query.Where(x => x.AssetList.UserId == userId);
            }

            var userAssets = await query.ToListAsync();

            foreach (var item in userAssets)
            {
                Db.AssetUserLists.Remove(item);
            }

            await Db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveAssetFromAllList(int assetId)
        {
            var userAssets = await Db.AssetUserLists.Where(x => x.AssetId == assetId).ToListAsync();

            foreach (var item in userAssets)
            {
                Db.AssetUserLists.Remove(item);
            }

            await Db.SaveChangesAsync();

            return true;
        }

    }
}