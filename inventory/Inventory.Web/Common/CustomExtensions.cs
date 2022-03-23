using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Inventory.Web.Common
{
    public static class CustomExtensions
    {
        public static List<string> CreateSerialList(this String str)
        {
            return str?.Trim().Split(',')
                       .Where(s => !string.IsNullOrEmpty(s.RemoveAllSpaces()))
                       .Select(x => x.RemoveAllSpaces()?.ToUpper()).ToList();
        }

        public static List<string> CreateLicenseList(this String str)
        {
            return str?.Trim().Split(',')
                       .Where(s => !string.IsNullOrEmpty(s.RemoveAllSpaces()))
                       .Select(x => x.RemoveAllSpaces()).ToList();
        }

        private static string RemoveAllSpaces(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace(" ", String.Empty)
                        .Replace("\r\n", String.Empty)
                        .Replace("\n", String.Empty)
                        .Replace("\r", String.Empty)
                        .Replace(lineSeparator, String.Empty)
                        .Replace(paragraphSeparator, String.Empty);
        }

        public static IEnumerable<TSource> DistinctItems<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static string GetNumbers(this string item)
        {
            return new string(item.Where(c => char.IsDigit(c)).ToArray());
        }

        public static string GetAlphaNumeric(this string item)
        {
            return new string(item.Where(c => char.IsLetterOrDigit(c)).ToArray());
        }

        public static string RemoveSpaces<T>(this T item)
        {
            return item?.ToString().RemoveAllSpaces();
        }

        public static string NullorEmpty<T>(this T item)
        {
            return string.IsNullOrEmpty(item?.ToString()) ? null : item.ToString();
        }

        public static List<string> ToStringList<T>(this T item)
        {
            return new List<string> { item?.ToString().RemoveAllSpaces() };
        }

        public static string ListToString<T>(this ICollection<T> list)
        {
            return list != null ? string.Join(", ", list) : null;
        }

        public static List<int> StringToIntList(this String str)
        {
            return str?.Trim().Split(',').Select(Int32.Parse).ToList();
        }

        public static string Capitalize(this String str)
        {
            str = str?.Trim();

            if (string.IsNullOrEmpty(str))
                return str;
            else if (str.Length == 1)
                return char.ToUpper(str[0]).ToString();
            else
                return (char.ToUpper(str[0]) + str.Substring(1));
        }

        public static T FindInvalidListItem<T>(this ICollection<T> list, Regex regex)
        {
            return list != null ? list.Where(s => regex.Matches(s.ToString()).Count == 0).FirstOrDefault() : default;
        }

        public static bool InValidItem<T>(this T item, Regex regex)
        {
            return regex.Matches(item.ToString()).Count == 0;
        }

        public static T SmallestItemInList<T>(this ICollection<T> list)
        {
            return list != null ? list.OrderBy(s => s.ToString().Length).FirstOrDefault() : default;
        }

        public static T LargestItemInList<T>(this ICollection<T> list)
        {
            return list != null ? list.OrderByDescending(s => s.ToString().Length).FirstOrDefault() : default;
        }

        public static T DuplicateItemInList<T>(this ICollection<T> list)
        {
            return list != null ? list.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).FirstOrDefault() : default;
        }

        public static string PadNumbers(this String str)
        {
            return Regex.Replace(str, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }

        public static MvcHtmlString YesNo(this HtmlHelper htmlHelper, bool yesNo)
        {
            var text = yesNo ? "Yes" : "No";
            return new MvcHtmlString(text);
        }

        public static List<SelectListItem> Statuses = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Yes", Value = true.ToString() },
            new SelectListItem() { Text = "No", Value = false.ToString() },
        };

        public static string ToPhoneFormat(this string phone)
        {
            // Remove non-digit characters
            var phoneDigits = Regex.Replace(phone ?? "", "[^0-9]", string.Empty);

            // Format the digits
            var temp = Regex.Replace(phoneDigits, @"(\d{1,3})(\d{0,3})(\d{0,4})", "$1-$2-$3");
            return temp;
        }
    }
}