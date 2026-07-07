using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.DataAccess.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        // Foreign Key към Order
        public int OrderId { get; set; }

        // Navigation property - редът принадлежи на една поръчка
        public Order Order { get; set; } = null!;

        // Foreign Key към Product
        public int ProductId { get; set; }

        // Navigation property - редът се отнася за един продукт
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }
    }
}
