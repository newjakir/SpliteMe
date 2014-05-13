using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SplitMe.Common;
using SplitMe.Domain;
using SplitMe.MVC.Extensions;
using SplitMe.Common.Data;
using SplitMe.Controllers;
using SplitMe.Domain.Api;

namespace SplitMe.Areas.Administration.Controllers
{
    public class LyricsController : SplitMeController
    {
        // GET: /Admin/Lyrics/
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Index()
        {
            return View();
        }

        public LyricsController()
        {
			//CacheLockMask = "Lock-" + _name + "[{0}-{1}]";
		}

		//[RequireSiteFilter]
		//[NavigationHistoryFilter(IsMileStone=true)]
        [Authorize(Roles = "SysAdmin")]
        public ActionResult List(int? page, string sidx, string sord, string st, string so, string sc)
        {
            IList<ItemDTO> items = new List<ItemDTO> {
				new ItemDTO { Text = "Genre", Value = "1" },
                new ItemDTO { Text = "Title", Value = "2" },
                new ItemDTO { Text = "Text", Value = "3" },
                new ItemDTO { Text = "Artist", Value = "4" },
			};

            MvcHelper.SetSearchViewData(ViewData, so, st, sc, items);
            SplitMe.Common.Data.IPagedList<Lyrics> lyricss = Lyrics.Search(page ?? 1, 30, so, st, sc, sidx, sord, null);
            //List<Lyrics> genres = (List<Lyrics>)Lyrics.Search(page??1, 30, sidx, sord, st, so, sc, null);
            return View(lyricss);
		}

        //[OutputCache(Duration = 1, Location = OutputCacheLocation.None, NoStore = true)]
        //[RequireSiteFilter]
        //[NavigationHistoryFilter]
        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Edit(Guid code)
        {
            Lyrics obj = null;
            obj = Lyrics.Fetch(code, null, CurrentUserId);
            string title = "Edit Lyrics";

            if (obj == null)
            {
                //return NotFound();
                obj = Lyrics.New();
                obj.Code = code;
                obj.IsNew = true;
                title = "Add New Lyrics";
                obj.Genre = Genre.New();
                obj.Artist = Artist.New();
                obj.Genre.Code = Guid.NewGuid();
                obj.Artist.Id = Guid.NewGuid();
            }
            else
            {
                obj.IsNew = false;
            }
            List<ItemDTO> genres = new List<ItemDTO>();
            List<Genre> gens = Genre.FetchAll(CurrentUserId, null);
            foreach (Genre genre in gens)
            {
                genres.Add(new ItemDTO { Text = genre.GenreText, Value = genre.Code.ToString() });
            }
            ViewData["Genres"] = new SelectList(genres, "Value", "Text", obj.Genre.GenreText);

            List<ItemDTO> artists = new List<ItemDTO>();
            List<Artist> arts = Artist.FetchAll(CurrentUserId, null);
            foreach (Artist a in arts)
            {
                artists.Add(new ItemDTO { Text = a.Name, Value = a.Id.ToString() });
            }
            ViewData["artists"] = new SelectList(artists, "Value", "Text", obj.Artist.Name);

            ViewBag.PageTitle = title;
            return View(obj);
        }

        [ActionName("Edit")]
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "SysAdmin")]
        //[RequireSiteFilter]
        public ActionResult OnEdit(Guid code, string txtTitle, string txtText, Guid Genres, Guid artists, string txtCorrectAnswer, string txtWrongAnswer1, string txtWrongAnswer2, string txtWrongAnswer3 )
        {

            Lyrics obj = null;
            obj = Lyrics.Fetch(code,null, CurrentUserId);

            //obj.IsNew = false;
            if (obj == null)
            {
                obj = Lyrics.New();
                obj.IsNew = true;
                obj.Code = code;
            }
            else
                obj.IsNew = false;

            try
            {
                obj.Title = txtTitle.Trim();
                obj.LyricsText = txtText.Trim();
                obj.CorrectAnswer = txtCorrectAnswer;
                obj.WrongAnswer1 = txtWrongAnswer1;
                obj.WrongAnswer2 = txtWrongAnswer2;
                obj.WrongAnswer3 = txtWrongAnswer3;

                Genre genre = Genre.New();
                genre.Code = Genres;
                obj.Genre = genre;

                Artist a = Artist.New();
                a.Id = artists;
                obj.Artist = a;

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
            Lyrics obj = null;
            //TODO : FIX HACK
            try
            {
                obj = Lyrics.Fetch(code, null, CurrentUserId);

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
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////API METHODS///////////////////////////////////////////////////////////////
        public JsonResult GetByGenre(Guid code)
        {
            try
            {
                List<Lyrics> ques = Lyrics.FetchByGenre(code, null, CurrentUserId);
                List<ApiShortLyrics> lyricss = new List<ApiShortLyrics>();
                foreach (Lyrics q in ques)
                {
                    lyricss.Add(new ApiShortLyrics { 
                        Code = q.Code, Title = q.Title,
                        Text = q.LyricsText, Artist = q.Artist.Name});
                }
                return Json(new { Success = true, Data = lyricss }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get(Guid code)
        {
            try
            {
                Lyrics ques = Lyrics.Fetch(code, null, CurrentUserId);

                ApiLyrics q = new ApiLyrics
                {
                    Code = ques.Code,
                    Title = ques.Title,
                    Text = ques.LyricsText,
                    CorrectAnswer = ques.CorrectAnswer,
                    WrongAnswer1 = ques.WrongAnswer1,
                    WrongAnswer2 = ques.WrongAnswer2,
                    WrongAnswer3 = ques.WrongAnswer3
                };

                return Json(new { Success = true, Data = q }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
