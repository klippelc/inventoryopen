using System;

namespace Inventory.Data.Common
{
    public class BaseEntity
    {
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }
    }
}