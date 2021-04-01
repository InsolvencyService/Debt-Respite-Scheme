using System.ComponentModel.DataAnnotations;

namespace Insolvency.Management.Models.ViewModels.ClientCredentials
{
    public class AddClientSecretViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
