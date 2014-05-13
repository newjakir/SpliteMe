using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitMe.Common.Data
{
    public class PagedResult<T>
    {
        /// <summary>
        /// Total rows in result set
        /// </summary>
        public int TotalRows { get; set; }
        /// <summary>
        /// Current page requested from result
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// Page size of the requested result
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// If request has another page
        /// </summary>
        public bool HasNext { get; set; }
        /// <summary>
        /// If request has previous page
        /// </summary>
        public bool HasPrevious { get; set; }
        /// <summary>
        /// The actual collection from the request
        /// </summary>
        public IEnumerable<T> Results { get; set; }

    }
}
