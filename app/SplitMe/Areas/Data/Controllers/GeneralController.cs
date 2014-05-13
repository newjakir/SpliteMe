using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SplitMe.Domain;
using SplitMe.Common.Data;
using SplitMe.Controllers;
using System.Web.Configuration;
using SplitMe.Apns;

namespace SplitMe.Areas.Data.Controllers
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
            pm.SendTestPush(txtToken, "Test SplitMe Push Notification!");
            return Redirect(Url.Action("SendPush"));
        }

        /// <summary>
        /// //////Register new user
        /// </summary>
        /// <param name="un">Username</param>
        /// <param name="pass">Password</param>
        /// <param name="conPass">Confirm Password</param>
        /// <param name="fn">First Name</param>
        /// <param name="sn">Surname</param>
        /// <param name="gender">Gender</param>
        /// <returns></returns>
        public JsonResult Register(string deviceToken, string email, string password, string firstName, string surname, string phoneNo, string ageRange, string gender)
        {
            User user = Domain.User.New();
            user.Email = email;
            user.Password = password;
            user.FirstName = firstName;
            user.Surname = surname;
            user.AgeRange = ageRange;
            user.Gender = gender;
            user.PhoneNo = phoneNo;
            user.DeviceToken = deviceToken;

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    user.Register(null);
                    return Json(new { Success = true, Data = "Registration Complete!" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false, Message = "Error occured during registration!" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Api Call for login
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="fbId"></param>
        /// <returns></returns>
        public JsonResult Login(string email, string password, string deviceToken)
        {
            try
            {
                User user = Domain.User.Login(email, password, deviceToken);
                return Json(new { Success = true, Data = user }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Api Call for Reset Password
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="fbId"></param>
        /// <returns></returns>
        public JsonResult ResetPassword(string email)
        {
            string fromUserFbName = string.Empty;
            try
            {
                Domain.User.ResetPassword(email);
                return Json(new { Success = true, Data = "New password has been sent to your email." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Api Call for split the amount among friends
        /// </summary>
        /// <param name="fromPhone"></param>
        /// <param name="toPhones"></param>
        /// <returns></returns>
        public JsonResult Split(string fromPhone, string toPhones)
        {
            try
            {
                Dictionary<string, double> cisWithDeviceTokens = Common.Accounting.DoTransaction(fromPhone, toPhones);
                PushManager pm = new PushManager();

                foreach (string devTkn in cisWithDeviceTokens.Keys)
                {
                    string msg = string.Format("Your account is credited with ${0} from {1}.", cisWithDeviceTokens[devTkn], fromPhone);
                    pm.SendPush(devTkn, msg);
                }
                return Json(new { Success = true, Data = "Transaction Completed." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
