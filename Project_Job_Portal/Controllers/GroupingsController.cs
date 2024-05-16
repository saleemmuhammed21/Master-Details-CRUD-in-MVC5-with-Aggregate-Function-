using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Project_Job_Portal.Models;
using Project_Job_Portal.ViewModels;

namespace Project_Job_Portal.Controllers
{
    public class GroupingsController : Controller
    {
        private readonly CandidateDbContext db = new CandidateDbContext();
        // GET: Groupings
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GroupingByCandidateName()
        {
            var data = db.Qualifications.Include(x => x.Candidate).ToList()
               .GroupBy(s => s.Candidate.CandidateName)
               .Select(g => new GroupedData { Key = g.Key,Data  = g.Select(x => x) })
               .ToList();
            return View(data);
        }
        public ActionResult GroupingByApplyFor()
        {
            var data = db.Candidates
                .ToList()
               .GroupBy(s => s.AppliedFor)
               .Select(g => new GroupedData { Key = g.Key.ToString(), candidates = g.Select(x => x) })
               .ToList();
            return View(data);
        }
    }
}