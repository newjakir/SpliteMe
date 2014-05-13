using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SplitMe.Domain;
using SplitMe.Common.Data;
using SplitMe.Domain.Api;
using SplitMe.Controllers;

namespace SplitMe.Areas.Administration.Controllers
{
    public class ArtistController : SplitMeController
    {
        // GET: /Admin/Artist/
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Index()
        {
            return View();
        }

        public ArtistController()
        {
			//CacheLockMask = "Lock-" + _name + "[{0}-{1}]";
		}

		//[RequireSiteFilter]
		//[NavigationHistoryFilter(IsMileStone=true)]
        [Authorize(Roles = "SysAdmin")]
		public ActionResult List() {

            List<Artist> Artists = Artist.FetchAll(CurrentUserId, null);

			return View(Artists);
		}

        //[OutputCache(Duration = 1, Location = OutputCacheLocation.None, NoStore = true)]
        //[RequireSiteFilter]
        //[NavigationHistoryFilter]
        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Edit(Guid id)
        {
            Artist obj = null;
            obj = Artist.Fetch(id, CurrentUserId, null);
            string title = "Edit Artist";

            if (obj == null)
            {
                //return NotFound();
                obj = Artist.New();
                obj.Id = id;
                obj.IsNew = true;
                title = "Add New Artist";
            }
            else
            {
                obj.IsNew = false;
            }
            ViewBag.PageTitle = title;
            return View(obj);
        }

        [ActionName("Edit")]
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "SysAdmin")]
        //[RequireSiteFilter]
        public ActionResult OnEdit(Guid id, string txtName)
        {

            Artist obj = null;
            obj = Artist.Fetch(id, CurrentUserId, null);

            if (obj == null)
            {
                obj = Artist.New();
                obj.IsNew = true;
                obj.Id = id;
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
                obj.Name = txtName.Trim();
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
            return RedirectToAction("Edit", new { id = Guid.NewGuid()});
            //return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Delete(Guid id)
        {
            Artist obj = null;
            //TODO : FIX HACK
            try
            {
                obj = Artist.Fetch(id, CurrentUserId, null);

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
        //public JsonResult GetAll()
        //{
        //    try
        //    {
        //        List<Artist> gens = Artist.FetchAll(CurrentUserId, null);
        //        List<ApiArtist> Artists = new List<ApiArtist>();
        //        foreach (Artist gen in gens)
        //        {
        //            Artists.Add(new ApiArtist { Code = gen.Code, Artist = gen.ArtistText });
        //        }
        //        return Json(new { Success = true, Data = Artists }, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {

        //        return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}
