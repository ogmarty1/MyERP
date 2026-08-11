using System.ComponentModel.DataAnnotations;
using ERP.DataAccess.Models;

namespace ERP.Web.Models
{
    public class OrdersFilterViewModel
    {
        [Display(Name = "Order ID")]
        public int? OrderId { get; set; }

        [Display(Name = "Customer")]
        public string? CustomerName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Min Total")]
        public decimal? MinTotal { get; set; }

        [Display(Name = "Max Total")]
        public decimal? MaxTotal { get; set; }

        [Display(Name = "Status")]
        public OrderStatus? Status { get; set; }
    }

    public class OrdersIndexViewModel
    {
        public OrdersFilterViewModel Filter { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
    }
}
