using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Web.ViewModels
{
    public class UserVM
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string EmployeeId { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        [Display(Name = "Mobile Phone")]
        public string MobilePhone { get; set; }

        public string Title { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name { get; set; }

        public string LastNameFirstInitial { get; set; }

        public string Initials { get; set; }

        public string LastNameFirstName { get; set; }

        public string Division { get; set; }

        public string Park { get; set; }

        public int? LocationId { get; set; }

        public bool Active { get; set; }

        [Display(Name = "Inv Last Login")]
        public DateTime? InventoryLastLoginDate { get; set; }

        [Display(Name = "AD Last Logon")]
        public DateTime? ADLastLogonDate { get; set; }

        public string ManagerOUPath { get; set; }

        public string ManagerName { get; set; }

        public int? ManagerId { get; set; }

        [Display(Name = "Manager")]
        public UserVM Manager { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public bool AdminCreated { get; set; }

        public IEnumerable<int> UserRoleIds { get; set; }

        public string OriginalUserRoleIds { get; set; }

        public string PreviousUrl { get; set; }

        [Display(Name = "Roles")]
        public IEnumerable<RoleVM> UserRoles { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }

        //
        public string UserRoleNames { get; set; }
        public string NameWithUserName { get; set; }

        //
        public IEnumerable<UserVM> UsersManaged { get; set; }
        public IEnumerable<RoleVM> Roles { get; set; }
    }
}