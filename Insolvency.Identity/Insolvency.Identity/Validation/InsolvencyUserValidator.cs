using FluentValidation;
using Insolvency.Identity.Models;

namespace Insolvency.Identity.Validation
{
    public class InsolvencyUserValidator : AbstractValidator<InsolvencyUser>
    {
        public InsolvencyUserValidator()
        {
            RuleFor(p => p.Email).NotEmpty();
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.ScpGroupId).NotEmpty();
        }
    }
}
