using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace SD.ACMA.DNCRProject.Website.Helpers
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FileUploadExtensionsAttribute : ValidationAttribute, IClientValidatable
    {
        private List<string> ValidExtensions { get; set; }

        public FileUploadExtensionsAttribute(string fileExtensions)
        {
            ValidExtensions = fileExtensions.Split('|').ToList();
        }

        public override bool IsValid(object value)
        {
            HttpPostedFileBase file = value as HttpPostedFileBase;
            if (file != null)
            {
                var fileName = file.FileName;
                var isValidExtension = ValidExtensions.Any(y => fileName.EndsWith(y));
                return isValidExtension;
            }
            return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
                       {
                           ErrorMessage = ErrorMessage,
                           ValidationType = "fileextensions"
                       };
            clientValidationRule.ValidationParameters.Add("fileextensions", string.Join(",", ValidExtensions));
            return new[] {clientValidationRule};
        }
    }
}