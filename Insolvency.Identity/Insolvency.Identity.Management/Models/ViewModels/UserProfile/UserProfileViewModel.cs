using System.Collections.Generic;
using Insolvency.Identity.Models;

namespace Insolvency.Management.Models.ViewModels.UserProfile
{
    public class UserProfileViewModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public List<Organisation> Organisations { get; set; }
        public string ManageGroupProfileUrl { get; set; }
        public string ManageProfileUrl { get; set; }
        public bool IsAdmin { get; set; }
    }
}
