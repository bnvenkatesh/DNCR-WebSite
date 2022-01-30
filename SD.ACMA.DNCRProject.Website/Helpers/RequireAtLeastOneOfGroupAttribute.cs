using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequireAtLeastOneOfGroupAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _groupName;

        public RequireAtLeastOneOfGroupAttribute(string groupName)
        {
            _groupName = groupName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = new ValidationResult(ErrorMessageString);

            var allProperties = validationContext.ObjectType.GetProperties()
                .Where(x => x.GetCustomAttributes(typeof (RequireAtLeastOneOfGroupAttribute), false)
                                .OfType<RequireAtLeastOneOfGroupAttribute>()
                                .Any(y => y._groupName == _groupName));

            foreach (var property in allProperties)
            {
                var propertyValue = property.GetValue(validationContext.ObjectInstance, null);
                if (propertyValue != null)
                {
                    validationResult = ValidationResult.Success;
                    break;
                }
            }

            return validationResult;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "group"
            };
            var propertyNames = metadata.ContainerType.GetProperties()
                .Where(x => x.GetCustomAttributes(typeof(RequireAtLeastOneOfGroupAttribute), false)
                                .OfType<RequireAtLeastOneOfGroupAttribute>()
                                .Any(y => y._groupName == _groupName))
                .Select(z => z.Name);

            clientValidationRule.ValidationParameters.Add("propertynames", String.Join(",", propertyNames));

            return new[] { clientValidationRule };

            /*var groupProperties = GetGroupProperties(metadata.ContainerType).Select(p => p.Name);
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage
            };
            rule.ValidationType = string.Format("group", GroupName.ToLower());
            rule.ValidationParameters["propertynames"] = string.Join(",", groupProperties);
            yield return rule;*/
        }
    }
}