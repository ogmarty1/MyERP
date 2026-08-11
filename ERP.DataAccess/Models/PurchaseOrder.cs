using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.DataAccess.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }

        // Foreign Key към Supplier
        public int SupplierId { get; set; }

        // Navigation property - поръчката за доставка е към един доставчик
        public Supplier Supplier { get; set; } = null!;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Ordered;

        [StringLength(1000)]
        public string? Notes { get; set; }

        // Foreign Key към User (потребителят, създал поръчката за доставка)
        public int CreatedByUserId { get; set; }

        // Navigation property
        public User CreatedByUser { get; set; } = null!;

        // Navigation property - поръчката за доставка има много редове
        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    }
}
