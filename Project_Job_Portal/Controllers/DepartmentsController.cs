using Project_Job_Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace Project_Job_Portal.Controllers
{
    [Authorize(Roles = "Admin, Users")]
    public class DepartmentsController : Controller
    {
        private readonly CandidateDbContext db = new CandidateDbContext();
        // GET: Departments
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Department model)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(model);
                db.SaveChanges();
                return PartialView("_Success");
            }
            return PartialView("_Fail");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var data = db.Departments.FirstOrDefault(x => x.DepartmentId == id);
            if (data == null) return new HttpNotFoundResult();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(Department model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return PartialView("_Success");
            }
            return PartialView("_Fail");
        }
        public ActionResult Delete(int id)
        {
            var department = db.Departments.FirstOrDefault(x => x.DepartmentId == id);
            if (department == null) return new HttpNotFoundResult();
            db.Departments.Remove(department);
            db.SaveChanges();
            return Json(new { success = true });
        }
        public PartialViewResult DepartmentsTable(int pg = 1)
        {
            var data = db.Departments.OrderBy(x => x.DepartmentId).ToPagedList(pg, 5);
            return PartialView("_DepartmentTable", data);
        }
    }
}