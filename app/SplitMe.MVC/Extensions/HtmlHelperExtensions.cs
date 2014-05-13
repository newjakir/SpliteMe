using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

using SplitMe.Domain;
using SplitMe.Common;

namespace SplitMe.MVC.Extensions
{
  public static class HtmlHelperExtensions
  {
    public static string SortableColumn(this HtmlHelper htmlHelper, string linkText, string columnName, object routeValues)
    {
      //automatically determine the current action
      System.Web.Routing.RouteData data = htmlHelper.ViewContext.Controller.ControllerContext.RouteData;
      string actionName = data.GetRequiredString("action");

      StringBuilder sb = new StringBuilder();
      var vals = new RouteValueDictionary(routeValues);

      string sidx = String.Empty;
      if (System.Web.HttpContext.Current.Request["sidx"] != null)
      {
        sidx = System.Web.HttpContext.Current.Request["sidx"].ToString();
      }

      //modify the sidx
      if (vals.ContainsKey("sidx") == false)
      {
        vals.Add("sidx", columnName);
      }
      else
      {
        vals["sidx"] = columnName;
      }

      //get the sort order from the request variable if it exists
      string sord = String.Empty;
      if (System.Web.HttpContext.Current.Request["sord"] != null)
      {
        sord = System.Web.HttpContext.Current.Request["sord"].ToString();
      }

      //add the sord key if needed
      if (vals.ContainsKey("sord") == false)
      {
        vals.Add("sord", String.Empty);
      }

      //if column matches
      if (sidx.Equals(columnName, StringComparison.CurrentCultureIgnoreCase) == true)
      {
        if (sord.Equals("asc", StringComparison.CurrentCultureIgnoreCase) == true)
        {
          //draw the ascending sort indicator using the wingdings font. 
          //sb.Append(" <font face='Wingdings 3'>&#112;</font>");
          sb.Append(" <img src='/Content/Images/tables/asc.gif' alt='asc'/>");
          vals["sord"] = "desc";
        }
        else
        {
          // sb.Append(" <font face='Wingdings 3'>&#113;</font>");
          sb.Append(" <img src='/Content/Images/tables/desc.gif' alt='desc'/>");
          vals["sord"] = "asc";
        }
      }
      else
      {
        //if the column does not match then force the next sort to ascending order
        vals["sord"] = "asc";
      }

      //Use the ActionLink to build the link and insert it into the string
      //sb.Insert(0, System.Web.Mvc.Ajax.AjaxExtensions.ActionLink(htmlHelper, linkText, actionName, vals));

      sb.Insert(0, System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, vals));
      return sb.ToString();
    }



    public static string SortableColumn(this AjaxHelper htmlHelper, string linkText, string columnName, object routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes = null)
    {

      //automatically determine the current action
      System.Web.Routing.RouteData data = htmlHelper.ViewContext.Controller.ControllerContext.RouteData;
      string actionName = data.GetRequiredString("action");

      StringBuilder sb = new StringBuilder();
      var vals = new RouteValueDictionary(routeValues);

      string sidx = String.Empty;
      if (System.Web.HttpContext.Current.Request["sidx"] != null)
      {
        sidx = System.Web.HttpContext.Current.Request["sidx"].ToString();
      }

      //modify the sidx
      if (vals.ContainsKey("sidx") == false)
      {
        vals.Add("sidx", columnName);
      }
      else
      {
        vals["sidx"] = columnName;
      }

      //get the sort order from the request variable if it exists
      string sord = String.Empty;
      if (System.Web.HttpContext.Current.Request["sord"] != null)
      {
        sord = System.Web.HttpContext.Current.Request["sord"].ToString();
      }

      //add the sord key if needed
      if (vals.ContainsKey("sord") == false)
      {
        vals.Add("sord", String.Empty);
      }

      //if column matches
      if (sidx.Equals(columnName, StringComparison.CurrentCultureIgnoreCase) == true)
      {
        if (sord.Equals("asc", StringComparison.CurrentCultureIgnoreCase) == true)
        {
          //draw the ascending sort indicator using the wingdings font. 
          //sb.Append(" <font face='Wingdings 3'>&#112;</font>");
          //sb.Append(" <img src='/Content/Images/tables/asc.gif' alt='asc'/>");
          sb.Append(" <img src='/Content/Images/tables/desc.gif' alt='desc'/>");
          vals["sord"] = "desc";
        }
        else
        {
          // sb.Append(" <font face='Wingdings 3'>&#113;</font>");
         
          sb.Append(" <img src='/Content/Images/tables/asc.gif' alt='asc'/>");
          vals["sord"] = "asc";
        }
      }
      else
      {
        //if the column does not match then force the next sort to ascending order
        vals["sord"] = "asc";
      }
      //Use the ActionLink to build the link and insert it into the string
      if (htmlAttributes == null)
        sb.Insert(0, System.Web.Mvc.Ajax.AjaxExtensions.ActionLink(htmlHelper, linkText, actionName, vals, ajaxOptions));
      else
        sb.Insert(0, System.Web.Mvc.Ajax.AjaxExtensions.ActionLink(htmlHelper, linkText, actionName, vals, ajaxOptions, htmlAttributes));
      //sb.Insert(0, System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, vals));
      return sb.ToString();
    }
  }
}
