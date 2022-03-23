using System;

namespace Inventory.Web.ViewModels
{
    public class AmenityVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Sequence { get; set; }

        public int? TypeId { get; set; }

        public bool IsDeleted { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }

        //
        public bool IsChecked { get; set; }
        public string Checked { get; set; }
        //
    }
}