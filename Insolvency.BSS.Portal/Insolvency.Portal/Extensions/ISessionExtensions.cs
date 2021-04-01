using Microsoft.AspNetCore.Http;
using Insolvency.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insolvency.Common;

namespace Insolvency.Portal.Extensions
{
    public static class ISessionExtensions
    {
        public static void SetOrganisation(this ISession session, OrganisationModel organisation)
        {
            var organisationValue = System.Text.Json.JsonSerializer.Serialize(organisation);
            session.SetString(Constants.OrganisationItemsKey, organisationValue);
        }

        public static OrganisationModel GetOrganisation(this ISession session)
        {
            var organisationValue = session.GetString(Constants.OrganisationItemsKey);
            if (string.IsNullOrEmpty(organisationValue))
            {
                return null;
            }

            return System.Text.Json.JsonSerializer.Deserialize<OrganisationModel>(organisationValue);
        }
    }
}
