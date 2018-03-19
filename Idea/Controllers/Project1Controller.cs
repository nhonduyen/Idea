using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Idea.Controllers
{
    public class Project1Controller : Controller
    {
        public ActionResult Index(string div = "", string dept = "", string grade = "", int page=1)
        {
            ProjectManager prj = ProjectManager.GetInstance();
            KpiManager km = KpiManager.GetInstance();
            EmployeeManager employeeManager = EmployeeManager.GetInstance();
            ViewBag.Divisions = employeeManager.GetDivision();
            ViewBag.Departments = employeeManager.GetDepartment();

            var from1 = DateTime.Now.AddMonths(-2);
            var to1 = from1.AddMonths(11);
            var colspan1 = 0;
            var colspan2 = 0;
            for (DateTime d = from1; d <= to1; d = d.AddMonths(1))
            {
                if (d.Year == from1.Year)
                    colspan1++;
                if (d.Year == to1.Year)
                    colspan2++;
            }
            if (from1.Year == to1.Year)
            {
                colspan1 = 12;
                colspan2 = 0;
            }
            var projects = prj.GetProject( div, dept, grade,page);
            var total = prj.GetSearchCount(div, dept, grade);
            Dictionary<string, object> kpis = new Dictionary<string, object>();
            var date1 = new DateTime(DateTime.Now.Year, 1, 1);
            var date2 = new DateTime(DateTime.Now.Year, 12, 1);
            foreach (var item in projects)
            {
                var kpi = km.GetResultKpi(item.IDEA_ID);
                List<KPI> lst = new List<KPI>();
                for (var d = date1; d <= date2; d = d.AddMonths(1))
                {
                    var count = 0;
                    foreach (var result in kpi)
                    {
                        var k1 = new KPI();
                        if (d.ToString("yyyy-MM").Equals(result.PRJ_MONTH, StringComparison.CurrentCultureIgnoreCase))
                        {
                            count++;
                            k1.PRJ_MONTH = result.PRJ_MONTH;
                            k1.RESULT_VALUE = result.RESULT_VALUE;
                            k1.TARGET_VALUE = result.TARGET_VALUE;
                            lst.Add(k1);

                        }
                    }
                    if (count == 0)
                    {
                        var k1 = new KPI();
                        k1.PRJ_MONTH = "";
                        k1.RESULT_VALUE = 0;
                        k1.TARGET_VALUE = 0;
                        lst.Add(k1);
                    }

                }
                kpis.Add(item.IDEA_ID, lst);

            }
            ViewBag.Colspan1 = colspan1;
            ViewBag.Colspan2 = colspan2;
            ViewBag.PRJ = projects;
            ViewBag.KPI = kpis;
            ViewBag.Total = total;
            return View();
        }

        [HttpPost]
        public JsonResult Upload()
        {
            ProjectManager pm = ProjectManager.GetInstance();
            var file = Request.Files[0];
            var path = Server.MapPath("~/Upload/" + DateTime.Now.ToString("yyyyMMdd") + "/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fileName = Path.GetFileName(file.FileName);

            var pathSave = Path.Combine(path, fileName);
            file.SaveAs(pathSave);
            return Json("/Upload/" + DateTime.Now.ToString("yyyyMMdd") + "/" + file.FileName);
        }


        public FileResult Download(string file)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(file));
            var response = new FileContentResult(fileBytes, "application/octet-stream");
            response.FileDownloadName = Path.GetFileName(Server.MapPath(file));
            return response;
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
