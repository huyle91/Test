using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace InfertilityTreatment.Business.Helpers
{
    public static class VNPayHelper
    {
        public static string CreateRequestUrl(string baseUrl, Dictionary<string, string> parameters, string hashSecret)
        {
            var sortedParams = parameters
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .OrderBy(x => x.Key)
                .ToList();

            var queryString = string.Join("&", sortedParams.Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));
            var secureHash = ComputeHmacSha512(hashSecret, queryString);

            return $"{baseUrl}?{queryString}&vnp_SecureHash={secureHash}";
        }

        public static bool ValidateSignature(Dictionary<string, string> parameters, string hashSecret, string secureHash)
        {
            var filteredParams = parameters
                .Where(x => x.Key != "vnp_SecureHash" && x.Key != "vnp_SecureHashType")
                .OrderBy(x => x.Key)
                .ToList();

            // Không được decode ở đây, giữ nguyên raw value
            var queryString = string.Join("&", filteredParams.Select(x => $"{x.Key}={x.Value}")); // ✅


            var computedHash = ComputeHmacSha512(hashSecret, queryString);

            return secureHash.Equals(computedHash, StringComparison.OrdinalIgnoreCase);
        }

        public static string ComputeHmacSha512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public static string GenerateOrderId()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public static string FormatAmount(decimal amount)
        {
            return ((long)(amount * 100)).ToString();
        }

        public static decimal ParseAmount(string vnpAmount)
        {
            if (long.TryParse(vnpAmount, out long amount))
            {
                return amount / 100m;
            }
            return 0;
        }

        public static DateTime? ParsePayDate(string vnpPayDate)
        {
            if (DateTime.TryParseExact(vnpPayDate, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            return null;
        }
    }
}
