using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitMe.Common.Search
{
    public class FinderSet
    {
        /// <summary>
        /// Sql used to return the actual requested page based on the filters applied in the Finder
        /// </summary>
        public string ResultSql { get; set; }

        /// <summary>
        ///  Sql used to return the total count of results based on the filters applied in the Finder
        /// </summary>
        public string CountSql { get; set; }
    }
}
