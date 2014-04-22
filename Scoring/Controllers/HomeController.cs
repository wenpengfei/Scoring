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
        [LoginAuthorize]
        public ActionResult Index()
        {
            scoring_employee scoringEmployee = (scoring_employee)Session["User"];
            string scoresSerializerId = DateTime.Now.ToString("yyyyMM");
            //当前用户 当月已经打过分 
            List<scoring_results> scoringResultses = db.scoring_results.Where(w => w.ScoresSerializerId == scoresSerializerId && w.EmployeeId == scoringEmployee.Id).ToList();
            ScoreMainModel smm = new ScoreMainModel();
            List<int> sonIds = GetSonId(scoringEmployee.DepartmentId ?? -1).ToList();
            sonIds.Add(scoringEmployee.DepartmentId ?? -1);
            if (scoringResultses.Count == 0)
            {
                List<ScoreMain> scoreMains = db.scoring_employee.Where(s => sonIds.Contains(s.DepartmentId ?? -1) && s.Id != scoringEmployee.Id)
                                     .Select(s => new ScoreMain
                                     {
                                         Id = s.Id,
                                         Name = s.Name,
                                         DepartmentId = s.DepartmentId,
                                         Department = db.scoring_department.Where(w => w.Id == s.DepartmentId).FirstOrDefault().Name
                                     }).OrderBy(o => o.Id).ToList();

                //如果是普通员工，查询组长信息
                if (scoringEmployee.Role == 1)
                {
                    List<ScoreMain> mains = db.scoring_employee.Where(
                        w =>
                        w.DepartmentId == db.scoring_department.FirstOrDefault(x => x.Id == scoringEmployee.DepartmentId).ParentId).
                        Select(
                            x =>
                            new ScoreMain
                                {
                                    Department =
                                        db.scoring_department.Where(w => w.Id == x.DepartmentId).FirstOrDefault().Name,
                                    DepartmentId = x.DepartmentId,
                                    Id = x.Id,
                                    Name = x.Name
                                }).ToList();
                    scoreMains = scoreMains.Concat(mains).OrderBy(o => o.Id).ToList();
                }


                smm = new ScoreMainModel { Employees = scoreMains };
            }
            return View(smm);
        }

        [HttpPost]
        [LoginAuthorize]
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


        [LeaderAuthorize]
        public ActionResult Result(string scoresSerializerId)
        {
            if (string.IsNullOrEmpty(scoresSerializerId))
            {
                scoresSerializerId = DateTime.Now.ToString("yyyyMM");
            }
            scoring_employee scoringEmployee = (scoring_employee)Session["User"];
            List<int> sonIds = GetSonId(scoringEmployee.DepartmentId ?? -1).ToList();
            IQueryable<scoring_employee> scoringEmployees = db.scoring_employee.Where(x => sonIds.Contains(x.DepartmentId ?? -1));
            IQueryable<int> ids = scoringEmployees.Select(x => x.Id);
            int id = 0;
            if (Int32.TryParse(scoresSerializerId, out id))
            {
                IQueryable<scoring_results> scoringResultses = db.scoring_results.Where(w => ids.Contains(w.BeRatedEmployeeId ?? -1) && w.ScoresSerializerId == scoresSerializerId);
                IQueryable<ScoreResult> scoreResults = scoringResultses.GroupBy(g => new { g.BeRatedEmployeeId }).Select(s => new ScoreResult
                                                                                                                     {
                                                                                                                         Department = db.scoring_department.Where(b => b.Id == db.scoring_employee.Where(w => w.Id == s.Key.BeRatedEmployeeId).FirstOrDefault().DepartmentId).FirstOrDefault().Name,
                                                                                                                         DepartmentId = db.scoring_employee.Where(w => w.Id == s.Key.BeRatedEmployeeId).FirstOrDefault().Id,
                                                                                                                         Name = db.scoring_employee.Where(w => w.Id == s.Key.BeRatedEmployeeId).FirstOrDefault().Name,
                                                                                                                         ScoreAvg = s.Average(avg => avg.Scores) ?? 0,
                                                                                                                         VoteCount = s.Count(count => count.BeRatedEmployeeId == s.Key.BeRatedEmployeeId)
                                                                                                                     }).OrderBy(o => o.DepartmentId);

                return View(scoreResults);
            }
            return View();
        }



        public IEnumerable<Int32> GetSonId(int pid)
        {
            var query = db.scoring_department.Where(w => w.ParentId == pid).Select(s => s.Id);
            return query.ToList().Concat(query.ToList().SelectMany(GetSonId));
        }
    }
}
