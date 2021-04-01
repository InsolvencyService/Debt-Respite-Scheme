using System;
using System.ComponentModel.DataAnnotations;
using Insolvency.Identity.Models;

namespace Insolvency.Management.Models.ViewModels.RoleManagement
{
    public class EditRoleUserViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public RoleType Role { get; set; }
        public Guid OrganisationId { get; set; }

        public RoleUser MapToRoleUserEntity()
        {
            return new RoleUser
            {
                Id = Id,
                Email = this.Email,
                NormalisedEmail = this.Email.Trim().ToUpper(),
                Role = this.Role
            };
        }
    }
}
