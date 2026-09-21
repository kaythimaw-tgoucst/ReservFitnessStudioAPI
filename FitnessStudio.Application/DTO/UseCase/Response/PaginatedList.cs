using Newtonsoft.Json;
using FitnessStudio.Application.Common.Constants;

namespace FitnessStudio.Application.DTO.UseCase.Response
{
    public class PaginatedList<T>
    {
        [JsonProperty]
        public IList<T> Items { get; private set; }
        [JsonProperty]
        public int PageNumber { get; private set; }
        [JsonProperty]
        public int TotalPages { get; private set; }
        [JsonProperty]
        public int TotalCount { get; private set; }

        [JsonConstructor]
        public PaginatedList()
        {
            Items = new List<T>();
            PageNumber = PaginationDefaults.PageNumber;
            TotalPages = 0;
            TotalCount = 0;
        }

        public PaginatedList(IList<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = count == 0 ? 0 : (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Items = items;
        }

        public PaginatedList(List<T> items)
        {
            PageNumber = PaginationDefaults.PageNumber;
            TotalPages = 0;
            TotalCount = 0;
            Items = items;
        }
        [JsonIgnore]
        public bool HasPreviousPage => PageNumber > 1;
        [JsonIgnore]
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
