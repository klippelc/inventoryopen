using Inventory.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class Supplier : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(150)]
        public string DisplayName { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string Url { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(150)]
        public string ContactName { get; set; }

        [MaxLength(255)]
        public string AddressLine1 { get; set; }

        [MaxLength(255)]
        public string AddressLine2 { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        public int? StateId { get; set; }

        [ForeignKey("StateId")]
        public State State { get; set; }

        [MaxLength(20)]
        public string PostalCode { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        [DefaultValue(true)]
        public bool Active { get; set; }

        [DefaultValue("false")]
        public bool IsDeleted { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}