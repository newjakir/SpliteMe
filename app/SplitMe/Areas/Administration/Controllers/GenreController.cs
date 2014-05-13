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
    public class GenreController : SplitMeController
    {
        
        // GET: /Admin/Genre/
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Index()
        {
            return View();
        }

        public GenreController()
        {
			//CacheLockMask = "Lock-" + _name + "[{0}-{1}]";
		}

		//[RequireSiteFilter]
		//[NavigationHistoryFilter(IsMileStone=true)]
        [Authorize(Roles = "SysAdmin")]
		public ActionResult List() {

            List<Genre> genres = Genre.FetchAll(CurrentUserId, null);

			return View(genres);
		}

        //[OutputCache(Duration = 1, Location = OutputCacheLocation.None, NoStore = true)]
        //[RequireSiteFilter]
        //[NavigationHistoryFilter]
        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Edit(Guid code)
        {
            Genre obj = null;
            obj = Genre.Fetch(code, CurrentUserId, null);
            string title = "Edit Genre";

            if (obj == null)
            {
                //return NotFound();
                obj = Genre.New();
                obj.Code = code;
                obj.IsNew = true;
                title = "Add New Genre";
            }
            else
            {
                obj.IsNew = false;
            }
            ViewBag.PageTitle = title;

            //PushManager pm = new PushManager();
            //pm.SendTestPush();
            return View(obj);
        }

        [ActionName("Edit")]
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "SysAdmin")]
        //[RequireSiteFilter]
        public ActionResult OnEdit(Guid code, string txtGenre)
        {

            Genre obj = null;
            obj = Genre.Fetch(code, CurrentUserId, null);

            if (obj == null)
            {
                obj = Genre.New();
                obj.IsNew = true;
                obj.Code = code;
                // do allergy not found error
                //ModelState.AddModelError("_FORM", "Failed to retrieve diet.");
                //return ShowValidation(model);
            }
            else
            {
                obj.IsNew = false;
            }

            try
            {
                string logoBasePath = WebConfigurationManager.AppSettings["LogoPath"].ToString();
                string savedPath = SaveFile(Request.Files["logoPath"], logoBasePath);
                obj.GenreText = txtGenre.Trim();
                obj.LogoPath = savedPath;
                obj.Save( CurrentUserId, null);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            //return View();
            return Redirect(Url.Action("List"));
        }

        //[RequireSiteFilter]
        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Add()
        {
            return RedirectToAction("Edit", new { code = Guid.NewGuid()});
            //return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Delete(Guid code)
        {
            Genre obj = null;
            //TODO : FIX HACK
            try
            {
                obj = Genre.Fetch(code, CurrentUserId, null);

                if (obj != null)
                    obj.Delete(CurrentUserId, null);

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    deleted = false
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                deleted = true
            }, JsonRequestBehavior.AllowGet);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////API METHODS //////////////////////////////////////////////////////////////////////////////
        public JsonResult GetAll()
        {
            try
            {
                List<Genre> gens = Genre.FetchAll(CurrentUserId, null);
                List<ApiGenre> genres = new List<ApiGenre>();
                foreach (Genre gen in gens)
                {
                    genres.Add(new ApiGenre { Code = gen.Code, Genre = gen.GenreText, LogoPath = gen.LogoPath });
                }
                return Json(new { Success = true, Data = genres }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
