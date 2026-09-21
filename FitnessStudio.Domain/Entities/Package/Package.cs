using System;

namespace FitnessStudio.Domain.Entities.Package
{
    public class Package
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BusinessStudioId { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal RemainingCredits { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
