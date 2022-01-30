using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateLessThanOrEqualToTodayAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            DateTime date;

            if (value != null && DateTime.TryParseExact(value.ToString(), "d/M/yyyy", CultureInfo.CreateSpecificCulture("en-AU"), DateTimeStyles.None, out date))
            {
                return date <= DateTime.Now;
            }
            return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "datelessthanorequaltotoday"
            };

            return new[] { clientValidationRule };
        }
    }
}