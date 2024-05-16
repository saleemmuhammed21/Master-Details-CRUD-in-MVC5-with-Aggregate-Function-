using Project_Job_Portal.Models;
using Project_Job_Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using X.PagedList;
using System.Data.Entity;
using System.Threading;

namespace Project_Job_Portal.Controllers
{
    //[Authorize(Roles = "Admin, Users")]
    public class CandidatesController : Controller
    {
        private readonly CandidateDbContext db = new CandidateDbContext();
        // GET: Candidates
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult CandidateTable(int pg = 1)
        {

            var data = db.Candidates
                .Include(x => x.Qualifications)
                .Include(x => x.Department)
                .OrderBy(x => x.CandidateId)
                .ToPagedList(pg, 2);
            Thread.Sleep(1000);
            return PartialView("_CandidateTable", data);
        }
        //[Authorize(Roles = "Admin")]
        public ActionResult Create()
        {

            return View();
        }
        public ActionResult CreateForm()
        {
            CandidateInputModel model = new CandidateInputModel();
            model.Qualifications.Add(new Qualification());
            ViewBag.Departments = db.Departments.ToList();
            return PartialView("_CreateForm", model);
        }

        [HttpPost]
        public ActionResult Create(CandidateInputModel model, string act = "")
        {
            if (act == "add")
            {
                model.Qualifications.Add(new Qualification());
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
                Thread.Sleep(1000);
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.Qualifications.RemoveAt(index);
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
                Thread.Sleep(1000);
            }
            if (act == "insert")
            {
                if (ModelState.IsValid)
                {
                    var candidate = new Candidate
                    {
                        DepartmentId = model.DepartmentId,
                        CandidateName = model.CandidateName,
                        BirthDate = model.BirthDate,
                        AppliedFor = (Models.AppliedFor)model.AppliedFor,
                        ExpectedSalary = model.ExpectedSalary??0,
                        Conditions = model.Conditions,
                    };
                    string ext = Path.GetExtension(model.Picture.FileName);
                    string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                    string savePath = Path.Combine(Server.MapPath("~/Pictures"), f);
                    model.Picture.SaveAs(savePath);
                    candidate.Picture = f;


                    db.Candidates.Add(candidate);
                    db.SaveChanges();
                    foreach (var s in model.Qualifications)
                    {
                        //Candidate.Qualification.Add(s);
                        db.Database.ExecuteSqlCommand($"EXEC InsertQualification '{s.Degree}', {(int)s.PassingYear}, '{s.Institute}', '{s.Result}', {candidate.CandidateId}");
                    }

                    CandidateInputModel newmodel = new CandidateInputModel();
                    newmodel.Qualifications.Add(new Qualification());
                    ViewBag.Departments = db.Departments.ToList();
                    foreach (var e in ModelState.Values)
                    {

                        e.Value = null;
                    }
                    return View("_CreateForm", newmodel);
                }
            }
            ViewBag.Departments = db.Departments.ToList();
            return View("_CreateForm", model);
        }
        //[Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {

            ViewBag.Id = id;
            return View();
        }
        public ActionResult EditForm(int id)
        {
            var data = db.Candidates.FirstOrDefault(x => x.CandidateId == id);
            if (data == null) return new HttpNotFoundResult();
            db.Entry(data).Collection(x => x.Qualifications).Load();
            CandidateEditModel model = new CandidateEditModel
            {
                CandidateId = id,
                DepartmentId = data.DepartmentId,
                CandidateName = data.CandidateName,
                BirthDate = data.BirthDate,
                AppliedFor = (ViewModels.AppliedFor)data.AppliedFor,
                ExpectedSalary = data.ExpectedSalary,
                Conditions = data.Conditions,
                Qualifications = data.Qualifications.ToList()
            };
            ViewBag.Departments = db.Departments.ToList();
            ViewBag.CurrentPic = data.Picture;
            return PartialView("_EditForm", model);
        }

        [HttpPost]
        public ActionResult Edit(CandidateEditModel model, string act = "")
        {
            if (act == "add")
            {
                model.Qualifications.Add(new Qualification());
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.Qualifications.RemoveAt(index);
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act == "update")
            {
                if (ModelState.IsValid)
                {
                    var candidate = db.Candidates.FirstOrDefault(x => x.CandidateId == model.CandidateId);
                    if (candidate == null) { return new HttpNotFoundResult(); }
                    candidate.CandidateName = model.CandidateName;
                    candidate.BirthDate = model.BirthDate;
                    candidate.AppliedFor = (Models.AppliedFor)model.AppliedFor;
                    candidate.ExpectedSalary = model.ExpectedSalary ?? 0;
                    candidate.Conditions = model.Conditions;
                    candidate.DepartmentId = model.DepartmentId;
                    if (model.Picture != null)
                    {
                        string ext = Path.GetExtension(model.Picture.FileName);
                        string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = Path.Combine(Server.MapPath("~/Pictures"), f);
                        model.Picture.SaveAs(savePath);
                        candidate.Picture = f;
                    }

                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand($"EXEC DeleteQualificationByCandidateId {candidate.CandidateId}");
                    foreach (var s in model.Qualifications)
                    {
                        db.Database.ExecuteSqlCommand($"EXEC InsertQualification '{s.Degree}', {(int)s.PassingYear}, '{s.Institute}', '{s.Result}', {candidate.CandidateId}");
                    }


                }
            }
            ViewBag.Departments = db.Departments.ToList();
            ViewBag.CurrentPic = db.Candidates.FirstOrDefault(x => x.CandidateId == model.CandidateId)?.Picture;
            return View("_EditForm", model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var qualification = db.Candidates.FirstOrDefault(x => x.CandidateId == id);
            if (qualification == null) return new HttpNotFoundResult();
            db.Candidates.Remove(qualification);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}