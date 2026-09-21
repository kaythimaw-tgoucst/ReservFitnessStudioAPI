using System.Linq;

namespace FitnessStudio.Application.DTO.UseCase.Response.Package
{
    public class GetPackageListResponse
    {
        public decimal TotalCredit { get; }
        public decimal TotalRemainingCredit { get; }

        public PaginatedList<PackageResponse> PaginatedList { get; }


        public GetPackageListResponse(PaginatedList<PackageResponse> paginatedList)
        {            
            TotalCredit = paginatedList.Items.Where(x => !x.IsExpired).Sum(x => x.TotalCredits);
            TotalRemainingCredit = paginatedList.Items.Where(x => !x.IsExpired).Sum(x => x.RemainingCredits);
            PaginatedList = paginatedList;
        }
    }
}
