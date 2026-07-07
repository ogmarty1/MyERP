using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.DataAccess.Models
{
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
        public decimal Price { get; set; }

        public int QuantityInStock { get; set; }

        public bool IsActive { get; set; } = true;

        // Foreign Key към Category
        public int CategoryId { get; set; }

        // Navigation property - продуктът принадлежи на една категория
        public Category Category { get; set; } = null!;
    }
}