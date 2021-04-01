using System.ComponentModel.DataAnnotations;

namespace Insolvency.Management.Models.ViewModels.ClientCredentials
{
    public class CreateClientCredentialsViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
