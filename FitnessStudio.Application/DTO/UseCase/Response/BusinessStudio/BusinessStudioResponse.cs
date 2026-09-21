
using FitnessStudio.Domain.Entities.BusinessStudio;

namespace FitnessStudio.Application.DTO.UseCase.Response.BusinessStudio
{
    public class BusinessStudioResponse
    {
        public Guid Id { get; private set; }
        public Guid CompanyId { get; private set; }
        public string CompanyName { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public DateTime CreatedOn { get; private set; }
        public DateTime? UpdatedOn { get; private set; }

        public BusinessStudioResponse(FitnessStudio.Domain.Entities.BusinessStudio.BusinessStudio businessStudio)
        {
            Id = businessStudio.Id;
            CompanyId = businessStudio.CompanyId;
            CompanyName = businessStudio.CompanyName;
            Name = businessStudio.Name;
            CreatedOn = businessStudio.CreatedOn;
            UpdatedOn = businessStudio.UpdatedOn;
        }
    }
}
