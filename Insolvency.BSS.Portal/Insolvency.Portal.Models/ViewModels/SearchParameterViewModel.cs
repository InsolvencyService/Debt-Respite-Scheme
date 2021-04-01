namespace Insolvency.Portal.Models.ViewModels
{
    public class SearchParameterViewModel
    {
        private string _referenceFilter;
        private string _surnameFilter;
        private string _dobFilter;

        private string ReferenceFilter
        {
            get
            {
                if (string.IsNullOrEmpty(_referenceFilter))
                    return string.Empty;
                else return $"\"{_referenceFilter}\"";
            }
            set => _referenceFilter = value;
        }

        private string SurnameFilter
        {
            get
            {
                if (string.IsNullOrEmpty(_surnameFilter))
                {
                    return string.Empty;
                }
                else
                {
                    var value = $"\"{_surnameFilter}\"";

                    if (!string.IsNullOrEmpty(ReferenceFilter))
                    {
                        value = $", {value}";
                    }

                    return value;
                }
            }
            set => _surnameFilter = value;
        }

        private string DobFilter
        {
            get
            {
                if (string.IsNullOrEmpty(_dobFilter))
                {
                    return string.Empty;
                }
                else 
                {
                    var value = $"\"{_dobFilter}\"";

                    if (!string.IsNullOrEmpty(ReferenceFilter) || !string.IsNullOrEmpty(SurnameFilter))
                    {
                        value = $", {value}";
                    }

                    return value;
                };
            }
            set => _dobFilter = value;
        }

        public string FilterMessage => $"{ReferenceFilter}{SurnameFilter}{DobFilter}";

        public SearchParameterViewModel(string referenceFilter, string surnameFilter, string dobFilter)
        {
            ReferenceFilter = referenceFilter;
            SurnameFilter = surnameFilter;
            DobFilter = dobFilter;
        }
    }
}
