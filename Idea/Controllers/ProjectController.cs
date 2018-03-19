using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Idea.Controllers
{
    public class ProjectController : Controller
    {
        public ActionResult Index()
        {
            EmployeeManager employeeManager = EmployeeManager.GetInstance();
            ViewBag.Divisions = employeeManager.GetDivision();
            ViewBag.Departments = employeeManager.GetDepartment();

            var from = DateTime.Now.AddMonths(-2);
            var to = from.AddMonths(11);
            var colspan1 = 0;
            var colspan2 = 0;
            for (DateTime d = from; d <= to; d = d.AddMonths(1))
            {
                if (d.Year == from.Year)
                    colspan1++;
                if (d.Year == to.Year)
                    colspan2++;
            }
            if (from.Year == to.Year)
            {
                colspan1 = 12;
                colspan2 = 0;
            }
            ViewBag.Colspan1 = colspan1;
            ViewBag.Colspan2 = colspan2;
            return View();
        }

        [HttpPost]
        public JsonResult Upload()
        {

            var file = Request.Files[0];
            var fileName = Path.GetFileName(file.FileName);

            var path = Path.Combine(Server.MapPath("~/Upload/" + DateTime.Now.ToString("yyyyMMdd") + "/"), fileName);
            file.SaveAs(path);
            return Json(1);
        }

        [HttpPost]
        public JsonResult GetDepartment(string DIV)
        {
            EmployeeManager em = EmployeeManager.GetInstance();
            var lst = em.GetDepartment(DIV);
            return Json(lst);
        }

        [HttpPost]
        public JsonResult UpdateTranslate(string ID, string REQUEST, string ISSUE)
        {
            ProjectManager prj = ProjectManager.GetInstance();
            var result = prj.UpdateTranslate(ID, REQUEST, ISSUE);
            return Json(result);
        }

    }
}
