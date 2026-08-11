using ERP.DataAccess.Models;

namespace ERP.Web.Models
{
    public class PurchaseOrderDetailsViewModel
    {
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public List<Product> ProductCatalog { get; set; } = new();
    }
}
