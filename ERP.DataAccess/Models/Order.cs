using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.DataAccess.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty; // Уникален номер на поръчката

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime? ShippedDate { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        // Foreign Key към Customer
        public int CustomerId { get; set; }

        // Navigation property - поръчката принадлежи на един клиент
        public Customer Customer { get; set; } = null!;

        // Foreign Key към User (потребителят, създал поръчката)
        public int UserId { get; set; }

        // Navigation property - поръчката е създадена от един потребител
        public User User { get; set; } = null!;

        // Navigation property - поръчката има много редове
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
