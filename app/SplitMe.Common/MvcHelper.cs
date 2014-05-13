using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace SplitMe.Common
{
    public class MvcHelper
    {
        public static void SetSearchViewData(ViewDataDictionary ViewData, string so, string st, string sc,
        IList<ItemDTO> SearchObject)
        {
            SetSearchViewData(ViewData, so, st, sc, false, SearchObject);
        }

        /// <summary>
        /// Inject search information into the ViewData
        /// </summary>
        /// <param name="ViewData">Current ViewData Dictionary</param>
        /// <param name="so">Search Object</param>
        /// <param name="st">Search Type</param>
        /// <param name="sc">Search Criteria</param>
        /// <param name="SearchObject">List of possible options for Search Object</param>
        public static void SetSearchViewData(ViewDataDictionary ViewData, string so, string st, string sc, bool inactive,
          IList<ItemDTO> SearchObject)
        {
            ViewData["so-val"] = so;
            ViewData["st-val"] = st;
            ViewData["sc-val"] = sc;
            ViewData["so"] = new SelectList(SearchObject, "Value", "Text", so);
            ViewData["st"] = new SelectList(GetSearchTypes(), "Value", "Text", st);
            ViewData["sc"] = sc;
            ViewData["inactive"] = inactive;
        }

        /// <summary>
        /// Inject search information into the ViewData for Unit types
        /// </summary>
        /// <param name="ViewData">Current ViewData Dictionary</param>
        /// <param name="so">Search Object</param>
        /// <param name="st">Search Type</param>
        /// <param name="sc">Search Criteria</param>
        /// <param name="SearchObject">List of possible options for Search Object</param>
        public static void SetUnitSearchViewData(ViewDataDictionary ViewData, string so, string st, string sc,
            IList<ItemDTO> SearchObject)
        {
            ViewData["so-val"] = so;
            ViewData["st-val"] = st;
            ViewData["sc-val"] = sc;
            ViewData["so"] = new SelectList(SearchObject, "Value", "Text", so);
            ViewData["st"] = new SelectList(GetUnitSearchTypes(), "Value", "Text", st);
            ViewData["sc"] = sc;
        }

        /// <summary>
        /// Generates a list of valid search types
        /// </summary>
        /// <returns>List of valid search types</returns>
        private static IList<ItemDTO> GetSearchTypes()
        {
            IList<ItemDTO> items = new List<ItemDTO> {
			new ItemDTO { Text = "Contains", Value = "%{0}%" },
			new ItemDTO { Text = "Matches", Value = "{0}" },
			new ItemDTO { Text = "Starts With", Value = "{0}%" },
			new ItemDTO { Text = "Ends With", Value = "%{0}" }
			};
            return items;
        }

        /// <summary>
        /// Generates a list of valid search types for units
        /// </summary>
        /// <returns>List of valid search types</returns>
        private static IList<ItemDTO> GetUnitSearchTypes()
        {
            IList<ItemDTO> items = new List<ItemDTO> {
			new ItemDTO { Text = "Contains", Value = "%{0}%" },
			new ItemDTO { Text = "Matches", Value = "{0}" },
			new ItemDTO { Text = "Starts With", Value = "{0}%" },
			new ItemDTO { Text = "Ends With", Value = "%{0}" }
			};
            return items;
        }
    }
}
