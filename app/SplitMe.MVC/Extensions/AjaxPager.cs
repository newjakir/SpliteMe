﻿using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Resources;

namespace SplitMe.MVC.Extensions
{
    public class AjaxPager
    {
        private AjaxHelper ajaxHelper;
        private ViewContext viewContext;
        private readonly int pageSize;
        private readonly int currentPage;
        private readonly int totalItemCount;
        //private readonly string actionName;
        private readonly RouteValueDictionary linkWithoutPageValuesDictionary;
        private readonly AjaxOptions ajaxOptions;

        public AjaxPager(AjaxHelper helper, ViewContext viewContext, int pageSize, int currentPage, int totalItemCount, AjaxOptions options, RouteValueDictionary valueDictionary)
        {
            this.ajaxHelper = helper;
            this.viewContext = viewContext;
            this.pageSize = pageSize;
            this.currentPage = currentPage;
            this.totalItemCount = totalItemCount;
            this.ajaxOptions = options;
            this.linkWithoutPageValuesDictionary = valueDictionary;
        }

        public string RenderHtml()
        {
            int pageCount = (int)Math.Ceiling(this.totalItemCount / (double)this.pageSize);
            int nrOfPagesToDisplay = 8;

            var sb = new StringBuilder();
            sb.Append("<ul>");

            // Previous
            if (this.currentPage > 1)
            {
                sb.Append("<li>" + GeneratePageLink("<", this.currentPage - 1) + "</li>");
            }
            else
            {
                sb.Append("<li><a>&lt;</a></li>");
            }

            int start = 1;
            int end = pageCount;

            if (pageCount > nrOfPagesToDisplay)
            {
                int middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                int below = (this.currentPage - middle);
                int above = (this.currentPage + middle);

                if (below < 4)
                {
                    above = nrOfPagesToDisplay;
                    below = 1;
                }
                else if (above > (pageCount - 4))
                {
                    above = pageCount;
                    below = (pageCount - nrOfPagesToDisplay);
                }

                start = below;
                end = above;
            }

            if (start > 3)
            {
                sb.Append("<li>" + GeneratePageLink("1", 1) + "</li>");
                sb.Append("<li>" + GeneratePageLink("2", 2) + "</li>");
                sb.Append("<li><a>...</a></li>");
            }
            for (int i = start; i <= end; i++)
            {
                if (i == this.currentPage)
                {
                    sb.AppendFormat("<li class=active><a>{0}</a></li>", i);
                }
                else
                {
                    sb.Append("<li>" + GeneratePageLink(i.ToString(), i) + "</li>");
                }
            }
            if (end < (pageCount - 3))
            {
                sb.Append("<li><a>...</a></li>");
                sb.Append("<li>" + GeneratePageLink((pageCount - 1).ToString(), pageCount - 1) + "</li>");
                sb.Append("<li>" + GeneratePageLink(pageCount.ToString(), pageCount) + "</li>");
            }

            // Next
            if (this.currentPage < pageCount)
            {
                sb.Append("<li>" + GeneratePageLink(">", (this.currentPage + 1)) + "</li>");
            }
            else
            {
                sb.Append("<li><a>&gt;</a></li>");
            }

            sb.Append("</ul>");
            return sb.ToString();
        }

        private string GeneratePageLink(string linkText, int pageNumber)
        {
            var pageLinkValueDictionary = new RouteValueDictionary(this.linkWithoutPageValuesDictionary);
            pageLinkValueDictionary.Add("page", pageNumber);

            return ajaxHelper.ActionLink(linkText, pageLinkValueDictionary["action"].ToString(), pageLinkValueDictionary, ajaxOptions).ToHtmlString();
        }
    }
}