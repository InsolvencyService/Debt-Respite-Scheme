using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Insolvency.Portal.TagHelpers
{
    public class BssInputDateTagHelper : TagHelper
    {
        public ModelExpression Day { get; set; }
        public ModelExpression Month { get; set; }
        public ModelExpression Year { get; set; }
        public ModelExpression IsValidDateOfBirth { get; set; }

        public string Id { get; set; }
        public string Label { get; set; }
        public string Hint { get; set; }
        public string ValidationMessage { get; set; }
        public bool IsAutoFocus { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected IHtmlGenerator Generator { get; }

        public BssInputDateTagHelper(IHtmlGenerator generator) => Generator = generator;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var isDayValid = !ViewContext.ViewData.ModelState.TryGetValue(Day.Name, out var day) || day.Errors.Count == 0;
            var isMonthValid = !ViewContext.ViewData.ModelState.TryGetValue(Month.Name, out var month) || month.Errors.Count == 0;
            var isYearValid = !ViewContext.ViewData.ModelState.TryGetValue(Year.Name, out var year) || year.Errors.Count == 0;
            var isValidDateOfBirthValid = !ViewContext.ViewData.ModelState.TryGetValue(IsValidDateOfBirth.Name, out var isDateOfBirthValid) || isDateOfBirthValid.Errors.Count == 0;

            var isGroupValid = isDayValid && isMonthValid && isYearValid;

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
            output.Attributes.Clear();
            output.Attributes.SetAttribute("class", $"govuk-form-group {(!(isGroupValid && isValidDateOfBirthValid) ? "govuk-form-group--error" : string.Empty)}");
            output.Attributes.SetAttribute("id", Id + "-container");

            var fieldset = new TagBuilder("fieldset");
            fieldset.AddCssClass("govuk-fieldset");
            fieldset.MergeAttribute("role", "group");
            fieldset.MergeAttribute("name", Id);

            var legend = new TagBuilder("legend");
            legend.AddCssClass("govuk-fieldset__legend govuk-fieldset__legend--m date_legend_header");
            legend.MergeAttribute("id", Id + "-label");

            var dobMainLabel = new TagBuilder("span");
            dobMainLabel.AddCssClass("govuk-label");
            dobMainLabel.InnerHtml.Append(Label);
            legend.InnerHtml.AppendHtml(dobMainLabel);
            fieldset.InnerHtml.AppendHtml(legend);

            if (!string.IsNullOrEmpty(Hint))
            {
                var hint = new TagBuilder("span");
                hint.AddCssClass("govuk-hint");
                hint.MergeAttribute("id", Id + "-hint");
                hint.InnerHtml.Append(Hint);
                fieldset.InnerHtml.AppendHtml(hint);
            }

            HandleErrorSpan(ref isDayValid, ref isMonthValid, ref isYearValid, isValidDateOfBirthValid, isDateOfBirthValid, isGroupValid, fieldset);

            var dateInput = new TagBuilder("div");
            dateInput.AddCssClass("govuk-date-input");

            var dayContainer = new TagBuilder("div");
            dayContainer.AddCssClass("govuk-date-input__item");

            var dayGroup = new TagBuilder("div");
            dayGroup.AddCssClass("govuk-form-group");

            var dayLabelBuilder = Generator.GenerateLabel(
                 ViewContext,
                 Day.ModelExplorer,
                 Day.Name,
                 labelText: null,
                 htmlAttributes: new Dictionary<string, object> { { "id", Id + "-day-label" }, { "for", Id + "-day-input" } });
            dayLabelBuilder.AddCssClass("govuk-label govuk-date-input__label");
            dayGroup.InnerHtml.AppendHtml(dayLabelBuilder);

            var dayHtmlAttributes = new Dictionary<string, object>{
                    {"id", Id + "-day-input" },
                    {"type","text" },
                    {"pattern","[0-9]*" },
                    {"inputmode","numeric" },
                    {"maxlength", "2" },
                    { "title", Day.Metadata.DisplayName }
            };

            if (IsAutoFocus)
            {
                dayHtmlAttributes.Add("autofocus", IsAutoFocus);
            }

            var dayInputBuilder = Generator.GenerateTextBox(
                ViewContext,
                Day.ModelExplorer,
                Day.Name,
                Day.ModelExplorer.Model,
                format: null,
                htmlAttributes: dayHtmlAttributes);
            dayInputBuilder.AddCssClass($"govuk-input govuk-input--width-2 {(!isDayValid ? "govuk-input--error" : string.Empty)}");
            dayGroup.InnerHtml.AppendHtml(dayInputBuilder);

            dayContainer.InnerHtml.AppendHtml(dayGroup);

            var monthContainer = new TagBuilder("div");
            monthContainer.AddCssClass("govuk-date-input__item");

            var monthGroup = new TagBuilder("div");
            monthGroup.AddCssClass("govuk-form-group");

            var monthLabelBuilder = Generator.GenerateLabel(
                 ViewContext,
                 Month.ModelExplorer,
                 Month.Name,
                 labelText: null,
                 htmlAttributes: new Dictionary<string, object> { { "id", Id + "-month-label" }, { "for", Id + "-month-input" } });
            monthLabelBuilder.AddCssClass("govuk-label govuk-date-input__label");
            monthGroup.InnerHtml.AppendHtml(monthLabelBuilder);

            var monthInputBuilder = Generator.GenerateTextBox(
                ViewContext,
                Month.ModelExplorer,
                Month.Name,
                Month.ModelExplorer.Model,
                format: null,
                htmlAttributes: new Dictionary<string, object>{
                    {"id", Id + "-month-input" },
                    {"type","text" },
                    {"pattern","[0-9]*" },
                    {"inputmode","numeric" },
                    {"maxlength", "2" },
                    { "title", Month.Metadata.DisplayName }
            });
            monthInputBuilder.AddCssClass($"govuk-input govuk-input--width-2 {(!isMonthValid ? "govuk-input--error" : string.Empty)}");
            monthGroup.InnerHtml.AppendHtml(monthInputBuilder);

            monthContainer.InnerHtml.AppendHtml(monthGroup);

            var yearContainer = new TagBuilder("div");
            yearContainer.AddCssClass("govuk-date-input__item");

            var yearGroup = new TagBuilder("div");
            yearGroup.AddCssClass("govuk-form-group");

            var yearLabelBuilder = Generator.GenerateLabel(
                 ViewContext,
                 Year.ModelExplorer,
                 Year.Name,
                 labelText: null,
                 htmlAttributes: new Dictionary<string, object> { { "id", Id + "-year-label" }, { "for", Id + "-year-input" } });
            yearLabelBuilder.AddCssClass("govuk-label govuk-date-input__label");
            yearGroup.InnerHtml.AppendHtml(yearLabelBuilder);

            var yearInputBuilder = Generator.GenerateTextBox(
                ViewContext,
                Year.ModelExplorer,
                Year.Name,
                Year.ModelExplorer.Model,
                format: null,
                htmlAttributes: new Dictionary<string, object>{
                    {"id", Id + "-year-input" },
                    {"type","text" },
                    {"pattern","[0-9]*" },
                    {"inputmode","numeric" },
                    {"maxlength", "4" },
                    { "title", Year.Metadata.DisplayName }
            });
            yearInputBuilder.AddCssClass($"govuk-input govuk-input--width-4 {(!isYearValid ? "govuk-input--error" : string.Empty)}");
            yearGroup.InnerHtml.AppendHtml(yearInputBuilder);

            yearContainer.InnerHtml.AppendHtml(yearGroup);

            dateInput.InnerHtml.AppendHtml(dayContainer);
            dateInput.InnerHtml.AppendHtml(monthContainer);
            dateInput.InnerHtml.AppendHtml(yearContainer);

            fieldset.InnerHtml.AppendHtml(dateInput);
            output.Content.SetHtmlContent(fieldset);
        }

        protected void HandleErrorSpan(ref bool isDayValid, ref bool isMonthValid, ref bool isYearValid,
            bool isValidDateOfBirthValid, ModelStateEntry isDateOfBirthValid, bool isGroupValid, TagBuilder fieldset)
        {
            if (IsValidDateOfBirth.Model != null && !(bool)IsValidDateOfBirth.Model)
            {
                CreateErrorSpan(fieldset, isDateOfBirthValid?.Errors.FirstOrDefault().ErrorMessage);
                return;
            }
            if (!isGroupValid)
            {
                CreateErrorSpan(fieldset, ValidationMessage);
                return;
            }
            if (!isValidDateOfBirthValid)
            {
                CreateErrorSpan(fieldset, isDateOfBirthValid?.Errors.FirstOrDefault().ErrorMessage);
                isDayValid = false;
                isMonthValid = false;
                isYearValid = false;
                return;
            }
        }

        protected void CreateErrorSpan(TagBuilder fieldset, string errorMessage)
        {
            var validation = new TagBuilder("span");
            validation.MergeAttribute("id", Id + "-validation");
            validation.InnerHtml.Append(errorMessage);
            validation.AddCssClass("govuk-error-message");
            fieldset.InnerHtml.AppendHtml(validation);
        }
    }
}

