using System.ComponentModel.DataAnnotations;

namespace ERP.DataAccess.Models
{
    public class PriceCheck
    {
        public int Id { get; set; }

        // Foreign Key към Product
        public int ProductId { get; set; }

        // Navigation property - проверката на цени се отнася за един продукт
        public Product Product { get; set; } = null!;

        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

        [StringLength(200)]
        public string Query { get; set; } = string.Empty;

        // Navigation property - проверката може да съдържа много намерени предложения
        public ICollection<PriceCheckOffer> Offers { get; set; } = new List<PriceCheckOffer>();
    }
}
