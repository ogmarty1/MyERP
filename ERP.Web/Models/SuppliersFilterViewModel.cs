using System.ComponentModel.DataAnnotations;
using ERP.DataAccess.Models;

namespace ERP.Web.Models
{
    public class SuppliersFilterViewModel
    {
        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Display(Name = "Company")]
        public string? Company { get; set; }

        [Display(Name = "Contact Person")]
        public string? ContactPerson { get; set; }

        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }
    }

    public class SuppliersIndexViewModel
    {
        public SuppliersFilterViewModel Filter { get; set; } = new();
        public List<Supplier> Suppliers { get; set; } = new();
    }
}
