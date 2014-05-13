using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitMe.Common.Data
{
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        /// <summary>
        /// Specialized pagination list
        /// </summary>
        /// <param name="source">Collection to paginate</param>
        /// <param name="index">Page to return</param>
        /// <param name="pageSize">Number of items to split collection by</param>
        public PagedList(IEnumerable<T> source, int index, int pageSize)
            : this(source, index, pageSize, null)
        {
        }

        /// <summary>
        /// Specialized pagination list
        /// </summary>
        /// <param name="source">Collection to paginate</param>
        /// <param name="index">Page to return</param>
        /// <param name="pageSize">Number of items to split collection by</param>
        /// <param name="totalCount">Total items in the source collection</param>
        public PagedList(IEnumerable<T> source, int index, int pageSize, int? totalCount)
        {
            Initialize(source.AsQueryable(), index, pageSize, totalCount);
        }

        /// <summary>
        /// Specialized pagination list
        /// </summary>
        /// <param name="source">Collection to paginate</param>
        /// <param name="index">Page to return</param>
        /// <param name="pageSize">Number of items to split collection by</param>
        public PagedList(IQueryable<T> source, int index, int pageSize)
            : this(source, index, pageSize, null)
        {
        }

        /// <summary>
        /// Specialized pagination list
        /// </summary>
        /// <param name="source">Collection to paginate</param>
        /// <param name="index">Page to return</param>
        /// <param name="pageSize">Number of items to split collection by</param>
        /// <param name="totalCount">Total items in the source collection</param>
        public PagedList(IQueryable<T> source, int index, int pageSize, int? totalCount)
        {
            Initialize(source, index, pageSize, totalCount);
        }

        #region IPagedList Members

        /// <summary>
        /// Number of pages in the pagination
        /// </summary>
        public int PageCount { get; private set; }
        /// <summary>
        /// Total items in the collection
        /// </summary>
        public int TotalItemCount { get; private set; }
        /// <summary>
        /// Index of the page to return
        /// </summary>
        public int PageIndex { get; private set; }
        /// <summary>
        /// Page number of the returned page
        /// </summary>
        public int PageNumber { get { return PageIndex + 1; } }
        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; private set; }
        /// <summary>
        /// Collection has previous pages
        /// </summary>
        public bool HasPreviousPage { get; private set; }
        /// <summary>
        /// Collection has more pages
        /// </summary>
        public bool HasNextPage { get; private set; }
        /// <summary>
        /// Current page is first page
        /// </summary>
        public bool IsFirstPage { get; private set; }
        /// <summary>
        /// Current page is last page
        /// </summary>
        public bool IsLastPage { get; private set; }

        #endregion

        /// <summary>
        /// Initialized the specialized collection
        /// </summary>
        /// <param name="source">Collection to paginate</param>
        /// <param name="index">Index of the page to return</param>
        /// <param name="pageSize">Number of items to split the source collection by</param>
        /// <param name="totalCount">Total items in the source collection</param>
        protected void Initialize(IQueryable<T> source, int index, int pageSize, int? totalCount)
        {
            // for using it externally with 1 based page number
            index = index - 1;

            //### argument checking
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("PageIndex cannot be below 0.");
            }
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("PageSize cannot be less than 1.");
            }

            //### set source to blank list if source is null to prevent exceptions
            if (source == null)
            {
                source = new List<T>().AsQueryable();
            }

            //### set properties
            if (!totalCount.HasValue)
            {
                TotalItemCount = source.Count();
            }
            else
                TotalItemCount = totalCount.Value;
#if CUSTOM
      else
      {
        TotalItemCount = totalCount.Value;
      }
#endif
            PageSize = pageSize;
            PageIndex = index;
            if (TotalItemCount > 0)
            {
                PageCount = (int)Math.Ceiling(TotalItemCount / (double)PageSize);
            }
            else
            {
                PageCount = 0;
            }
            HasPreviousPage = (PageIndex > 0);
            HasNextPage = (PageIndex < (PageCount - 1));
            IsFirstPage = (PageIndex <= 0);
            IsLastPage = (PageIndex >= (PageCount - 1));

            //### add items to internal list
            if (TotalItemCount > 0)
            {
                AddRange(source
            #if !CUSTOM
            .Skip((index) * pageSize).Take(pageSize).ToList()
            #endif
            );
            }
        }
    }
}
