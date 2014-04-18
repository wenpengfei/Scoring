using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Scoring.Extension;
using Scoring.Models;

namespace Scoring.Controllers
{
    public class HomeController : Controller
    {
        scoringEntities db = new scoringEntities();
        //
        // GET: /Home/
        [CustomerAuthorize]
        public ActionResult Index()
        {
            scoring_employee scoringEmployee = (scoring_employee)Session["User"];
            string str = DateTime.Now.ToString("yyyyMM");

            //当前用户 当月已经打过分
            List<scoring_results> scoringResultses = db.scoring_results.Where(w => w.ScoresSerializerId == str && w.EmployeeId == scoringEmployee.Id).ToList();
            ScoreMainModel smm = new ScoreMainModel();
            if (scoringResultses.Count == 0)
            {
                List<ScoreMain> scoreMains = db.scoring_employee.Where(s => s.DepartmentId == scoringEmployee.DepartmentId && s.Id != scoringEmployee.Id)
                                     .Select(s => new ScoreMain
                                     {
                                         Id = s.Id,
                                         Name = s.Name,
                                         DepartmentId = s.DepartmentId,
                                         Department = db.scoring_department.Where(w => w.Id == s.DepartmentId).FirstOrDefault().Name
                                     }).OrderBy(o => o.Id).ToList();
                smm = new ScoreMainModel { Employees = scoreMains };
            }
            return View(smm);
        }

        [HttpPost]
        [CustomerAuthorize]
        public ActionResult Submit()
        {
            string str = Request.Form["postdata"];
            if (str.IndexOf(';') != -1)
            {
                string[] strings = str.Split(';');
                for (int i = 0; i < strings.Length; i++)
                {
                    string updates = strings[i];//BeRatedEmployeeId,Scores,Suggest
                    if (updates.IndexOf(',') != -1)
                    {
                        string[] split = updates.Split(',');
                        scoring_results result = new scoring_results
                                                     {
                                                         BeRatedEmployeeId = int.Parse(split[0]),
                                                         EmployeeId = ((scoring_employee)Session["User"]).Id,
                                                         Scores = int.Parse(split[1]),
                                                         Suggest = split[2],
                                                         ScoresSerializerId = DateTime.Now.ToString("yyyyMM"),
                                                         ScoreTime = DateTime.Now
                                                     };
                        db.scoring_results.AddObject(result);
                    }
                }
                if (db.SaveChanges() > 0)
                {
                    return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { IsSuccess = false }, JsonRequestBehavior.AllowGet);
        }
    }
}
