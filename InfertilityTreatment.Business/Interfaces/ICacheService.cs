namespace InfertilityTreatment.Business.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class;
        Task RemoveAsync(string key);
        Task RemovePatternAsync(string pattern);

        // Specific cache key generators
        string GetDoctorAvailabilityKey(int doctorId, DateTime date);
        string GetTreatmentServicesKey();
        string GetTreatmentPackagesKey(int serviceId);
        string GetAnalyticsKey(string type, DateTime startDate, DateTime endDate);

        // Cache expiration getters
        TimeSpan GetDoctorAvailabilityCacheTime();
        TimeSpan GetTreatmentServicesCacheTime();
        TimeSpan GetAnalyticsCacheTime();
    }
}
