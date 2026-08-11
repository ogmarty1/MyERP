using ERP.DataAccess.Models;

namespace ERP.Web.Models
{
    public class SupplierDetailsViewModel
    {
        public SupplierFormViewModel Form { get; set; } = new();
        public List<PurchaseOrder> PurchaseOrderHistory { get; set; } = new();
    }
}
