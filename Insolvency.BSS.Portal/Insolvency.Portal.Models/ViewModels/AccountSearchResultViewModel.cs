using System.Collections.Generic;

namespace Insolvency.Portal.Models.ViewModels
{
    public class AccountSearchResultViewModel
    {
        public List<SearchResultViewModel> SearchResultView { get; set; }
        public SearchParameterViewModel SearchParameterView { get; set; }
        public Pagination SearchPagination { get; set; }
    }
}
