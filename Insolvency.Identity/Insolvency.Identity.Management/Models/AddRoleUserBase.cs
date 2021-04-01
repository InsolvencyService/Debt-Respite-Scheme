using System.ComponentModel.DataAnnotations;
using Insolvency.Identity.Models;

namespace Insolvency.Management.Models
{
    public class AddRoleUserBase
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public RoleType Role { get; set; }
    }
}
