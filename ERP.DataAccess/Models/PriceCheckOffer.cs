using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.DataAccess.Models
{
    public class PriceCheckOffer
    {
        public int Id { get; set; }

        // Foreign Key към PriceCheck
        public int PriceCheckId { get; set; }

        // Navigation property - предложението принадлежи на една проверка на цени
        public PriceCheck PriceCheck { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string SourceName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(1000)]
        public string? Link { get; set; }

        public int Position { get; set; }
    }
}
