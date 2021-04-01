using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Insolvency.Portal.TagHelpers
{
    public class BssInputMonthYearTagHelper : TagHelper
    {
        public ModelExpression MonthExpression { get; set; }
        public ModelExpression YearExpression { get; set; }

        public string Id { get; set; }
        public string Label { get; set; }
        public string Hint { get; set; }
        public string ValidationMessage { get; set; }

        public int? Month { get; set; }
        public int? Year { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected IHtmlGenerator Generator { get; }

        public BssInputMonthYearTagHelper(IHtmlGenerator generator) => Generator = generator;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var isMonthValid = (!ViewContext.ViewData.ModelState.TryGetValue(MonthExpression.Name, out var month) || month.Errors.Count == 0) && (Month.HasValue || ViewContext.ViewData.ModelState.IsValid);
            var isYearValid = (!ViewContext.ViewData.ModelState.TryGetValue(YearExpression.Name, out var year) || year.Errors.Count == 0) && (Year.HasValue || ViewContext.ViewData.ModelState.IsValid);


            var isGroupValid = isMonthValid && isYearValid;

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
            output.Attributes.Clear();
            output.Attributes.SetAttribute("class", $"govuk-form-group {(!(isGroupValid) ? "govuk-form-group--error" : string.Empty)}");
            output.Attributes.SetAttribute("id", Id + "-container");

            var fieldset = new TagBuilder("fieldset");
            fieldset.AddCssClass("govuk-fieldset");
            fieldset.MergeAttribute("role", "group");
            fieldset.MergeAttribute("name", Id);

            var dobMainLabel = new TagBuilder("span");
            dobMainLabel.AddCssClass("govuk-label");
            dobMainLabel.InnerHtml.Append(Label);
            output.PreContent.AppendHtml(dobMainLabel);

            if (!string.IsNullOrEmpty(Hint))
            {
                var hint = new TagBuilder("span");
                hint.AddCssClass("govuk-hint");
                hint.MergeAttribute("id", Id + "-hint");
                hint.InnerHtml.Append(Hint);
                fieldset.InnerHtml.AppendHtml(hint);
            }

            HandleErrorSpan(isGroupValid, fieldset);

            var dateInput = new TagBuilder("div");
            dateInput.AddCssClass("govuk-date-input");

            var monthContainer = new TagBuilder("div");
            monthContainer.AddCssClass("govuk-date-input__item");

            var monthGroup = new TagBuilder("div");
            monthGroup.AddCssClass("govuk-form-group");

            var monthLabelBuilder = Generator.GenerateLabel(
                 ViewContext,
                 MonthExpression.ModelExplorer,
                 MonthExpression.Name,
                 labelText: null,
                 htmlAttributes: new Dictionary<string, object> { { "id", Id + "-month-label" }, { "for", Id + "-month-input" } });
            monthLabelBuilder.AddCssClass("govuk-label govuk-date-input__label");
            monthGroup.InnerHtml.AppendHtml(monthLabelBuilder);

            var monthInputBuilder = Generator.GenerateTextBox(
                ViewContext,
                MonthExpression.ModelExplorer,
                MonthExpression.Name,
                MonthExpression.ModelExplorer.Model,
                format: null,
                htmlAttributes: new Dictionary<string, object>{
                    {"id", Id + "-month-input" },
                    {"type","text" },
                    {"pattern","[0-9]*" },
                    {"inputmode","numeric" },
                    {"maxlength", "2" },
                    { "title", MonthExpression.Metadata.DisplayName }
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
                 YearExpression.ModelExplorer,
                 YearExpression.Name,
                 labelText: null,
                 htmlAttributes: new Dictionary<string, object> { { "id", Id + "-year-label" }, { "for", Id + "-year-input" } });
            yearLabelBuilder.AddCssClass("govuk-label govuk-date-input__label");
            yearGroup.InnerHtml.AppendHtml(yearLabelBuilder);

            var yearInputBuilder = Generator.GenerateTextBox(
                ViewContext,
                YearExpression.ModelExplorer,
                YearExpression.Name,
                YearExpression.ModelExplorer.Model,
                format: null,
                htmlAttributes: new Dictionary<string, object>{
                    {"id", Id + "-year-input" },
                    {"type","text" },
                    {"pattern","[0-9]*" },
                    {"inputmode","numeric" },
                    {"maxlength", "4" },
                    { "title", YearExpression.Metadata.DisplayName }
            });
            yearInputBuilder.AddCssClass($"govuk-input govuk-input--width-4 {(!isYearValid ? "govuk-input--error" : string.Empty)}");
            yearGroup.InnerHtml.AppendHtml(yearInputBuilder);

            yearContainer.InnerHtml.AppendHtml(yearGroup);

            dateInput.InnerHtml.AppendHtml(monthContainer);
            dateInput.InnerHtml.AppendHtml(yearContainer);

            fieldset.InnerHtml.AppendHtml(dateInput);
            output.Content.SetHtmlContent(fieldset);
        }

        protected void HandleErrorSpan(bool isGroupValid, TagBuilder fieldset)
        {

            if (!isGroupValid)
            {
                CreateErrorSpan(fieldset, ValidationMessage);
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


