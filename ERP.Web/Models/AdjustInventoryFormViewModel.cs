using System.ComponentModel.DataAnnotations;
using ERP.DataAccess.Models;

namespace ERP.Web.Models
{
    public class AdjustInventoryFormViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be zero or greater.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Please select an adjustment type.")]
        [Display(Name = "Type")]
        public MovementType MovementType { get; set; }

        [StringLength(250, ErrorMessage = "Reason cannot exceed 250 characters.")]
        public string? Reason { get; set; }
    }
}
