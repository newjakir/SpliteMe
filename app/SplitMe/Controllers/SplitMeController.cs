using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SplitMe.Controllers
{
    public class SplitMeController : Controller
    {
        private Guid _userId = Membership.GetUser() != null ?(Guid) Membership.GetUser().ProviderUserKey : Guid.Empty;
        protected Guid CurrentUserId 
        {
            get
            {
                if (HttpContext == null || HttpContext.User == null || !HttpContext.User.Identity.IsAuthenticated) return Guid.Empty;
                return _userId;
            }
        }

        protected static String PrefixFName(String fname)
        {
            if (String.IsNullOrEmpty(fname))
            {
                return null;
            }
            else
            {
                return String.Format("{0}{1}",
                                     DateTime.Now.ToString("yyyyMMddhhmmss"),
                                     fname);
            }
        }

        protected String SaveFile(HttpPostedFileBase file, String virtualPath)
        {
            //Check whether Image directory exists
            string physicalPath = Server.MapPath(virtualPath);
            if (!System.IO.Directory.Exists(physicalPath))
            {
                System.IO.Directory.CreateDirectory(physicalPath);
            }

            if (file != null && file.ContentLength > 0)
            {
                if (virtualPath == null)
                {
                    throw new ArgumentNullException("path cannot be null");
                }
                string pFileName = PrefixFName(file.FileName);
                String relpath = String.Format("{0}/{1}", virtualPath, pFileName);
                try
                {
                    file.SaveAs(Server.MapPath(relpath));
                    return pFileName;
                }
                catch (HttpException e)
                {
                    throw new ApplicationException("Cannot save uploaded file", e);
                }
            }
            return null;
        }
    }
}