using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.DNCRProject.Website.Helpers
{
    public static class CountryDropDownHelper
    {
        public static List<SelectListItem> BuildCountryDropDownList(List<CountryModel> countryModels)
        {
            var countries = new List<SelectListItem>();
            foreach (var countryModel in countryModels)
            {
                countries.Add(new SelectListItem() { Text = countryModel.countryName, Value = countryModel.CountryISO });
            }
            return countries;
        }
    }
}