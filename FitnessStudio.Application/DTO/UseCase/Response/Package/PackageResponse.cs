using FitnessStudio.Domain.Entities.Package;

namespace FitnessStudio.Application.DTO.UseCase.Response.Package
{
    public class PackageResponse
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid BusinessStudioId { get; private set; }
        public decimal TotalCredits { get; private set; }
        public decimal RemainingCredits { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        public bool IsExpired { get; private set; }
        public DateTime CreatedOn { get; private set; }

        public PackageResponse(FitnessStudio.Domain.Entities.Package.Package package, DateTime currentDateUtc)
        {
            Id = package.Id;
            UserId = package.UserId;
            BusinessStudioId = package.BusinessStudioId;
            TotalCredits = package.TotalCredits;
            RemainingCredits = package.RemainingCredits;
            ExpiryDate = package.ExpiryDate;
            IsExpired = package.ExpiryDate < currentDateUtc;
            CreatedOn = package.CreatedOn;
        }
    }
}
