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
    public class GeneralController : SplitMeController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize(Roles = "SysAdmin")]
        public ActionResult SendPush()
        {
            return View();
        }

        [ActionName("SendPush")]
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "SysAdmin")]
        //[RequireSiteFilter]
        public ActionResult OnSendPush(string txtToken)
        {
            PushManager pm = new PushManager();
            pm.SendTestPush(txtToken, "Test Push Notification!");
            return Redirect(Url.Action("SendPush"));
        }
        /// <summary>
        /// Api Call for login
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="fbId"></param>
        /// <returns></returns>
        public JsonResult Login(string deviceToken, string fbId, string fbName)
        {
            try
            {
                Player player = Player.Login(deviceToken,fbId, fbName, null);
                return Json(new { Success = true, Data = player }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Api Call for login
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="fbId"></param>
        /// <returns></returns>
        public JsonResult Challenge(string toFbID, string fromFbId, Guid code)
        {
            string fromUserFbName = string.Empty;
            try
            {
                string deviceToken = Player.Challenge(toFbID, fromFbId, ref code, ref fromUserFbName, null);
                PushManager pm = new PushManager();
                pm.Challenge(deviceToken, code, fromUserFbName);
                return Json(new { Success = true, Data = "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Accept the challenge
        /// </summary>
        /// <param name="chKey"></param>
        /// <returns></returns>
        public JsonResult Accept(Guid chKey)
        {
            string fromUserFbName = string.Empty;
            try
            {
                ApiLyrics apiLyrics = Player.Accept(chKey, null);
                return Json(new { Success = true, Data = apiLyrics }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// //////Submit Answer of Lyrics
        /// </summary>
        /// <param name="chKey"></param>
        /// <returns></returns>
        public JsonResult SubmitAnswer(Guid chKey, int timeTaken, bool isCorrect)
        {
            int totalScore = 0;
            try
            {
                totalScore = Player.SubmitAnswer(chKey, timeTaken, isCorrect, null);
                return Json(new { Success = true, Data = totalScore }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
