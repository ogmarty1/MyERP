using System.ComponentModel.DataAnnotations;

namespace ERP.DataAccess.Models
{
    public enum MovementType
    {
        Inflow,
        Outflow,
        Adjustment
    }

    public class InventoryMovement
    {
        public int Id { get; set; }

        // Foreign Key към Product
        public int ProductId { get; set; }

        // Navigation property - движението се отнася за един продукт
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        public MovementType MovementType { get; set; }

        [StringLength(250)]
        public string? Reason { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        // Foreign Key към Order - опционален, тъй като не всяко движение произлиза от поръчка
        public int? OrderId { get; set; }

        // Navigation property - движението може да е свързано с една поръчка (продажба) или да е ръчна корекция
        public Order? Order { get; set; }

        // Foreign Key към PurchaseOrder - опционален, тъй като не всяко движение произлиза от доставка
        public int? PurchaseOrderId { get; set; }

        // Navigation property - движението може да е свързано с получена поръчка за доставка
        public PurchaseOrder? PurchaseOrder { get; set; }
    }
}
