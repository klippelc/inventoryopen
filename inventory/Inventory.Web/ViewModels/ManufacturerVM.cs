using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Web.ViewModels
{
    public class ManufacturerVM
    {
        public int Id { get; set; }

        [RegularExpression("^[a-zA-Z0-9' '_/&.-]*$", ErrorMessage = "Only AlphaNumeric, and '&-/.' allowed")]
        public string Name { get; set; }

        public string OriginalName { get; set; }

        [RegularExpression("^[a-zA-Z0-9' '_/&.-]*$", ErrorMessage = "Only AlphaNumeric, and '&-/.' allowed")]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        public string OriginalDisplayName { get; set; }

        public string Description { get; set; }

        [Display(Name = "URL")]
        [Url]
        public string SupportUrl { get; set; }

        [Display(Name = "Phone")]
        [Phone]
        public string SupportPhone { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string SupportEmail { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public bool Active { get; set; }

        public List<ProductVM> Products { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }

        public string PreviousUrl { get; set; }

        //
        public int? ProductCount { get; set; }

        public int? InvoiceItemCount { get; set; }
        //
    }
}