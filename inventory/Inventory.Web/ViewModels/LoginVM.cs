using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Inventory.Web.ViewModels
{
    public class LoginVM
    {
        [Display(Name = "Username"), Required]
        public string Username { get; set; }

        [Display(Name = "Password"), Required, DataType(DataType.Password)]
        public string Password { get; set; }
        
        public string ReturnUrl { get; set; }

        public string IpAddress => HttpContext.Current.Request.UserHostAddress;
    }
}