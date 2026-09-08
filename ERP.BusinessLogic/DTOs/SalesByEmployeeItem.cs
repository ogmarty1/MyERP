namespace ERP.BusinessLogic.DTOs
{
    public class SalesByEmployeeItem
    {
        public int UserId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
