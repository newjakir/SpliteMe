using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SplitMe.Domain;
using SplitMe.Common.Data;
using SplitMe.Domain.Api;
using SplitMe.Controllers;
using System.Web.Configuration;
using SplitMe.Apns;
namespace SplitMe.Areas.Administration.Controllers
{
    public class SettingsController : SplitMeController
    {
        // GET: /Admin/Genre/
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Index()
        {
            return View();
        }

        public SettingsController()
        {
			//CacheLockMask = "Lock-" + _name + "[{0}-{1}]";
		}

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Edit()
        {
            ViewBag.PageTitle = "Edit Settings";
            Settings settings = Settings.Fetch(null);
            settings = settings ?? new Settings();
            return View(settings);
        }

        [ActionName("Edit")]
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "SysAdmin")]
        //[RequireSiteFilter]
        public ActionResult OnEdit(int txtFirstTimeLimit, int txtSecondTimeLimit, int txtThirdTimeLimit)
        {
            Settings settings = new Settings();
            settings.FirstTimeLimit = txtFirstTimeLimit;
            settings.SecondTimeLimit = txtSecondTimeLimit;
            settings.ThirdTimeLimit = txtThirdTimeLimit;

            try
            {
                settings.Save(CurrentUserId, null);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            //return View();
            return Redirect(Url.Action("Index"));
        }

    }
}
