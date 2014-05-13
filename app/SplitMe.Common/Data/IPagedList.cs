using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitMe.Common.Data
{
    public interface IPagedList<T> : IList<T>
    {
        /// <summary>
        /// Pages in the collection
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Total count of items in the collection
        /// </summary>
        int TotalItemCount { get; }

        /// <summary>
        /// Index of the page to return
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// Number of the page of the index
        /// </summary>
        int PageNumber { get; }

        /// <summary>
        /// Number of items on the page
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Determines if the collection has previous pages
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// Determines if the collection contains more pages
        /// </summary>
        bool HasNextPage { get; }

        /// <summary>
        /// Determines if the current page is the first page
        /// </summary>
        bool IsFirstPage { get; }

        /// <summary>
        /// Determines if the current page is the last page
        /// </summary>
        bool IsLastPage { get; }

    }
}
