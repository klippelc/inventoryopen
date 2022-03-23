using Inventory.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class City : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        public int? StateId { get; set; }

        [ForeignKey("StateId")]
        public State State { get; set; }

        [DefaultValue("true")]
        public bool Active { get; set; }

        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<Building> Buildings { get; set; }
    }
}