using System;

namespace Insolvency.IntegrationAPI
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class SwaggerGroupAttribute : Attribute
    {
        public SwaggerGroupAttribute(params string[] version)
        {
            Versions = version;
        }

        public string[] Versions { get; set; }
    }
}