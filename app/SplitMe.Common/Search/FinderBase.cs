using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitMe.Common.Search
{
    public abstract class FinderBase
    {
        protected string ScrubForSql(string input)
        {
            return input.Replace("'", "''");
        }
        /// <summary>
        /// Scrubs string and wraps with ' for sql formatting
        /// </summary>
        /// <param name="input">String to clean and wrap</param>
        /// <returns>Clean and wrapped sql string</returns>
        protected string DBQuote(string input)
        {
            return "'" + ScrubForSql(input) + "'";
        }
    }
}
