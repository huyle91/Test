using InfertilityTreatment.Entity.DTOs.Common;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface IQueryOptimizationService
    {
        Task<T> ExecuteOptimizedQueryAsync<T>(Func<Task<T>> query, string operationName);
        IQueryable<T> ApplyOptimizations<T>(IQueryable<T> query) where T : class;
        Task<PaginatedResultDto<T>> GetPaginatedResultAsync<T>(IQueryable<T> query, int pageNumber, int pageSize, string operationName) where T : class;
        IQueryable<T> IncludeOptimized<T>(IQueryable<T> query, params string[] includes) where T : class;
        Task WarmupCriticalQueries();
        Task<Dictionary<string, object>> GetQueryStatistics();
    }
}
