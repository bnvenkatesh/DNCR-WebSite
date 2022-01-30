using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc.Html;
using System.Web.Routing;
using SD.ACMA.DNCRProject.Website.Helpers;

namespace System.Web.Mvc
{
    public static class HtmlExtension
    {
        public static MvcHtmlString RefundTypeDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Object htmlAttributes = null)
        {
            SelectList list = new SelectList(new string[] { "Subscription Refund", "Wash Credits RollOver", "Reverse Wash", "Balance Refund", "Manual Adjustment" });
            return SelectExtensions.DropDownListFor(htmlHelper, expression, list, "Select", htmlAttributes);
        }

        public static MvcHtmlString TitlesDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Object htmlAttributes = null)
        {
            SelectList list = new SelectList(new string[] { "Mr", "Ms", "Miss", "Mrs", "Dr"});
            return SelectExtensions.DropDownListFor(htmlHelper, expression, list, "Select", htmlAttributes);
        }

        public static MvcHtmlString StateDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Object htmlAttributes = null)
        {
            SelectList list = new SelectList(new string[] { "ACT", "NSW", "NT", "QLD", "SA", "TAS", "VIC", "WA" });
            return SelectExtensions.DropDownListFor(htmlHelper, expression, list, "Select", htmlAttributes);
        }

        /*public static MvcHtmlString CountryDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Object htmlAttributes = null)
        {
            var regex = new Regex(@"^[a-zA-Z].*$");
            var countryList = new List<Country>();
            var isoCodes =
                CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    .OrderBy(x => new RegionInfo(x.LCID).TwoLetterISORegionName)
                    .GroupBy(x => new RegionInfo(x.LCID).TwoLetterISORegionName)
                    .Select(x => x.Key);
            foreach (var isoCode in isoCodes)
            {
                if (isoCode.Length == 2)
                {
                    var name = new RegionInfo(isoCode).DisplayName;
                    if (regex.IsMatch(name))
                    {
                        countryList.Add(new Country() {IsoCode = isoCode, Name = new RegionInfo(isoCode).DisplayName});
                    }
                }
            }
            countryList = countryList.OrderBy(x => x.Name).ToList();
            SelectList list = new SelectList(countryList, "IsoCode", "Name");
            return SelectExtensions.DropDownListFor(htmlHelper, expression, list, "Select", htmlAttributes);
        }*/

        public static MvcHtmlString QuantityDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Object htmlAttributes = null)
        {
            SelectList list = new SelectList(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            return SelectExtensions.DropDownListFor(htmlHelper, expression, list, htmlAttributes);
        }

        public static MvcHtmlString LimitedQuantityDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Object htmlAttributes = null)
        {
            SelectList list = new SelectList(new int[] { 0, 1 });
            return SelectExtensions.DropDownListFor(htmlHelper, expression, list, htmlAttributes);
        }

        public static MvcHtmlString PhoneProviderDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Object htmlAttributes = null)
        {
            SelectList list = new SelectList(new string[] { "Telstra", "Optus", "Vodafone", "TPG", "iiNet", "iPrimus", "Dodo", "Virgin Mobile", "Other" });
            return SelectExtensions.DropDownListFor(htmlHelper, expression, list, "Select", htmlAttributes);
        }

        /*public static MvcHtmlString IndustryEnquirySubjectDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Object htmlAttributes = null)
        {
            SelectList list = new SelectList(new string[] { "Balance Refunds", "Wash Credit Refunds", "Wash Credit Rollovers", "Balance Transfer Between Accounts", "Others" });
            return SelectExtensions.DropDownListFor(htmlHelper, expression, list, "Select", htmlAttributes);
        }*/

        public static MvcHtmlString TimePickerDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Object htmlAttributes = null)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var interval = 30;
            DateTime start = DateTime.ParseExact("12:00", "h:mm", null);
            for (DateTime i = start; i < start.AddHours(12); i = i.AddMinutes(interval))
            {
                var text = String.Format("{0} to {1}", i.ToString("h:mm"), i.AddMinutes(interval - 1).ToString("h:mm"));
                list.Add(new SelectListItem { Value = text, Text = text });
            }

            return SelectExtensions.DropDownListFor(htmlHelper, expression, list, "Select", htmlAttributes);
        }

        public static MvcHtmlString ChannelDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Object htmlAttributes = null)
        {           
            SelectList list = new SelectList(new string[] {});
            if (SessionHelper.IsAcma)
                list = new SelectList(new string[] { "OasisIVR", "Symposium", "Agent", "Webform", "Other", "Phone", "Email", "Letter", "Fax", "ACMA", "Split" });                
            else 
                list = new SelectList(new string[] { "OasisIVR", "Symposium", "Agent", "Webform", "Other", "Phone", "Email", "Letter", "Fax", "Split" });
            return SelectExtensions.DropDownListFor(htmlHelper, expression, list, htmlAttributes);
        }

        public static MvcHtmlString CheckBoxSimple(this HtmlHelper htmlHelper, string name, object htmlAttributes)
        {
            string checkBoxWithHidden = htmlHelper.CheckBox(name, htmlAttributes).ToHtmlString().Trim();
            string pureCheckBox = checkBoxWithHidden.Substring(0, checkBoxWithHidden.IndexOf("<input", 1));
            return new MvcHtmlString(pureCheckBox);
        }

        public static MvcHtmlString IndustrySectorDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Object htmlAttributes = null)
        {
            SelectList list = new SelectList(new string[] { "Commonwealth Department", 
                "Other Commonwealth Agency", 
                "State Government", 
                "Local Government", 
                "Company", 
                "Community or Volunteer Group", 
                "Person" });

            return htmlHelper.DropDownListFor(expression, list, "Select", htmlAttributes);
        }

        public static MvcHtmlString GetDisplayName<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            string labelText;
            if (metadata.ContainerType.GetProperty(metadata.PropertyName).GetCustomAttributes(typeof(RequiredAttribute), false).Any() || metadata.ContainerType.GetProperty(metadata.PropertyName).GetCustomAttributes(typeof(MandatoryAttribute), false).Any())
            {
                labelText = (metadata.DisplayName ?? metadata.PropertyName) + "<abbr title='required'>*</abbr>";
            }
            else labelText = metadata.DisplayName ?? metadata.PropertyName;
            return MvcHtmlString.Create(labelText);
        }

        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);

            string labelText;
            if (metadata.ContainerType.GetProperty(metadata.PropertyName).GetCustomAttributes(typeof(RequiredAttribute), false).Any() || metadata.ContainerType.GetProperty(metadata.PropertyName).GetCustomAttributes(typeof(MandatoryAttribute), false).Any())
            {
                labelText = (metadata.DisplayName ?? metadata.PropertyName) + "<abbr title='required'>*</abbr>";
            }
            else labelText = metadata.DisplayName ?? metadata.PropertyName;

            TagBuilder tagBuilder = new TagBuilder("label");
            if (htmlAttributes != null)
            {
                tagBuilder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            }

            if (!tagBuilder.Attributes.ContainsKey("for"))
            {
                tagBuilder.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
            }
            tagBuilder.InnerHtml = labelText;

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString RequiredTextFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);

            string labelText;
            if (metadata.ContainerType.GetProperty(metadata.PropertyName).GetCustomAttributes(typeof(RequiredAttribute), false).Any() || metadata.ContainerType.GetProperty(metadata.PropertyName).GetCustomAttributes(typeof(MandatoryAttribute), false).Any())
            {
                labelText = (metadata.DisplayName ?? metadata.PropertyName) + "<abbr title='required'>*</abbr>";
            }
            else labelText = metadata.DisplayName ?? metadata.PropertyName;

            TagBuilder tagBuilder = new TagBuilder("p");
            if (htmlAttributes != null)
            {
                tagBuilder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            }
            tagBuilder.InnerHtml = labelText;

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        #region DropDownHelper

        private static MvcHtmlString DropDownCategoryListFor(this HtmlHelper htmlHelper, string optionLabel, string name, IDictionary<string, IEnumerable<SelectListItem>> selectList, IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            object value;
            if (fullName.EndsWith("]"))
            {
                var startIndex = fullName.IndexOf("[") + 1;
                var endIndex = fullName.IndexOf("]");
                var listIndex = Int32.Parse(fullName.Substring(startIndex, endIndex - startIndex));
                value = htmlHelper.ViewData.Eval(fullName.Substring(0, startIndex - 1));
                if (value != null)
                {
                    var list = ((IList<string>)value);
                    if (listIndex < list.Count)
                    {
                        value = list[listIndex];
                    }
                }
            } else value = htmlHelper.ViewData.Eval(fullName);

            if (value != null)
            {
                Dictionary<string, IEnumerable<SelectListItem>> newSelectListDictionary = new Dictionary<string, IEnumerable<SelectListItem>>();
                foreach (var category in selectList.Keys)
                {
                    List<SelectListItem> newSelectList = new List<SelectListItem>();

                    foreach (SelectListItem item in selectList[category])
                    {
                        item.Selected = (item.Value == value.ToString());
                        newSelectList.Add(item);
                    }

                    newSelectListDictionary.Add(category, newSelectList);
                }
                selectList = newSelectListDictionary;
            }

            StringBuilder listItemBuilder = new StringBuilder();

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null)
            {
                listItemBuilder.AppendLine(ListItemToOption(new SelectListItem() { Text = optionLabel, Value = String.Empty, Selected = false }));
            }

            foreach (var category in selectList.Keys)
            {
                if (category == selectList[category].First().Text)
                {
                    foreach (var item in selectList[category])
                    {
                        listItemBuilder.AppendLine(ListItemToOption(new SelectListItem() { Text = item.Text, Value = item.Value, Selected = item.Selected }));
                    }
                }
                else listItemBuilder.AppendLine(ListItemToOption(category, selectList[category]));
            }

            TagBuilder tagBuilder = new TagBuilder("select")
            {
                InnerHtml = listItemBuilder.ToString()
            };

            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
            tagBuilder.GenerateId(fullName);

            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name));

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        internal static string ListItemToOption(string category, IEnumerable<SelectListItem> items)
        {
            TagBuilder builder = new TagBuilder("optgroup");
            builder.Attributes["label"] = category;

            // Convert each ListItem to an <option> tag
            StringBuilder listItemBuilder = new StringBuilder();

            foreach (SelectListItem item in items)
            {
                listItemBuilder.AppendLine(ListItemToOption(item));
            }

            builder.InnerHtml = listItemBuilder.ToString();

            return builder.ToString(TagRenderMode.Normal);
        }

        internal static string ListItemToOption(SelectListItem item)
        {
            TagBuilder builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }
            return builder.ToString(TagRenderMode.Normal);
        }

        #endregion
    }

    public class Country
    {
        public string IsoCode { get; set; }
        public string Name { get; set; }
    }
}