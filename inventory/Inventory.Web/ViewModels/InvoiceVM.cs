using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Web.ViewModels
{
    public class InvoiceVM
    {
        [Display(Name = "Invoice Number")]
        public int Id { get; set; }

        public int? SupplierId { get; set; }

        public int? SupplierIdOriginal { get; set; }

        public string SupplierDisplayNameOriginal { get; set; }

        public SupplierVM Supplier { get; set; }

        [RegularExpression("^[a-zA-Z0-9_-]*$", ErrorMessage = "Only AlphaNumeric and Hyphen allowed")]
        [Display(Name = "PO Number")]
        public string PONumber { get; set; }

        public string PONumberOriginal { get; set; }

        [Display(Name = "Purchase Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? PurchaseDate { get; set; }

        [Display(Name = "Total Price")]
        [RegularExpression("^\\d{0,8}(\\.\\d{2,2})?$", ErrorMessage = "The Total Price must include two decimals.")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal? TotalPrice { get; set; }

        [Display(Name = "Items Total Price")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal? InvoiceItemsTotal { get; set; }

        [Display(Name = "Items Count")]
        public int? InvoiceItemsCount { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }

        //
        [Display(Name = "Supplier")]
        public string SupplierName { get; set; }
        //

        public string PreviousUrl { get; set; }

        public IEnumerable<SupplierVM> Suppliers { get; set; }
    }
}