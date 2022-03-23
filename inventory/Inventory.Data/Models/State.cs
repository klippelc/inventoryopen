using Inventory.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Models
{
    public class State : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(10)]
        public string Code { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [DefaultValue("true")]
        public bool Active { get; set; }

        public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<Building> Buildings { get; set; }
        public virtual ICollection<Supplier> Suppliers { get; set; }
    }
}