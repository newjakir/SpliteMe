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
    public class QuestionController : SplitMeController
    {
        // GET: /Admin/Question/
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Index()
        {
            return View();
        }

        public QuestionController()
        {
			//CacheLockMask = "Lock-" + _name + "[{0}-{1}]";
		}

		//[RequireSiteFilter]
		//[NavigationHistoryFilter(IsMileStone=true)]
        [Authorize(Roles = "SysAdmin")]
        public ActionResult List(int? page, string sidx, string sord, string st, string so, string sc)
        {
            IList<ItemDTO> items = new List<ItemDTO> {
				new ItemDTO { Text = "Question", Value = "2" },
				new ItemDTO { Text = "Genre", Value = "1" },
			};

            MvcHelper.SetSearchViewData(ViewData, so, st, sc, items);
            SplitMe.Common.Data.IPagedList<Question> questions = Question.Search(page ?? 1, 30, so, st, sc, sidx, sord, null);
            //List<Question> genres = (List<Question>)Question.Search(page??1, 30, sidx, sord, st, so, sc, null);
            return View(questions);
		}

        //[OutputCache(Duration = 1, Location = OutputCacheLocation.None, NoStore = true)]
        //[RequireSiteFilter]
        //[NavigationHistoryFilter]
        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Edit(Guid code)
        {
            Question obj = null;
            obj = Question.Fetch(code, null, CurrentUserId);
            string title = "Edit Lyrix";

            if (obj == null)
            {
                //return NotFound();
                obj = Question.New();
                obj.Code = code;
                obj.IsNew = true;
                title = "Add New Lyrix";
                obj.Genre = Genre.New();
                obj.Genre.Code = Guid.NewGuid();
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
            ViewBag.PageTitle = title;
            return View(obj);
        }

        [ActionName("Edit")]
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "SysAdmin")]
        //[RequireSiteFilter]
        public ActionResult OnEdit(Guid code, string txtQuestion, Guid Genres, string txtCorrectAnswer, string txtWrongAnswer1, string txtWrongAnswer2, string txtWrongAnswer3 )
        {

            Question obj = null;
            obj = Question.Fetch(code,null, CurrentUserId);

            //obj.IsNew = false;
            if (obj == null)
            {
                obj = Question.New();
                obj.IsNew = true;
                obj.Code = code;
            }
            else
                obj.IsNew = false;

            try
            {
                obj.QuestionText = txtQuestion.Trim();
                obj.CorrectAnswer = txtCorrectAnswer;
                obj.WrongAnswer1 = txtWrongAnswer1;
                obj.WrongAnswer2 = txtWrongAnswer2;
                obj.WrongAnswer3 = txtWrongAnswer3;

                Genre genre = Genre.New();
                genre.Code = Genres;
                obj.Genre = genre;
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
            Question obj = null;
            //TODO : FIX HACK
            try
            {
                obj = Question.Fetch(code, null, CurrentUserId);

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
                List<Question> ques = Question.FetchByGenre(code, null, CurrentUserId);
                List<ApiShortQuestion> questions = new List<ApiShortQuestion>();
                foreach (Question q in ques)
                {
                    questions.Add(new ApiShortQuestion { Code = q.Code, Question = q.QuestionText });
                }
                return Json(new { Success = true, Data = questions }, JsonRequestBehavior.AllowGet);

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
                Question ques = Question.Fetch(code, null, CurrentUserId);

                ApiQuestion q = new ApiQuestion
                {
                    Code = ques.Code,
                    Question = ques.QuestionText,
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
