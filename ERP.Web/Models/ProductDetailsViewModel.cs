using ERP.DataAccess.Models;

namespace ERP.Web.Models
{
    public class ProductDetailsViewModel
    {
        public ProductFormViewModel Form { get; set; } = new();
        public List<Order> OrderHistory { get; set; } = new();
        public List<PriceCheckOffer> PriceHistory { get; set; } = new();
    }
}
