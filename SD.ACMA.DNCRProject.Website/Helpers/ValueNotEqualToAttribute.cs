using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValueNotEqualToAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _otherPropertyName;

        public ValueNotEqualToAttribute(string otherPropertyName)
        {
            _otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;
            if(value != null)
            {
                var otherPhoneNumber = validationContext.ObjectType.GetProperty(_otherPropertyName).GetValue(validationContext.ObjectInstance, null).ToString();
                if (otherPhoneNumber == value.ToString())
                {
                    validationResult = new ValidationResult(ErrorMessageString);
                }
            }
            return validationResult;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "valuenotequalto"
            };
            clientValidationRule.ValidationParameters.Add("otherpropertyname", _otherPropertyName);

            return new[] { clientValidationRule };
        }
    }
}