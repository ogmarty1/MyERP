namespace ERP.DataAccess.Models
{
    public enum MovementType
    {
        Inflow,
        Outflow
    }

    public class InventoryMovement
    {
        public int Id { get; set; }

        // Foreign Key към Product
        public int ProductId { get; set; }

        // Navigation property - движението се отнася за един продукт
        public Product Product { get; set; } = null!;

        public decimal Quantity { get; set; }

        public MovementType MovementType { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        // Foreign Key към Order - опционален, тъй като не всяко движение произлиза от поръчка
        public int? OrderId { get; set; }

        // Navigation property - движението може да е свързано с една поръчка (продажба) или да е ръчна корекция
        public Order? Order { get; set; }
    }
}
