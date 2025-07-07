using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.DTOs.Doctors;
using InfertilityTreatment.Entity.DTOs.TreatmentServices;

namespace InfertilityTreatment.Business.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;

        // Cache keys
        private const string DOCTOR_AVAILABILITY_KEY = "doctor_availability_{0}_{1}"; // doctorId_date
        private const string TREATMENT_SERVICES_KEY = "treatment_services";
        private const string TREATMENT_PACKAGES_KEY = "treatment_packages_{0}"; // serviceId
        private const string ANALYTICS_DATA_KEY = "analytics_{0}_{1}_{2}"; // type_startDate_endDate

        // Cache expiration times
        private static readonly TimeSpan DoctorAvailabilityCacheTime = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan TreatmentServicesCacheTime = TimeSpan.FromHours(2);
        private static readonly TimeSpan AnalyticsCacheTime = TimeSpan.FromMinutes(30);

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out T? cachedValue))
                {
                    _logger.LogDebug("Cache hit for key: {Key}", key);
                    return cachedValue;
                }

                _logger.LogDebug("Cache miss for key: {Key}", key);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving from cache for key: {Key}", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class
        {
            try
            {
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration,
                    Priority = CacheItemPriority.Normal
                };

                _memoryCache.Set(key, value, cacheOptions);
                _logger.LogDebug("Cached data for key: {Key}, Expiration: {Expiration}", key, expiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                _logger.LogDebug("Removed cache for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key: {Key}", key);
            }
        }

        public async Task RemovePatternAsync(string pattern)
        {
            try
            {
                // Note: MemoryCache doesn't support pattern removal natively
                // This is a simplified implementation
                _logger.LogDebug("Pattern removal requested for: {Pattern}", pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache pattern: {Pattern}", pattern);
            }
        }

        // Specific cache methods
        public string GetDoctorAvailabilityKey(int doctorId, DateTime date)
        {
            return string.Format(DOCTOR_AVAILABILITY_KEY, doctorId, date.ToString("yyyy-MM-dd"));
        }

        public string GetTreatmentServicesKey()
        {
            return TREATMENT_SERVICES_KEY;
        }

        public string GetTreatmentPackagesKey(int serviceId)
        {
            return string.Format(TREATMENT_PACKAGES_KEY, serviceId);
        }

        public string GetAnalyticsKey(string type, DateTime startDate, DateTime endDate)
        {
            return string.Format(ANALYTICS_DATA_KEY, type, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
        }

        public TimeSpan GetDoctorAvailabilityCacheTime() => DoctorAvailabilityCacheTime;
        public TimeSpan GetTreatmentServicesCacheTime() => TreatmentServicesCacheTime;
        public TimeSpan GetAnalyticsCacheTime() => AnalyticsCacheTime;
    }
}
