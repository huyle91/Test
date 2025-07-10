using InfertilityTreatment.Entity.Constants;
using System.Collections.Generic;

namespace InfertilityTreatment.Entity.DTOs.Common
{
    public class PaginatedResultDto<T>
    {
        public List<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public PaginatedResultDto(List<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber > 0 ? pageNumber : 1;
            PageSize = pageSize > 0 ? pageSize : PaginationConstants.DefaultPageSize;

            Items = items ?? new List<T>();
            TotalCount = count;
            TotalPages = PageSize > 0
                ? (int)Math.Ceiling(count / (double)PageSize)
                : 0;
        }
    }
}
