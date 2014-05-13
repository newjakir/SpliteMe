using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitMe.Common.Data
{
    public static class IEnumerableExtensions
    {
        #region IEnumerable<T> extensions
        /// <summary>
        /// Paged List Extension
        /// </summary>
        /// <typeparam name="T">Generic type in the list</typeparam>
        /// <param name="source">Source collection to convert to PagedList</param>
        /// <param name="pageIndex">Page Index to return</param>
        /// <param name="pageSize">How many items on the page to return</param>
        /// <returns>A page specified by the index from the source split into the pages by the size.</returns>
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

        /// <summary>
        /// Paged List Extension
        /// </summary>
        /// <typeparam name="T">Generic type in the list</typeparam>
        /// <param name="result">PagedResult containing information for the PagedList</param>
        /// <returns>A page specified by the index from the source split into the pages by the size.</returns>
        public static IPagedList<T> ToPagedList<T>(this PagedResult<T> result)
        {
            return new PagedList<T>(result.Results, result.CurrentPage, result.PageSize, result.TotalRows);
        }

        /// <summary>
        /// Paged List Extension 
        /// </summary>
        /// <typeparam name="T">Generic type in the list</typeparam>
        /// <param name="source">Source collection to convert to PagedList</param>
        /// <param name="pageIndex">Page Index to return</param>
        /// <param name="pageSize">How many items on the page to return</param>
        /// <param name="totalCount">Total count of items in the list</param>
        /// <returns>A page specified by the index from the source split into the pages by the size.</returns>
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion
    }
}
