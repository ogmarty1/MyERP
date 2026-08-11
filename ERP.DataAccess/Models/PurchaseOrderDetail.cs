using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.DataAccess.Models
{
    public class PurchaseOrderDetail
    {
        public int Id { get; set; }

        // Foreign Key към PurchaseOrder
        public int PurchaseOrderId { get; set; }

        // Navigation property - редът принадлежи на една поръчка за доставка
        public PurchaseOrder PurchaseOrder { get; set; } = null!;

        // Foreign Key към Product
        public int ProductId { get; set; }

        // Navigation property - редът се отнася за един продукт
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}
