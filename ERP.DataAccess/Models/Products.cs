using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.DataAccess.Models
{
    public enum ProductStatus
    {
        Active,
        Discontinued,
        OutOfStock
    }

    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty; // Уникален код на продукта

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }

        public int QuantityInStock { get; set; }

        public int MinimumQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        public ProductStatus Status { get; set; } = ProductStatus.Active;

        [StringLength(1000)]
        public string? Notes { get; set; }

        // Foreign Key към Category
        public int CategoryId { get; set; }

        // Navigation property - продуктът принадлежи на една категория
        public Category Category { get; set; } = null!;

        // Foreign Key към Supplier - опционален, не всеки продукт има зададен доставчик
        public int? SupplierId { get; set; }

        // Navigation property - продуктът може да е свързан с един доставчик
        public Supplier? Supplier { get; set; }
    }
}