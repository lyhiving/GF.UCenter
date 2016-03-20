using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common
{
    public static class StringExtensions
    {
        public static string MapNullToEmpty(this string str)
        {
            return str == null ? "" : str;
        }

        public static string FormatInvariant(this string template, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, template, args);
        }

        public static string JoinToString(this IEnumerable<string> items, string separator)
        {
            return string.Join(separator, items);
        }

        public static string JoinToString<T>(this IEnumerable<T> items, string separator, Func<T, string> selector)
        {
            return string.Join(separator, items.Select(i => selector(i)));
        }

        public static string FirstCharacterToLower(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
                return str;

            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}
