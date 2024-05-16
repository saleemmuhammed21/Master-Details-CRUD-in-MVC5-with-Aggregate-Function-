using Project_Job_Portal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_Job_Portal.Controllers
{
    public class AggregatesController : Controller
    {
        private readonly CandidateDbContext db = new CandidateDbContext();
        // GET: Aggregates
        public ActionResult Index()
        {
            var data =db.Candidates.Include(x=>x.Qualifications).ToList();
            return View(data);
        }
    }
}