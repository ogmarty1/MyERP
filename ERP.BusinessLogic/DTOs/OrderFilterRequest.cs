using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.DTOs
{
    public class OrderFilterRequest
    {
        public int? OrderId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinTotal { get; set; }
        public decimal? MaxTotal { get; set; }
        public OrderStatus? Status { get; set; }
    }
}
