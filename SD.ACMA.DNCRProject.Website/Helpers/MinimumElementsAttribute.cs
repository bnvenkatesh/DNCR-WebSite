using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SD.ACMA.DNCRProject.Website.Models;

namespace SD.ACMA.DNCRProject.Website.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MinimumElementsAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly int _minElements;
        private readonly string _selector;

        public MinimumElementsAttribute(int minElements, string selector)
        {
            _minElements = minElements;
            _selector = selector;
        }

        public override bool IsValid(object value)
        {
            var list = value as IList;
            if (list != null)
            {
                if (list is List<string>)
                {
                    return list.OfType<string>().Count(item => !String.IsNullOrEmpty(item)) >= _minElements;
                }
                if (list is List<RegistrationNumber>)
                {
                    return list.OfType<RegistrationNumber>().Count(item => !String.IsNullOrEmpty(item.Number)) >= _minElements;
                }
                if (list is List<CheckNumber>)
                {
                    return list.OfType<CheckNumber>().Count(item => !String.IsNullOrEmpty(item.Number)) >= _minElements;
                }
                if (list is List<CheckboxViewModel>)
                {
                    return list.OfType<CheckboxViewModel>().Count(item => item.Checked) >= _minElements;
                }
                if (list is List<WashNumber>)
                {
                    return list.OfType<WashNumber>().Count(item => !String.IsNullOrEmpty(item.Number)) >= _minElements;
                }
                if (list is List<SubscriptionModel>)
                {
                    return list.OfType<SubscriptionModel>().Count(item => item.Quantity != 0) >= _minElements;
                }
            }
            return false;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "minimumelements"
            };
            clientValidationRule.ValidationParameters.Add("minimumelements", _minElements);
            clientValidationRule.ValidationParameters.Add("selector", _selector);

            return new[] { clientValidationRule };
        }
    }
}