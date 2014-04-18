using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Scoring.Models;
namespace Scoring.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        scoringEntities db = new scoringEntities();
        public ActionResult Index()
        {
            return View();
        }

        public void LoginOut()
        {
            Session["User"] = null;
            Session.Abandon();
        }


        [HttpPost]
        public ActionResult Login(scoring_employee entity)
        {
            Validate(entity);
            if(ModelState.IsValid)
            {
                List<scoring_employee> scoringEmployees = db.scoring_employee.Where(x => x.Name == entity.Name && x.Password == entity.Password).ToList();
                if (scoringEmployees.Count > 0)
                {
                    Session["User"] = scoringEmployees.FirstOrDefault();
                    return RedirectToAction("Index", "Home"); 
                }
                else
                {
                    ModelState.AddModelError("Valid", "用户名密码不匹配");
                }
            }
            return View("Index");
        }

        private void Validate(scoring_employee entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
            {
                ModelState.AddModelError("Valid", "用户名不能为空");
            }
            if (string.IsNullOrEmpty(entity.Password))
            {
                ModelState.AddModelError("Valid", "密码不能为空");
            }

        }

    }
}
