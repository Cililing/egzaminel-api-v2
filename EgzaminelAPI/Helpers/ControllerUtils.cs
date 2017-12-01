using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EgzaminelAPI.Helpers
{
    public static class ControllerUtils
    {
        public static readonly string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public static readonly string EMPTY_DATE_PATTERN = "0001-01-01 00:00:00";
        public static DateTime? ConvertToDateTimeFromMySQLString(this string str)
        {
            if (str == null || str == "" || str == EMPTY_DATE_PATTERN) return null;
            else
            {
                return DateTime.ParseExact(str, DATE_TIME_FORMAT, null);
            }
        }

        public static string GetAuthTokenFromHttpContext(this Controller controller)
        {
            StringValues token;
            controller.HttpContext.Request.Headers.TryGetValue("Authorization", out token);

            if (!token.Any()) return "";
            return token[0];
        }

        public static bool UpdateIfNotNull<T>(T objectToUpdate, Action updater)
        {
            if (objectToUpdate == null) return false;
            else
            {
                updater.Invoke();
                return true;
            }
        }
    }
}
