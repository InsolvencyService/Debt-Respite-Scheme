using System;
using System.Text;

namespace Insolvency.Portal.Models
{
    public class Pagination
    {
        private const string DisabledCssClass = "disabled";
        private const string ActiveCssClass = "active";
        private const string AriaDisableHtml = "aria-disabled='true'";
        private const int MaxPageDisplay = 5;

        private int PageSize { get; }

        private string SubmitPropName { get; }

        private decimal TotalPageDecimal => decimal.Divide(TotalItems, PageSize);

        private int TotalPages => (int)Math.Ceiling(TotalPageDecimal);

        private int CurrentTotalPageItems { get; }

        private int PreviousPageNo
        {
            get
            {
                var pageNo = CurrentPageNo;
                pageNo--;

                if (pageNo < 1)
                {
                    pageNo = 1;
                }

                return pageNo;
            }
        }

        private int NexPageNo
        {
            get
            {
                var pageNo = CurrentPageNo;
                
                pageNo++;

                return pageNo;
            }
        }

        private bool IsFirstPage => CurrentPageNo <= 1;

        private bool IsLastPage => CurrentPageNo >= LastPageNo;

        private int CurrentPageNo { get; }

        private int LastPageNo => (int) TotalPages;

        public int CurrentStartIndex
        {
            get
            {
                var currIndex = 1;

                if (CurrentPageNo > 1)
                    currIndex = (PageSize * (CurrentPageNo - 1)) + 1;

                return currIndex;
            }
        }

        public int CurrentEndIndex
        {
            get
            {
                var lastIndex = CurrentStartIndex + (PageSize - 1);

                if (CurrentTotalPageItems < PageSize)
                    lastIndex = CurrentStartIndex + (CurrentTotalPageItems - 1);

                return lastIndex;
            }
        }

        public long TotalItems { get; private set; }

        public bool ShowPagingInfo => TotalPages != 1 && TotalPages != 0;

        public string PaginationHtml
        {
            get
            {
                if (!ShowPagingInfo)
                    return string.Empty;

                var htmlAsString = new StringBuilder();
                var isFirstPageDisabledCss = IsFirstPage ? DisabledCssClass : string.Empty;
                var isLastPageDisabledCss = IsLastPage ? DisabledCssClass : string.Empty;
                var ariaPrev = IsFirstPage ? AriaDisableHtml : string.Empty;
                var ariaNext = IsLastPage ? AriaDisableHtml : string.Empty;

                var previousPageString = $"<li>" +
                                         $"<button id='btn-previous' {ariaPrev} type='submit' class='{isFirstPageDisabledCss}' name='{SubmitPropName}' value='{PreviousPageNo}' {isFirstPageDisabledCss} >" +
                                         "<< Previous " +
                                         "</button>" +
                                         "</li>";
                var nextPageString = $"<li>" +
                                     $"<button id='btn-next' {ariaNext} type='submit' class='{isLastPageDisabledCss}' name='{SubmitPropName}' value='{NexPageNo}' {isLastPageDisabledCss} >" +
                                     " Next >>" +
                                     "</button>" +
                                     "</li>";

                htmlAsString.Append("<nav role='navigation' aria-label='pagination'>" +
                                    "<div id='page-container' class='bss-pagination-container'>" +
                                    "<ul class='pagination'>");

                // previous page link
                htmlAsString.Append(previousPageString);

                var pageNoStart = 1;
                var pageCount = 1;

                var totalDisplayPages = (TotalPages - CurrentPageNo) + (!IsLastPage ? 1 : 0);
                var totalMissingPages = (MaxPageDisplay - totalDisplayPages) - (IsLastPage ? 1 : 0);
                var shouldDisplayPrevPages = (totalDisplayPages < MaxPageDisplay
                                           && TotalPages >= MaxPageDisplay);
                if (TotalPages > 5 && CurrentPageNo > 1)
                    pageNoStart = CurrentPageNo;

                if (CurrentPageNo >= 3)
                    pageNoStart = CurrentPageNo - 2;
                
                if (shouldDisplayPrevPages)
                    pageNoStart = CurrentPageNo - totalMissingPages;

                for (int pageNo = pageNoStart; pageNo <= TotalPages; pageNo++)
                {
                    if (pageCount > MaxPageDisplay)
                        break;

                    var isActiveCss = CurrentPageNo == pageNo ? ActiveCssClass : string.Empty;
                    var isDisabled = CurrentPageNo == pageNo ? DisabledCssClass : string.Empty;
                    var aria = CurrentPageNo == pageNo ? AriaDisableHtml : string.Empty;

                    htmlAsString.Append($"<li>" +
                                        $"<button id='btn-{pageNo}' {aria} type='submit' class='{isActiveCss}' name='{SubmitPropName}' value='{pageNo}' {isDisabled} >{pageNo}</button>" +
                                        "</li>");

                    pageCount++;
                }

                // next page link
                htmlAsString.Append(nextPageString);

                htmlAsString.Append("</ul></div></nav>");

                return htmlAsString.ToString();
            }
        }

        private Pagination()
        {
        }

        public Pagination(
            long totalItems,
            string pageNumberPropName,
            int currentTotalPageItems,
            int currentPage = 1,
            int pageSize = 10
        )
        {
            if (string.IsNullOrEmpty(pageNumberPropName))
                throw new ArgumentNullException(nameof(pageNumberPropName));

            TotalItems = totalItems;
            PageSize = pageSize;
            CurrentPageNo = currentPage;
            SubmitPropName = pageNumberPropName;
            CurrentTotalPageItems = currentTotalPageItems;
        }
    }
}