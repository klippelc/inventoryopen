using Inventory.Data.Common;
using System.ComponentModel;

namespace Inventory.Data.Models
{
    public class UserRole : BaseEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int RoleId { get; set; }

        public Role Role { get; set; }

        [DefaultValue("false")]
        public bool IsDeleted { get; set; }
    }
}