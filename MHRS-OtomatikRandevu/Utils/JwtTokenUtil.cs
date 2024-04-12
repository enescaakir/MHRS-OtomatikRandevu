using MHRS_OtomatikRandevu.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace MHRS_OtomatikRandevu.Utils
{
    public static class JwtTokenUtil
    {
        public static DateTime GetTokenExpireTime(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return DateTime.MinValue;
                if (!token.Contains("."))
                    return DateTime.MinValue;

                string[] parts = token.Split('.');
                var payload = JsonSerializer.Deserialize<JwtTokenModel>(Base64UrlEncoder.Decode(parts[1]));
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(payload.ExpirationUnix);
                return dateTimeOffset.LocalDateTime;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
    }
}
