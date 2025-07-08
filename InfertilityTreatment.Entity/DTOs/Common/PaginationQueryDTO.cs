using System.Collections.Generic;

namespace InfertilityTreatment.Entity.DTOs.Common
{
    public class PaginationQueryDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }
}
