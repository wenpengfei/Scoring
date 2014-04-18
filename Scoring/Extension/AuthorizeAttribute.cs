using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scoring.Extension
{
    public class CustomerAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            if (context.Session != null)
                if (context.Session["User"] == null)
                    context.Response.Redirect("/login/index");
        }
    }
}