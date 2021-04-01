using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Insolvency.Portal.TagHelpers
{
    public class BssInput : TagHelper
    {
        public ModelExpression AspFor { get; set; }
        public string Id { get; set; }
        public string InputClass { get; set; }
        public string LabelClass { get; set; }
        public string InputHint { get; set; }
        public string InputHintClass { get; set; }
        public bool IsAutoFocus { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }
        protected IHtmlGenerator Generator { get; }
        public BssInput(IHtmlGenerator generator) => Generator = generator;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var isFieldValid = true;
            if (ViewContext.ViewData.ModelState.TryGetValue(AspFor.Name, out var entry))
            {
                isFieldValid = entry.Errors.Count == 0;
            }

            var inputHtmlAttributes = output.Attributes.ToDictionary(
               x => x.Name,
               x => x.Value);

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
            output.Attributes.Clear();
            output.Attributes.SetAttribute("class", $"govuk-form-group {(!isFieldValid ? "govuk-form-group--error" : string.Empty)}");

            if (IsAutoFocus)
            {
                inputHtmlAttributes.Add("autofocus", IsAutoFocus);
            }

            if (Id is null)
            {
                Id = AspFor.Name;
            }
            output.Attributes.SetAttribute("id", Id + "-container");
            inputHtmlAttributes.Add("id", Id + "-input");
            inputHtmlAttributes.Add("title", Id);

            var labelBuilder = GenerateCustomLabel(ViewContext,
                AspFor.ModelExplorer,
                AspFor.Name,
                labelText: null,
                htmlAttributes: null,
                LabelClass,
                Id);
            output.PreContent.AppendHtml(labelBuilder);

            var validationId = Id + "-validation";
            var validationBuilder = Generator.GenerateValidationMessage(
                   ViewContext,
                   AspFor.ModelExplorer,
                   AspFor.Name,
                   message: string.Empty,
                   tag: null,
                   htmlAttributes: new Dictionary<string, object> { { "id", validationId } });
            validationBuilder.AddCssClass("govuk-error-message");
            output.PreContent.AppendHtml(validationBuilder);

            if (!string.IsNullOrEmpty(InputHint))
            {
                var hint = new TagBuilder("span");
                hint.AddCssClass($"govuk-hint {InputHintClass}");
                hint.MergeAttribute("id", Id + "-hint");
                hint.InnerHtml.Append(InputHint);
                output.PreContent.AppendHtml(hint);
            }

            var inputBuilder = Generator.GenerateTextBox(
                ViewContext,
                AspFor.ModelExplorer,
                AspFor.Name,
                AspFor.ModelExplorer.Model,
                format: null,
                htmlAttributes: inputHtmlAttributes);
            inputBuilder.AddCssClass($"govuk-input {(!isFieldValid ? "govuk-input--error" : string.Empty)}");
            if (!string.IsNullOrEmpty(InputClass))
            {
                inputBuilder.AddCssClass(InputClass);
            }
            output.Content.SetHtmlContent(inputBuilder);
        }

        private static TagBuilder GenerateCustomLabel(
            ViewContext viewContext,
            ModelExplorer modelExplorer,
            string expression,
            string labelText,
            object htmlAttributes,
            string additionalCss,
            string id)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException(nameof(viewContext));
            }

            if (modelExplorer == null)
            {
                throw new ArgumentNullException(nameof(modelExplorer));
            }

            var resolvedLabelText = labelText ??
                modelExplorer.Metadata.DisplayName ??
                modelExplorer.Metadata.PropertyName;
            if (resolvedLabelText == null && expression != null)
            {
                var index = expression.LastIndexOf('.');
                if (index == -1)
                {
                    // Expression does not contain a dot separator.
                    resolvedLabelText = expression;
                }
                else
                {
                    resolvedLabelText = expression.Substring(index + 1);
                }
            }

            TagBuilder innerTagBuilder = null;

            if (!string.IsNullOrWhiteSpace(additionalCss))
            {
                innerTagBuilder = new TagBuilder("strong");
                innerTagBuilder.InnerHtml.SetContent(resolvedLabelText);
            }

            var tagBuilder = new TagBuilder("label");
            var lablelClass = $"govuk-label {(string.IsNullOrWhiteSpace(additionalCss) ? "" : additionalCss)}";

            tagBuilder.Attributes.Add("for", id + "-input");
            tagBuilder.Attributes.Add("id", id + "-label");
            tagBuilder.AddCssClass(lablelClass);

            if (innerTagBuilder is null)
                tagBuilder.InnerHtml.SetContent(resolvedLabelText);
            else
                tagBuilder.InnerHtml.AppendHtml(innerTagBuilder);

            tagBuilder.MergeAttributes(GetHtmlAttributeDictionaryOrNull(htmlAttributes), replaceExisting: true);

            return tagBuilder;
        }

        private static IDictionary<string, object> GetHtmlAttributeDictionaryOrNull(object htmlAttributes)
        {
            IDictionary<string, object> htmlAttributeDictionary = null;
            if (htmlAttributes != null)
            {
                htmlAttributeDictionary = htmlAttributes as IDictionary<string, object>;
                if (htmlAttributeDictionary == null)
                {
                    htmlAttributeDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                }
            }

            return htmlAttributeDictionary;
        }
    }
}