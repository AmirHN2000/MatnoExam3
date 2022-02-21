using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace Competition3.Helper
{
    public static class Extension
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            var id = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value.ToString();
            if (string.IsNullOrEmpty(id))
                return 0;
            
            return int.Parse(id);
        }
        
        public static string GetDisplayName<T>(this T enumValue)
        {
            try
            {
                var type = typeof(T);
                var memInfo = type.GetMember(enumValue.ToString())[0];

                var attributes = memInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

                if (attributes.Length == 0)
                    return memInfo.Name;

                return ((DisplayAttribute)attributes[0]).GetName();
            }
            catch (Exception exp)
            {
                return enumValue.ToString();
            }
        }

        public static DateTime ToEnglishDateTime(this string date)
        {
            var splited = date.Split('/');
            return new DateTime(int.Parse(splited[0]), int.Parse(splited[1]), int.Parse(splited[2]),
                new PersianCalendar());
        }
    }
}