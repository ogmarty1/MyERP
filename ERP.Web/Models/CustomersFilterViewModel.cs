using System.ComponentModel.DataAnnotations;
using ERP.DataAccess.Models;

namespace ERP.Web.Models
{
    public class CustomersFilterViewModel
    {
        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Display(Name = "Company")]
        public string? Company { get; set; }

        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }
    }

    public class CustomersIndexViewModel
    {
        public CustomersFilterViewModel Filter { get; set; } = new();
        public List<Customer> Customers { get; set; } = new();
    }
}
