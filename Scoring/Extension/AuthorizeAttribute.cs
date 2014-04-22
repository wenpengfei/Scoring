using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Scoring.Models;

namespace Scoring.Extension
{
    public class LoginAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            if (context.Session != null)
                if (context.Session["User"] == null)
                    context.Response.Redirect("/login/index");
        }
    }

    public class LeaderAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            if (context.Session != null)
            {
                if (context.Session["User"] == null)
                {
                    context.Response.Redirect("/login/index");
                }
                else
                {
                    if (((scoring_employee)context.Session["User"]).Role != 2 && ((scoring_employee)context.Session["User"]).Role != 3)
                    {
                        context.Response.Redirect("/");
                    }
                }
            }
        }
    }
}