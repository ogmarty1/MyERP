namespace ERP.BusinessLogic.DTOs
{
    public class CreateCustomerRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string TaxNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
