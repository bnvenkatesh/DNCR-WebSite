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
    public class DateGreaterThanAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _otherPropertyName;

        public DateGreaterThanAttribute(string otherPropertyName)
        {
            _otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;

            var otherProperty = validationContext.ObjectType.GetProperty(_otherPropertyName).GetValue(validationContext.ObjectInstance, null).ToString();

            DateTime otherPropertyDate;
            DateTime toValidate;

            if (DateTime.TryParseExact(otherProperty, "d/M/yyyy", CultureInfo.CreateSpecificCulture("en-AU"), DateTimeStyles.None, out otherPropertyDate) &&
                DateTime.TryParseExact(value.ToString(), "d/M/yyyy", CultureInfo.CreateSpecificCulture("en-AU"), DateTimeStyles.None, out toValidate))
            {
                if(otherPropertyDate > toValidate)
                {
                    validationResult = new ValidationResult(ErrorMessageString);
                }
            }
            else
            {
                validationResult = new ValidationResult("An error occurred while validating the property. OtherProperty is not of type DateTime");
            }

            return validationResult;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "dategreaterthan"
            };
            clientValidationRule.ValidationParameters.Add("otherpropertyname", _otherPropertyName);

            return new[] { clientValidationRule };
        }
    }
}