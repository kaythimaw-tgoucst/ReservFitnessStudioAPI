using System.ComponentModel.DataAnnotations;

namespace FitnessStudio.Application.DTO.UseCase.Request.Package
{
    public class PurchasePackageRequest
    {
        [Required]
        public Guid BusinessStudioId { get; set; }

        [Range(1, double.MaxValue)]
        public decimal TotalCredits { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }
    }
}
