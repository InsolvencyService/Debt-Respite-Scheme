using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Insolvency.IntegrationAPI
{
    public class SwaggerAddEnumDescriptions : IDocumentFilter
    {
        protected static List<Type> Enums { get; set; }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var schemaEnums = swaggerDoc.Components.Schemas.Where(x => x.Value.Enum != null && x.Value.Enum.Count > 0).ToList();
            var enums = schemaEnums.ToDictionary(x => GetEnumType(x.Key), x => x.Value);
            // add enum descriptions to result models
            foreach (KeyValuePair<Type, OpenApiSchema> schemaDictionaryItem in enums)
            {
                var schema = schemaDictionaryItem.Value;
                if (schema.Enum != null && schema.Enum.Count > 0)
                {
                    schema.Description += DescribeEnum(schemaDictionaryItem.Key, schema.Enum);
                }
                foreach (KeyValuePair<string, OpenApiSchema> propertyDictionaryItem in schema.Properties)
                {
                    var property = propertyDictionaryItem.Value;
                    var propertyEnums = property.Enum;
                    if (propertyEnums != null && propertyEnums.Count > 0)
                    {
                        property.Description += DescribeEnum(schemaDictionaryItem.Key, propertyEnums);
                    }
                }
            }

            // add enum descriptions to input parameters
            if (swaggerDoc.Paths.Count > 0)
            {
                foreach (var pathItem in swaggerDoc.Paths.Values)
                {
                    DescribeEnumParameters(pathItem.Parameters);

                    // head, patch, options, delete left out
                    var possibleParameterisedOperations = pathItem.Operations
                        .Where(x => x.Key == OperationType.Get || x.Key == OperationType.Post || x.Key == OperationType.Put)
                        .Select(x => x.Value)
                        .ToList();
                    possibleParameterisedOperations.FindAll(x => x != null).ForEach(x => DescribeEnumParameters(x.Parameters));
                }
            }
        }

        protected virtual void DescribeEnumParameters(IList<OpenApiParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var paramEnums = param.Schema.Enum;
                    if (paramEnums != null && paramEnums.Count > 0)
                    {
                        param.Description += DescribeEnum(GetEnumType(param.Schema.Type),paramEnums);
                    }
                }
            }
        }

        protected virtual string DescribeEnum(Type enumType, IList<IOpenApiAny> enums)
        {
            List<string> enumDescriptions = new List<string>();
            foreach (var enumOption in enums.Cast<OpenApiInteger>())
            {
                enumDescriptions.Add(string.Format("{0} = {1}", enumOption.Value, Enum.GetName(enumType, enumOption.Value)));
            }
            return string.Join(", ", enumDescriptions.ToArray());
        }

        protected virtual Type GetEnumType(string enumName)
        {
            if (Enums == null)
            {
                Enums = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes().Where(y => y.IsEnum).ToList()).ToList();
            }

            var foundEnums = Enums.Where(x => x.Name == enumName).ToList();

            if (foundEnums.Count == 0 || foundEnums.Count > 1)
            {
                throw new InvalidOperationException("Enums should be uniquely named, so that we can easily identify the Type without the Type.FullName.");
            }

            return foundEnums.First();
        }
    }
}
