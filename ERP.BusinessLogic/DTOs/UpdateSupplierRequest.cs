namespace ERP.BusinessLogic.DTOs
{
    public class UpdateSupplierRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? ContactPerson { get; set; }
        public string TaxNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
