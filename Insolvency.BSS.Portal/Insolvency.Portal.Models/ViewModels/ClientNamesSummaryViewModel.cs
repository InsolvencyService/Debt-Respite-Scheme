using System.Collections.Generic;
using System.Linq;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class ClientNamesSummaryViewModel
    {
        public ClientNamesSummaryViewModel() { }
        public ClientNamesSummaryViewModel(DebtorDetailsResponse debtorDetails)
        {
            CurrentName.FirstName = debtorDetails.FirstName;
            CurrentName.MiddleName = debtorDetails.MiddleName;
            CurrentName.LastName = debtorDetails.LastName;
            CurrentNameDisplay = CurrentName.GetFullName();

            if (debtorDetails.PreviousNames != null && debtorDetails.PreviousNames.Any())
            {
                PreviousNames = debtorDetails.PreviousNames.Select(x => new ClientName
                {
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    NameId = x.Id
                });

                PreviousNamesDisplay = PreviousNames.Select(x => x.GetFullName());
            }
        }

        public string CurrentNameDisplay { get; set; }
        public IEnumerable<string> PreviousNamesDisplay { get; set; }
        private ClientName CurrentName { get; set; } = new ClientName();
        public IEnumerable<ClientName> PreviousNames { get; set; }
    }
}
