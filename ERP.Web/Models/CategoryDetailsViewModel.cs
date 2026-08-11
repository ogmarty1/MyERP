using ERP.DataAccess.Models;

namespace ERP.Web.Models
{
    public class CategoryDetailsViewModel
    {
        public CategoryFormViewModel Form { get; set; } = new();
        public List<Product> Products { get; set; } = new();
    }
}
