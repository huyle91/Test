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
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)System.Math.Ceiling(count / (double)pageSize);
        }
    }
}
