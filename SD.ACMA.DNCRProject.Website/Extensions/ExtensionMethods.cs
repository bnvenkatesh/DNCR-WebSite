using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Extensions
{
    public static class ExtensionMethods
    {
        public static void MapModel<T>(this WebViewPage<T> page) where T : class
        {
            var models = page.ViewContext.TempData.Where(item => item.Value is T);

            if (models.Any())
            {
                page.ViewData.Model = (T)models.First().Value;
                page.ViewContext.TempData.Remove(models.First().Key);
            }
        }

        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

        public static string FixPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return string.Empty;
            }
            return phoneNumber.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Replace(".", "");
        }
    }
}