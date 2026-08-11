using ERP.DataAccess.Models;

namespace ERP.Web.Models
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; } = null!;
        public List<Product> ProductCatalog { get; set; } = new();
    }
}
