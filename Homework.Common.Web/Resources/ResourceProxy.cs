using Microsoft.AspNetCore.Http;
using Homework.Common.Enums;
using Homework.Common.Web.Resources;

namespace Homework.AspNetCoreMvc.Utils
{
    public class ResourceProxy
    {
        public static string GetResourceValue(string resourceName)
        {
            HttpContextAccessor httpContextAccessor = new();

            var currentLaguange = httpContextAccessor.HttpContext.Request.Cookies["lang"];

            if (currentLaguange == "en") return en.ResourceManager.GetString(resourceName);
            else return sr.ResourceManager.GetString(resourceName);
        }

        public static Dictionary<ErrorCode, string> MapToDictionary(ErrorCode[] errorCodes)
        {
            var result = new Dictionary<ErrorCode, string>();
            foreach (var error in errorCodes)
            {
                result.Add(error, GetResourceValue($"ErrorCode_{(int)error}"));
            }
            return result;
        }
    }
}
