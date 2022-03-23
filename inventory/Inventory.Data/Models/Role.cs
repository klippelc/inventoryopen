using Inventory.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Models
{
    public class Role : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string Definition { get; set; }

        [DefaultValue("true")]
        public bool Active { get; set; }

        public int Sequence { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}