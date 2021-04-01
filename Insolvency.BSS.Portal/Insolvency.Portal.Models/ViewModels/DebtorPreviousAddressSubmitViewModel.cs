using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorPreviousAddressSubmitViewModel :  AddressWithValidation
    {
        [Range(1, 12)]
        [Display(Name = "Month")]
        public int? MonthFrom { get; set; }

        [Range(1880, 2200)]
        [Display(Name = "Year")]
        public int? YearFrom { get; set; }

        [Range(1, 12)]
        [Display(Name = "Month")]
        public int? MonthTo { get; set; }


        [Range(1880, 2200)]
        [Display(Name = "Year")]
        public int? YearTo { get; set; }

        [Required(ErrorMessage = "The move into date must include a valid month and year")]
        [DisplayFormat(DataFormatString = "{0:yyyy-M-d}")]
        [Range(typeof(DateTime), "1880/1/1", "2200/12/31", ErrorMessage = "The move into date must include a valid month and year")]
        public DateTime? MoveInDate
        {
            get
            {
                return GetDateFromMonthAndYear(YearFrom, MonthFrom);
            }
        }

        public string ReturnAction { get; set; }

        [Required(ErrorMessage = "The move out date must include a valid month and year")]
        [DisplayFormat(DataFormatString = "{0:yyyy-M-d}")]
        [Range(typeof(DateTime), "1880/1/1", "2200/12/31", ErrorMessage = "The move out date must include a valid month and year")]
        public DateTime? MoveOutDate
        {
            get
            {
                return GetDateFromMonthAndYear(YearTo, MonthTo);
            }
        }

        public void SetFromToDate()
        {
            this.DateFrom = MoveInDate;
            this.DateTo = MoveOutDate;
        }

        private DateTime? GetDateFromMonthAndYear(int? year, int? month)
        {
            if (year.HasValue && month.HasValue)
            {
                var input = $"{year}-{month}-1";
                if (DateTime.TryParseExact(input, "yyyy-M-d", CultureInfo.CurrentCulture, DateTimeStyles.None, out var result))
                    return result;
            }
            return null;
        }
    }
}
