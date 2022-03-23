using System;

namespace Inventory.Web.ViewModels
{
    public class RoomTypeVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Sequence { get; set; }

        public bool Active { get; set; }

        public bool IsDeleted { get; set; }

        public string Notes { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }
    }
}