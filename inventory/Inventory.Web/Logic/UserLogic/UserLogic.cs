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
    public class UserLogic : AssetBaseLogic, IUserLogic
    {
        //Constructor
        public UserLogic(InventoryDbContext db)
        {
            Db = db;
        }

        //Get List
        public async Task<IEnumerable<UserVM>> GetList(string sortOrder = "", string currentFilter = "", string searchString = "", string assetType = "", int page = 1, int pageSize = 10000000)
        {
            int pageNumber = page;
            var vmList = new List<UserVM>();

            //
            var query = Db.Users.Include(x => x.Manager).AsQueryable();
            //

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => x.UserName.Contains(searchString) 
                                      || x.LastName.Contains(searchString) 
                                      || x.FirstName.Contains(searchString)
                                      || (x.Manager != null && x.Manager.LastName.Contains(searchString))
                                      || (x.Manager != null && x.Manager.FirstName.Contains(searchString))
                                      || (x.ManagerLastName.Contains(searchString))
                                      || (x.ManagerFirstName.Contains(searchString))
                                      || x.Title.Contains(searchString)
                                      || x.Park.Contains(searchString)
                                      || x.MobilePhone.Contains(searchString)
                                      || x.Phone.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "id":
                    query = query.OrderBy(s => s.Id);
                    break;

                case "id_desc":
                    query = query.OrderByDescending(s => s.Id);
                    break;

                case "name":
                    query = query.OrderBy(s => s.LastName ?? "zzz")
                                 .ThenBy(s => s.FirstName ?? "");
                    break;

                case "name_desc":
                    query = query.OrderByDescending(s => s.LastName ?? "")
                                 .ThenBy(s => s.FirstName ?? "");
                    break;

                case "username":
                    query = query.OrderBy(s => s.UserName ?? "zzz");
                    break;

                case "username_desc":
                    query = query.OrderByDescending(s => s.UserName ?? "");
                    break;

                case "manager":
                    query = query.OrderBy(s => s.Manager != null ? s.Manager.LastName ?? "zzz" : s.ManagerLastName ?? "zzz");
                    break;

                case "manager_desc":
                    query = query.OrderByDescending(s => s.Manager != null ? s.Manager.LastName ?? "" : s.ManagerLastName ?? "");
                    break;

                case "title":
                    query = query.OrderBy(s => s.Title ?? "zzz")
                                 .ThenBy(s => s.LastName ?? "zzz");
                    break;

                case "title_desc":
                    query = query.OrderByDescending(s => s.Title ?? "")
                                 .ThenBy(s => s.LastName ?? "");
                    break;

                case "park":
                    query = query.OrderBy(s => s.Park ?? "zzz")
                                 .ThenBy(s => s.LastName ?? "zzz");
                    break;

                case "park_desc":
                    query = query.OrderByDescending(s => s.Park ?? "")
                                 .ThenBy(s => s.LastName ?? "");
                    break;

                case "phone":
                    query = query.OrderBy(s => s.Phone ?? "zzz")
                                 .ThenBy(s => s.LastName ?? "zzz");
                    break;

                case "phone_desc":
                    query = query.OrderByDescending(s => s.Phone ?? "")
                                 .ThenBy(s => s.LastName ?? "");
                    break;

                case "mobile":
                    query = query.OrderBy(s => s.MobilePhone ?? "zzz")
                                 .ThenBy(s => s.LastName ?? "zzz");
                    break;

                case "mobile_desc":
                    query = query.OrderByDescending(s => s.MobilePhone ?? "")
                                 .ThenBy(s => s.LastName ?? "");
                    break;

                case "lastlogin":
                    query = query.OrderBy(s => s.InventoryLastLoginDate)
                                 .ThenBy(s => s.LastName ?? "zzz");
                    break;

                case "lastlogin_desc":
                    query = query.OrderByDescending(s => s.InventoryLastLoginDate)
                                 .ThenBy(s => s.LastName ?? "");
                    break;

                case "lastlogon":
                    query = query.OrderBy(s => s.ADLastLogonDate)
                                 .ThenBy(s => s.LastName ?? "zzz");
                    break;

                case "lastlogon_desc":
                    query = query.OrderByDescending(s => s.ADLastLogonDate)
                                 .ThenBy(s => s.LastName ?? "");
                    break;

                default:
                    query = query = query.OrderBy(s => s.LastName ?? "zzz").ThenBy(s => s.FirstName ?? "zzz");

                    break;
            }

            var List = await query.ToPagedListAsync(page, pageSize);

            var items = (IPagedList)List;

            foreach (var item in List)
            {
                var vm = new UserVM()
                {
                    Id = item.Id,
                    UserId = item.Id,
                    Title = item.Title,
                    Phone = item.Phone.ToPhoneFormat(),
                    MobilePhone = item.MobilePhone.ToPhoneFormat(),
                    Park = item.Park,
                    UserName = item.UserName,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Name = item.LastName + ", " + item.FirstName,
                    Active = item.Active,
                    InventoryLastLoginDate = item.InventoryLastLoginDate,
                    ADLastLogonDate = item.ADLastLogonDate,

                    ManagerName = !string.IsNullOrEmpty(item.ManagerLastName) ? item.ManagerLastName + ", " + item.ManagerFirstName : null,
                    ManagerId = item.ManagerId,
                    Manager = item.Manager != null ? new UserVM()
                    {
                        Id = item.Manager.Id,
                        UserId = item.Manager.Id,
                        Title = item.Manager.Title,
                        UserName = item.Manager.UserName,
                        FirstName = item.Manager.FirstName,
                        LastName = item.Manager.LastName,
                        Name = item.Manager.LastName + ", " + item.Manager.FirstName,
                        NameWithUserName = item.Manager.LastName + ", " + item.Manager.FirstName + " (" + item.Manager.UserName + ")",
                        Phone = item.Manager.Phone.ToPhoneFormat(),
                        MobilePhone = item.Manager.MobilePhone.ToPhoneFormat(),
                        Email = item.Manager.Email,
                        DateAdded = item.Manager.DateAdded,
                        AdminCreated = item.Manager.AdminCreated,
                        Active = item.Manager.Active

                    } : new UserVM(),
                };
                vmList.Add(vm);
            };

            StaticPagedList<UserVM> list = new StaticPagedList<UserVM>(vmList, items.PageNumber, items.PageSize, items.TotalItemCount);

            return list;
        }

        //Get Users
        public async Task<IEnumerable<UserVM>> GetUsers()
        {
            var vmUserList = new List<UserVM>();

            var userList = await Db.Users
                                   .Include(x => x.Manager)
                                   .Where(x => x.Active == true)
                                   .OrderBy(r => r.UserName)
                                   .ToListAsync();

            foreach (var item in userList)
            {
                var vmUser = item != null ? new UserVM()
                {
                    Id = item.Id,
                    UserId = item.Id,
                    Title = item.Title,
                    UserName = item.UserName,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Name = item.LastName + ", " + item.FirstName,
                    NameWithUserName = item.LastName + ", " + item.FirstName + " (" + item.UserName + ")",
                    Phone = item.Phone.ToPhoneFormat(),
                    MobilePhone = item.MobilePhone.ToPhoneFormat(),
                    Email = item.Email,
                    DateAdded = item.DateAdded,
                    AdminCreated = item.AdminCreated,
                    Active = item.Active,
                    InventoryLastLoginDate = item.InventoryLastLoginDate,
                    ADLastLogonDate = item.ADLastLogonDate,

                    ManagerName = !string.IsNullOrEmpty(item.ManagerLastName) ? item.ManagerLastName + ", " + item.ManagerFirstName : null,
                    ManagerId = item.ManagerId,
                    Manager = item.Manager != null ? new UserVM()
                    {
                        Id = item.Manager.Id,
                        UserId = item.Manager.Id,
                        Title = item.Manager.Title,
                        UserName = item.Manager.UserName,
                        FirstName = item.Manager.FirstName,
                        LastName = item.Manager.LastName,
                        Name = item.Manager.LastName + ", " + item.Manager.FirstName,
                        NameWithUserName = item.Manager.LastName + ", " + item.Manager.FirstName + " (" + item.Manager.UserName + ")",
                        Phone = item.Manager.Phone.ToPhoneFormat(),
                        MobilePhone = item.Manager.MobilePhone.ToPhoneFormat(),
                        Email = item.Manager.Email,
                        DateAdded = item.Manager.DateAdded,
                        AdminCreated = item.Manager.AdminCreated,
                        Active = item.Manager.Active

                    } : new UserVM(),

                } : new UserVM();
                vmUserList.Add(vmUser);
            };
            return vmUserList.OrderBy(x => x.LastName);
        }

        //Get User and their Roles
        public async Task<UserVM> GetUserRoles(string userName)
        {
            userName = userName.RemoveSpaces();

            var user = await Db.Users.AsNoTracking()
                               .Include(u => u.UserRoles.Select(r => r.Role))
                               .Where(u => u.UserName == userName)
                               .FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            var defaultLocationId = await GetUserDefaultLocation(user.Id);

            var vmUser = user != null ? new UserVM()
            {
                UserId = user.Id,
                Title = user.Title,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Name = user.LastName + ", " + user.FirstName,
                Initials = ((user.FirstName?.Substring(0, 1)) + (user.LastName?.Substring(0, 1)))?.ToUpper(),
                Phone = user.Phone.ToPhoneFormat(),
                MobilePhone = user.MobilePhone.ToPhoneFormat(),
                Email = user.Email,
                Division = user.Division,
                Park = user.Park,
                LocationId = defaultLocationId?.Id,
                DateAdded = user.DateAdded,
                AdminCreated = user.AdminCreated,
                Active = user.Active,
                InventoryLastLoginDate = user.InventoryLastLoginDate,
                ADLastLogonDate = user.ADLastLogonDate,

                UserRoles = user.UserRoles != null ? user.UserRoles.Where(x => x.IsDeleted == false
                                                                          && x.Role != null
                                                                          && x.UserId == user.Id)
                .Select(p => new RoleVM
                {
                    Id = p.Role.Id,
                    Name = p.Role.Name,
                    Description = p.Role.Description,
                    Sequence = p.Role.Sequence,
                }).ToList() : new List<RoleVM>()
            } : null;

            return vmUser;
        }

        //Get
        public async Task<UserVM> Get(int Id)
        {
            var vmUsersManagedList = new List<UserVM>();

            var item = await Db.Users
                               .Include(x => x.UserRoles.Select(r => r.Role))
                               .Include(x => x.Manager)
                               .Where(x => x.Id == Id)
                               .FirstOrDefaultAsync();

            if (item == null)
            {
                return null;
            }

            var usersManagedList = await Db.Users
                                           .Where(x => x.ManagerId == item.Id)
                                           .ToListAsync();

            var vm = new UserVM()
            {
                Id = item.Id,
                UserId = item.Id,
                Email = item.Email,
                Park = item.Park,
                Phone = item.Phone.ToPhoneFormat(),
                Title = item.Title,
                UserName = item.UserName,
                Division = item.Division,
                MobilePhone = item.MobilePhone.ToPhoneFormat(),
                Name = item.FirstName + " " + item.LastName,
                Initials = ((item.FirstName?.Substring(0, 1)) + (item.LastName ?.Substring(0, 1)))?.ToUpper(),
                Active = item.Active,
                InventoryLastLoginDate = item.InventoryLastLoginDate,
                ADLastLogonDate = item.ADLastLogonDate,
                Notes = item.Notes,

                ManagerName = !string.IsNullOrEmpty(item.ManagerLastName) ? item.ManagerLastName + ", " + item.ManagerFirstName : null,

                ManagerOUPath = item.ManagerOUPath,
                ManagerId = item.ManagerId,
                Manager = item.Manager != null ? new UserVM()
                {
                    Id = item.Manager.Id,
                    UserId = item.Manager.Id,
                    Title = item.Manager.Title,
                    UserName = item.Manager.UserName,
                    FirstName = item.Manager.FirstName,
                    LastName = item.Manager.LastName,
                    Name = item.Manager.LastName + ", " + item.Manager.FirstName,
                    NameWithUserName = item.Manager.LastName + ", " + item.Manager.FirstName + " (" + item.Manager.UserName + ")",
                    Phone = item.Manager.Phone.ToPhoneFormat(),
                    MobilePhone = item.Manager.MobilePhone.ToPhoneFormat(),
                    Email = item.Manager.Email,
                    DateAdded = item.Manager.DateAdded,
                    AdminCreated = item.Manager.AdminCreated,
                    Active = item.Manager.Active,
                    InventoryLastLoginDate = item.InventoryLastLoginDate,
                    ADLastLogonDate = item.ADLastLogonDate

                } : new UserVM(),

                UserRoles = item.UserRoles != null ? item.UserRoles.Where(x => x.IsDeleted == false && x.Role != null && x.UserId == item.Id)
                .Select(p => new RoleVM
                {
                    Id = p.Role.Id,
                    Name = p.Role.Name,
                    Description = p.Role.Description,
                    Sequence = p.Role.Sequence,
                    IsChecked = true,
                    Checked = "Checked",
                    Active = p.Role.Active
                }).OrderBy(x => x.Sequence).ToList() : new List<RoleVM>()

            };

            foreach (var user in usersManagedList)
            {
                var vmUser = new UserVM()
                {
                    Id = user.Id,
                    UserId = user.Id,
                    Title = user.Title,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Name = user.FirstName + " " + user.LastName,
                    NameWithUserName = user.FirstName + " " + user.LastName + " (" + user.UserName + ")",
                    Phone = user.Phone.ToPhoneFormat(),
                    MobilePhone = user.MobilePhone.ToPhoneFormat(),
                    Email = user.Email,
                    DateAdded = user.DateAdded,
                    AdminCreated = user.AdminCreated,
                    Active = user.Active,
                    InventoryLastLoginDate = item.InventoryLastLoginDate,
                    ADLastLogonDate = item.ADLastLogonDate
                };
                vmUsersManagedList.Add(vmUser);
            }

            vm.UsersManaged = vmUsersManagedList;
            vm.UserRoleIds = vm.UserRoles?.Select(x => x.Id).ToList();
            vm.OriginalUserRoleIds = vm.UserRoles?.Select(x => x.Id).ToList().ListToString();
            vm.UserRoleNames = vm.UserRoles?.Select(x => x.Description).ToList().ListToString();

            return vm;
        }

        //Save
        public async Task Save(UserVM vm)
        {
            // Roles
            var originalIds = vm.OriginalUserRoleIds.StringToIntList();
            var removed = originalIds?.Except(vm.UserRoleIds ?? new List<int>()).ToList();
            var added = vm.UserRoleIds?.Except(originalIds ?? new List<int>()).ToList();

            foreach (var roleId in removed ?? new List<int>())
            {
                var userRole = await Db.UserRoles
                                        .Where(x => x.IsDeleted == false && x.UserId == vm.Id && x.RoleId == roleId)
                                        .FirstOrDefaultAsync();
                if (userRole != null)
                {
                    userRole.IsDeleted = true;
                    userRole.ModifiedBy = vm.ModifiedBy;
                    Db.Entry(userRole).State = EntityState.Modified;
                }
            }

            foreach (var roleId in added ?? new List<int>())
            {
                var userRole = await Db.UserRoles
                                        .Where(x => x.IsDeleted == false && x.UserId == vm.Id && x.RoleId == roleId)
                                        .FirstOrDefaultAsync();
                if (userRole == null)
                {
                    UserRole ur = new UserRole()
                    {
                        UserId = vm.Id,
                        RoleId = roleId,
                        IsDeleted = false,
                        CreatedBy = vm.ModifiedBy,
                        ModifiedBy = vm.ModifiedBy
                    };
                    Db.UserRoles.Add(ur);
                }
            }
            //

            var item = new User()
            {
                Id = vm.Id,
                MobilePhone = vm.MobilePhone,
                Notes = vm.Notes?.Trim(),
                ModifiedBy = vm.ModifiedBy
            };

            Db.Entry(item).State = EntityState.Modified;
            Db.Entry(item).Property(x => x.EmployeeId).IsModified = false;
            Db.Entry(item).Property(x => x.ManagerId).IsModified = false;
            Db.Entry(item).Property(x => x.ManagerOUPath).IsModified = false;
            Db.Entry(item).Property(x => x.ManagerFirstName).IsModified = false;
            Db.Entry(item).Property(x => x.ManagerLastName).IsModified = false;
            Db.Entry(item).Property(x => x.Active).IsModified = false;
            Db.Entry(item).Property(x => x.InventoryLastLoginDate).IsModified = false;
            Db.Entry(item).Property(x => x.ADLastLogonDate).IsModified = false;
            Db.Entry(item).Property(x => x.Email).IsModified = false;
            Db.Entry(item).Property(x => x.Phone).IsModified = false;
            Db.Entry(item).Property(x => x.Title).IsModified = false;
            Db.Entry(item).Property(x => x.UserName).IsModified = false;
            Db.Entry(item).Property(x => x.FirstName).IsModified = false;
            Db.Entry(item).Property(x => x.LastName).IsModified = false;
            Db.Entry(item).Property(x => x.Division).IsModified = false;
            Db.Entry(item).Property(x => x.Park).IsModified = false;
            Db.Entry(item).Property(x => x.AdminCreated).IsModified = false;
            Db.Entry(item).Property(x => x.CreatedBy).IsModified = false;
            Db.Entry(item).Property(x => x.DateAdded).IsModified = false;
            await Db.SaveChangesAsync();
        }

        //Update Last Login Date
        public async Task UpdateLastLogin(int userId)
        {
            var item = new User()
            {
                Id = userId,
                InventoryLastLoginDate = DateTime.Now
            };

            Db.Entry(item).State = EntityState.Modified;
            Db.Entry(item).Property(x => x.EmployeeId).IsModified = false;
            Db.Entry(item).Property(x => x.Email).IsModified = false;
            Db.Entry(item).Property(x => x.Phone).IsModified = false;
            Db.Entry(item).Property(x => x.MobilePhone).IsModified = false;
            Db.Entry(item).Property(x => x.Title).IsModified = false;
            Db.Entry(item).Property(x => x.UserName).IsModified = false;
            Db.Entry(item).Property(x => x.FirstName).IsModified = false;
            Db.Entry(item).Property(x => x.LastName).IsModified = false;
            Db.Entry(item).Property(x => x.Division).IsModified = false;
            Db.Entry(item).Property(x => x.Park).IsModified = false;
            Db.Entry(item).Property(x => x.ManagerOUPath).IsModified = false;
            Db.Entry(item).Property(x => x.ManagerFirstName).IsModified = false;
            Db.Entry(item).Property(x => x.ManagerLastName).IsModified = false;
            Db.Entry(item).Property(x => x.ManagerId).IsModified = false;
            Db.Entry(item).Property(x => x.Active).IsModified = false;
            Db.Entry(item).Property(x => x.ADLastLogonDate).IsModified = false;
            Db.Entry(item).Property(x => x.AdminCreated).IsModified = false;
            Db.Entry(item).Property(x => x.Notes).IsModified = false;
            Db.Entry(item).Property(x => x.CreatedBy).IsModified = false;
            Db.Entry(item).Property(x => x.DateAdded).IsModified = false;
            await Db.SaveChangesAsync();
        }

        //Get Roles
        public async Task<IEnumerable<RoleVM>> GetRoles(IEnumerable<int> SelectedIds)
        {
            var roles = await Db.Roles
                                .Where(x => x.Active == true)
                                .OrderBy(r => r.Name).ToListAsync();

            var userRoleList = new List<RoleVM>();

            foreach (var item in roles)
            {
                var vmRole = item != null ? new RoleVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Sequence = item.Sequence,
                    Active = item.Active
                } : new RoleVM();

                if (SelectedIds != null && SelectedIds.Contains(vmRole.Id))
                {
                    vmRole.IsChecked = true;
                    vmRole.Checked = "Checked";
                }
                userRoleList.Add(vmRole);
            }

            return userRoleList.OrderBy(x => x.Sequence);
        }

        public async Task<LocationVM> GetUserDefaultLocation(int userId)
        {
            var userPark = await Db.Users
                                   .Where(x => x.Id == userId && x.Park != null && x.Park != "")
                                   .GroupBy(x => new { Park = x.Park.Trim().ToLower() })
                                   .Select(x => new { x.Key.Park })
                                   .Select(x => x.Park)
                                   .FirstOrDefaultAsync();

            if (userPark == null)
            {
                return new LocationVM();
            }

            var location = await Db.Locations
                                   .Include(x => x.LocationAliases)
                                   .Where(x => (x.IsDeleted == false)
                                          &&  ((x.Name != null && x.Name != "" && x.Name.Trim().ToLower() == userPark)
                                          ||   (x.DisplayName != null && x.DisplayName != "" && x.DisplayName.Trim().ToLower() == userPark)
                                          ||   (x.LocationAliases.Select(l => l.Name).Contains(userPark))))
                                   .OrderBy(x => x.DisplayName != null && x.DisplayName != "" ? x.DisplayName : x.Name ?? "")
                                   .ThenBy(x => x.Name)
                                   .FirstOrDefaultAsync();

            var vmLocation = location != null ? new LocationVM()
            {
                Id = location.Id,
                Name = !string.IsNullOrEmpty(location.DisplayName) ? location.DisplayName : location.Name ?? ""

            } : new LocationVM();

            return vmLocation;
        }

        //Get Users By Location
        public async Task<IEnumerable<UserVM>> GetUsersByLocation(int locationId)
        {
            var vmUserList = new List<UserVM>();

            var location = await Db.Locations
                                   .Where(x => x.Id == locationId) 
                                   .GroupBy(x => new { DisplayName = (x.DisplayName != null && x.DisplayName != "") ? x.DisplayName.Trim().ToLower() : "", Name = x.Name.Trim().ToLower() })
                                   .Select(x => new { x.Key.DisplayName, x.Key.Name })
                                   .Select(x => new { x.DisplayName, x.Name })
                                   .FirstOrDefaultAsync();

            var locationAliases = await Db.LocationAliases
                                          .Where(x => x.LocationId == locationId)
                                          .GroupBy(x => new { Name = (x.Name != null ? x.Name.Trim().ToLower() : "") })
                                          .Select(x => new { x.Key.Name })
                                          .Select(x => x.Name)
                                          .ToListAsync();

            var userList = await Db.Users
                                   .Include(x => x.Manager)
                                   .OrderBy(r => r.UserName)
                                   .Where(x => (x.Active == true && x.Park != null && x.Park != "")
                                    && ((x.Park.Trim().ToLower() == location.Name.ToString())
                                    || (x.Park.Trim().ToLower() == location.DisplayName.ToString())
                                    || (locationAliases.Contains(x.Park.ToLower()))))
                             .GroupBy(x => new { x.Id, x.Title, x.UserName, x.FirstName, x.LastName, x.Phone, x.MobilePhone, x.Email, x.ManagerId, x.Manager })
                             .Select(x => new { x.Key.Id, x.Key.Title, x.Key.UserName, x.Key.FirstName, x.Key.LastName, x.Key.Phone, x.Key.MobilePhone, x.Key.Email, x.Key.Manager, x.Key.ManagerId })
                             .ToListAsync();

            foreach (var item in userList)
            {
                var vmUser = item != null ? new UserVM()
                {
                    Id = item.Id,
                    UserId = item.Id,
                    Title = item.Title,
                    UserName = item.UserName,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Name = item.LastName + ", " + item.FirstName,
                    NameWithUserName = item.LastName + ", " + item.FirstName + " (" + item.UserName + ")",
                    Phone = item.Phone.ToPhoneFormat(),
                    MobilePhone = item.MobilePhone.ToPhoneFormat(),
                    Email = item.Email,

                    ManagerId = item.ManagerId,
                    Manager = item.Manager != null ? new UserVM()
                    {
                        Id = item.Manager.Id,
                        UserId = item.Manager.Id,
                        Title = item.Manager.Title,
                        UserName = item.Manager.UserName,
                        FirstName = item.Manager.FirstName,
                        LastName = item.Manager.LastName,
                        Name = item.Manager.LastName + ", " + item.Manager.FirstName,
                        NameWithUserName = item.Manager.LastName + ", " + item.Manager.FirstName + " (" + item.Manager.UserName + ")",
                        Phone = item.Manager.Phone.ToPhoneFormat(),
                        MobilePhone = item.Manager.MobilePhone.ToPhoneFormat(),
                        Email = item.Manager.Email,
                        DateAdded = item.Manager.DateAdded,
                        AdminCreated = item.Manager.AdminCreated,
                        Active = item.Manager.Active

                    } : new UserVM(),

                } : new UserVM();
                vmUserList.Add(vmUser);
            };
            return vmUserList.OrderBy(x => x.LastName);
        }
    }
}