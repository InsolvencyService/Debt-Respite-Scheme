using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Insolvency.Integration.Models.Shared.Responses;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insolvency.Portal.Models.ViewModels
{
    public class CreditorSearchResultsViewModel
    {
        public CreditorSearchResultsViewModel() { }
        public List<SelectListItem> Creditors { get; set; }

        [Required(ErrorMessage = "Please select a creditor")]
        [Display(Name = "Choose a creditor")]
        public virtual Guid? SelectedCreditor { get; set; }

        public Guid DebtId { get; set; }
        public string ReturnAction { get; set; }

        public void MapCreditors(CreditorSearchResponse searchResult)
        {
            var result = new List<SelectListItem>();

            if (searchResult == null || searchResult.Count == 0)
            {
                result.Add(new SelectListItem("No creditors found", "", true, true));
            }
            else
            {
                result.Add(new SelectListItem($"{searchResult.Count} creditors found", "", true, true));
                result.AddRange(searchResult.Select(x => new SelectListItem(x.Name, x.Id.ToString(), false, false)));
            }

            Creditors = result;
        }
    }
}
