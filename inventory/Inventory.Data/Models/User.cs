using Inventory.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class User : BaseEntity
    {
        public static object Identity { get; set; }

        public int Id { get; set; }

        [MaxLength(50)]
        public string EmployeeId { get; set; }

        [MaxLength(500)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(20)]
        public string MobilePhone { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(50)]
        public string UserName { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(50)]
        public string Division { get; set; }

        [MaxLength(50)]
        public string Park { get; set; }

        [MaxLength(500)]
        public string ManagerOUPath { get; set; }

        [MaxLength(50)]
        public string ManagerFirstName { get; set; }

        [MaxLength(50)]
        public string ManagerLastName { get; set; }

        public int? ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public User Manager { get; set; }

        [DefaultValue("true")]
        public bool Active { get; set; }

        public DateTime? InventoryLastLoginDate { get; set; }

        public DateTime? ADLastLogonDate { get; set; }

        [DefaultValue("false")]
        public bool AdminCreated { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        public virtual ICollection<User> Managers { get; set; }
        public virtual ICollection<Asset> Assets { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<AssetList> AssetLists { get; set; }
        public virtual ICollection<AssetUserList> AssetUserLists { get; set; }
        public virtual ICollection<UserAssetConfirmation> UserAssetConfirmations { get; set; }
    }
}