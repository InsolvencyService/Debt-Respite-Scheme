using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Insolvency.Portal.TagHelpers
{
    [HtmlTargetElement("div", Attributes = ValidationSummaryAttributeName)]
    public class BssErrorSummaryTagHelper : TagHelper
    {
        private const string HiddenListItem = @"<li style=""display:none""></li>";
        private const string ValidationSummaryAttributeName = "bss-error-summary";
        private ValidationSummary _validationSummary;

        public IDictionary<string, string> IdOverwrite { get; set; } = new Dictionary<string, string>();

        public BssErrorSummaryTagHelper(IHtmlGenerator generator) => Generator = generator;

        public override int Order => -1000;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeNotBound]
        protected IHtmlGenerator Generator { get; }

        [HtmlAttributeName(ValidationSummaryAttributeName)]
        public ValidationSummary ValidationSummary
        {
            get => _validationSummary;
            set
            {
                switch (value)
                {
                    case ValidationSummary.All:
                    case ValidationSummary.ModelOnly:
                    case ValidationSummary.None:
                        _validationSummary = value;
                        break;

                    default:
                        throw new ArgumentException(nameof(value));
                }
            }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (ValidationSummary == ValidationSummary.None)
            {
                return;
            }

            var tagBuilder = BssErrorSummaryTagHelper.GenerateValidationSummary(
                ViewContext,
                IdOverwrite,
                excludePropertyErrors: ValidationSummary == ValidationSummary.ModelOnly,
                message: null,
                headerTag: null,
                htmlAttributes: null);
            if (tagBuilder == null)
            {
                // The generator determined no element was necessary.
                output.SuppressOutput();
                return;
            }

            output.MergeAttributes(tagBuilder);
            if (tagBuilder.HasInnerHtml)
            {
                output.PostContent.AppendHtml(tagBuilder.InnerHtml);
            }
        }

        public static TagBuilder GenerateValidationSummary(
            ViewContext viewContext,
            IDictionary<string, string> idOverwrite,
            bool excludePropertyErrors,
            string message,
            string headerTag,
            object htmlAttributes)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException(nameof(viewContext));
            }

            var viewData = viewContext.ViewData;
            if (!viewContext.ClientValidationEnabled && viewData.ModelState.IsValid)
            {
                // Client-side validation is not enabled to add to the generated element and element will be empty.
                return null;
            }

            if (excludePropertyErrors &&
                (!viewData.ModelState.TryGetValue(viewData.TemplateInfo.HtmlFieldPrefix, out var entryForModel) ||
                 entryForModel.Errors.Count == 0))
            {
                // Client-side validation (if enabled) will not affect the generated element and element will be empty.
                return null;
            }

            TagBuilder messageTag;
            if (string.IsNullOrEmpty(message))
            {
                messageTag = null;
            }
            else
            {
                if (string.IsNullOrEmpty(headerTag))
                {
                    headerTag = viewContext.ValidationSummaryMessageElement;
                }

                messageTag = new TagBuilder(headerTag);
                messageTag.InnerHtml.SetContent(message);
            }

            // If excludePropertyErrors is true, describe any validation issue with the current model in a single item.
            // Otherwise, list individual property errors.
            var isHtmlSummaryModified = false;
            var modelStates = GetModelStateList(viewData, excludePropertyErrors);

            var htmlSummary = new TagBuilder("ul");
            htmlSummary.AddCssClass("govuk-list govuk-error-summary__list");
            var j = 0;
            foreach (var modelState in modelStates)
            {
                j++;
                // Perf: Avoid allocations
                for (var i = 0; i < modelState.Value.Errors.Count; i++)
                {
                    var modelError = modelState.Value.Errors[i];
                    var errorText = GetModelErrorMessageOrDefault(modelError);

                    if (!string.IsNullOrEmpty(errorText))
                    {
                        var targetId = $"{modelState.Key}-input";
                        if (idOverwrite.TryGetValue(modelState.Key, out var overwriteValue))
                            targetId = overwriteValue;

                        var linkItem = new TagBuilder("a");
                        linkItem.Attributes.Add("href", $"#{targetId}");
                        linkItem.AddCssClass("govuk-link");
                        linkItem.Attributes.Add("id", $"error-list-item-{targetId}-{j}");

                        var listItem = new TagBuilder("li");
                        linkItem.InnerHtml.SetContent(errorText);
                        listItem.InnerHtml.AppendHtml(linkItem);

                        htmlSummary.InnerHtml.AppendLine(listItem);
                        isHtmlSummaryModified = true;
                    }
                }
            }

            if (!isHtmlSummaryModified)
            {
                htmlSummary.InnerHtml.AppendHtml(HiddenListItem);
                htmlSummary.InnerHtml.AppendLine();
            }

            var tagBuilder = new TagBuilder("div");
            tagBuilder.MergeAttributes(GetHtmlAttributeDictionaryOrNull(htmlAttributes));

            if (viewData.ModelState.IsValid)
            {
                tagBuilder.AddCssClass(HtmlHelper.ValidationSummaryValidCssClassName);
            }
            else
            {
                tagBuilder.AddCssClass(HtmlHelper.ValidationSummaryCssClassName);
            }

            if (messageTag != null)
            {
                tagBuilder.InnerHtml.AppendLine(messageTag);
            }

            tagBuilder.InnerHtml.AppendHtml(htmlSummary);

            if (viewContext.ClientValidationEnabled && !excludePropertyErrors)
            {
                // Inform the client where to replace the list of property errors after validation.
                tagBuilder.MergeAttribute("data-valmsg-summary", "true");
            }

            return tagBuilder;

            static string GetModelErrorMessageOrDefault(ModelError modelError)
            {
                Debug.Assert(modelError != null);

                if (!string.IsNullOrEmpty(modelError.ErrorMessage))
                {
                    return modelError.ErrorMessage;
                }

                // Default in the ValidationSummary case is no error message.
                return string.Empty;
            }
            static IDictionary<string, object> GetHtmlAttributeDictionaryOrNull(object htmlAttributes)
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

        // Returns non-null list of model states, which caller will render in order provided.
        public static IDictionary<string, ModelStateEntry> GetModelStateList(
            ViewDataDictionary viewData,
            bool excludePropertyErrors)
        {
            if (excludePropertyErrors)
            {
                viewData.ModelState.TryGetValue(viewData.TemplateInfo.HtmlFieldPrefix, out var ms);

                if (ms != null)
                {
                    return new Dictionary<string, ModelStateEntry> { { viewData.TemplateInfo.HtmlFieldPrefix, ms } };
                }
            }
            else if (viewData.ModelState.Count > 0)
            {
                var metadata = viewData.ModelMetadata;
                var modelStateDictionary = viewData.ModelState;
                var entries = new Dictionary<string, ModelStateEntry>();
                Visit(modelStateDictionary.Root, metadata, entries);

                if (entries.Count < modelStateDictionary.Count)
                {
                    // Account for entries in the ModelStateDictionary that do not have corresponding ModelMetadata values.
                    foreach (var entry in modelStateDictionary)
                    {
                        if (!entries.Keys.Contains(entry.Key))
                        {
                            entries.Add(entry.Key, entry.Value);
                        }
                    }
                }

                return entries;
            }

            return new Dictionary<string, ModelStateEntry>();

            static void Visit(
               ModelStateEntry modelStateEntry,
               ModelMetadata metadata,
               IDictionary<string, ModelStateEntry> orderedModelStateEntries, string propertyName = null)
            {
                if (metadata.ElementMetadata != null && modelStateEntry.Children != null)
                {
                    foreach (var indexEntry in modelStateEntry.Children)
                    {
                        Visit(indexEntry, metadata.ElementMetadata, orderedModelStateEntries);
                    }
                }
                else
                {
                    for (var i = 0; i < metadata.Properties.Count; i++)
                    {
                        var propertyMetadata = metadata.Properties[i];
                        var propertyModelStateEntry = modelStateEntry.GetModelStateForProperty(propertyMetadata.PropertyName);
                        if (propertyModelStateEntry != null)
                        {
                            Visit(propertyModelStateEntry, propertyMetadata, orderedModelStateEntries, propertyMetadata.PropertyName);
                        }
                    }
                }

                if (!modelStateEntry.IsContainerNode)
                {
                    orderedModelStateEntries.Add(propertyName, modelStateEntry);
                }
            }
        }
    }
}
