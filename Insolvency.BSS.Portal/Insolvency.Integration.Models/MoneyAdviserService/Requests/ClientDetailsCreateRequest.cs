using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common;
using Insolvency.Common.Attributes;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class ClientDetailsCreateRequest : IBornEntity
    {
        [Required(ErrorMessage = "Enter First name")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Enter Last name")]
        public string LastName { get; set; }

        [DateOfBirthValidation]
        public DateTime DateOfBirth { get; set; }

        public DateTime GetBirthDate() => DateOfBirth;

        [JsonIgnore]
        public bool IsValidDateOfBirth { get; set; }

        public virtual Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>
            {
                { "FirstName", FirstName },
                { "MiddleName", MiddleName },
                { "LastName", LastName },
                { "DateOfBirth", DateOfBirth }
            };
            return dictionary;
        }
    }
}
