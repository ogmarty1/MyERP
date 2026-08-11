using System.ComponentModel.DataAnnotations;
using ERP.BusinessLogic.DTOs;

namespace ERP.Web.Models
{
    public class ReportsFilterViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Group By")]
        public ReportPeriodGrouping Grouping { get; set; } = ReportPeriodGrouping.Monthly;
    }

    public class ReportsViewModel
    {
        public ReportsFilterViewModel Filter { get; set; } = new();
        public List<SalesByPeriodItem> SalesByPeriod { get; set; } = new();
        public List<SalesByCategoryItem> SalesByCategory { get; set; } = new();
        public List<PurchasesByPeriodItem> PurchasesByPeriod { get; set; } = new();
        public List<PurchasesBySupplierItem> PurchasesBySupplier { get; set; } = new();
    }
}
