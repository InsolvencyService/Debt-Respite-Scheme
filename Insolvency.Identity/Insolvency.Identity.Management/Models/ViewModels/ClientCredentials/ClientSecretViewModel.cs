using System;
using IdentityServer4.EntityFramework.Entities;

namespace Insolvency.Management.Models.ViewModels.ClientCredentials
{
    public class ClientSecretViewModel
    {
        public ClientSecretViewModel()
        {

        }
        public ClientSecretViewModel(ClientSecret clientSecret)
        {
            this.Id = clientSecret.Id;
            this.Description = clientSecret.Description;
            this.Expiration = clientSecret.Expiration;
            this.Type = clientSecret.Type;
            this.DateCreated = clientSecret.Created;
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime? Expiration { get; set; }
        public string Type { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
