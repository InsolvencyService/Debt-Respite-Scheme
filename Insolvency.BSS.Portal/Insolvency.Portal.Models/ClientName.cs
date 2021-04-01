using System;

namespace Insolvency.Portal.Models
{
    public class ClientName
    {
        public Guid NameId { get; set; }
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string GetFullName()
        {
            var fullName = string.Empty;

            if (!string.IsNullOrWhiteSpace(FirstName))
            {
                fullName += $"{FirstName}";
            }

            if (!string.IsNullOrWhiteSpace(MiddleName))
            {
                fullName += $" {MiddleName}";
            }

            if (!string.IsNullOrWhiteSpace(LastName))
            {
                fullName += $" {LastName}";
            }

            return fullName;
        }
    }
}
