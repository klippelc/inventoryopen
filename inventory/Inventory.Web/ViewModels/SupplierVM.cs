using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Web.ViewModels
{
    public class SupplierVM
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

        [Url]
        public string Url { get; set; }

        [Phone]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }

        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public int? StateId { get; set; }

        public StateVM State { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        public bool Active { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        //
        public int? InvoiceCount { get; set; }

        public string PreviousUrl { get; set; }

        //

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }

        public IEnumerable<StateVM> States { get; set; }

    }
}