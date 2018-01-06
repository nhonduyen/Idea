using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Idea.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ProjectManager pm = ProjectManager.GetInstance();
            EmployeeManager employeeManager = EmployeeManager.GetInstance();
            ViewBag.Divisions = employeeManager.GetDivision();
            ViewBag.Departments = employeeManager.GetDepartment();
            ViewBag.Units = pm.GetKPIUnit();
            return View();
        }

        //
        // GET: /Home/Details/5

        public ActionResult ChangePassword(int success = 0)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string EMP_ID, string Password, string NewPassword, string PwConfirm)
        {
            if (!NewPassword.Equals(PwConfirm))
            {
                return RedirectToAction("ChangePassword", new { success = -1, message = "Password does not match" });
            }
            EmployeeManager em = EmployeeManager.GetInstance();
            var result = em.ChangePassword(EMP_ID, Password, NewPassword);
            var message = result > 0 ? "Success" : "Fail";
            return RedirectToAction("ChangePassword", new { success = result, message = message });
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        //
        // GET: /Home/Details/5
        [HttpPost]
        public JsonResult RegIdea(IDEA idea, string division, string department, int action)
        {
            EmployeeManager em = EmployeeManager.GetInstance();
            IdeaManager ideaManager = IdeaManager.GetInstance();
            int affectedRows = 0;
            if (action == 0)
            {
                affectedRows = ideaManager.Insert(idea.EMP_ID, idea.IDEA_TITLE, idea.DETAIL, idea.QUANTITATIVE, idea.QUALITATIVE);
            }
            else
            {
                affectedRows = ideaManager.Update(idea.ID, idea.EMP_ID, idea.IDEA_TITLE, idea.DETAIL, idea.QUANTITATIVE, idea.QUALITATIVE);
            }
            if (!string.IsNullOrWhiteSpace(division))
                em.UpdateDivision(idea.EMP_ID, division.Trim());
            if (!string.IsNullOrWhiteSpace(department))
                em.UpdateDepartment(idea.EMP_ID, department.Trim());

            return Json(affectedRows);
        }

        [HttpPost]
        public JsonResult RegPrj(PROJECT Project, List<PROJECT_PLAN> Plans, List<KPI> KPIs)
        {
            int result = 0;
            PlanManager plan = PlanManager.GetInstance();
            ProjectManager prj = ProjectManager.GetInstance();
            KpiManager km = KpiManager.GetInstance();

            string existID = prj.IsExist(Project.IDEA_ID);
            if (string.IsNullOrEmpty(existID))
            {
                result = prj.Insert(Project.IDEA_ID, Project.EMP_ID, Project.IDEA_TITLE, Project.PRJECT_GRADE, Project.KPI_NAME,
                    Project.KPI_UNIT, Project.BACKGROUND, Project.NAME);
                if (result > 0)
                {
                    foreach (var p in Plans)
                    {
                        plan.InsertPlan(p.IDEA_ID, p.PLAN_CONTENTS, p.PLAN_DATE);
                    }
                    foreach (var kpi in KPIs)
                    {
                        km.InsertKPI(kpi.IDEA_ID, kpi.PRJ_MONTH, kpi.TARGET_VALUE);
                    }
                }
            }
            else
            {
            }
            return Json(result);

        }

        [HttpPost]
        public JsonResult GetMainProject(DataTableParameters dataTableParameters)
        {
            ProjectManager prj = ProjectManager.GetInstance();

            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = prj.GetMainProject(dataTableParameters.Start + 1, dataTableParameters.Start + dataTableParameters.Length + 1);
            resultSet.recordsTotal = resultSet.recordsFiltered = prj.GetCount(1);
            var seq = 0;
            foreach (var i in lst)
            {
                seq++;
                var columns = new List<string>();
                columns.Add(i.DIVISION.Trim());
                columns.Add(i.DEPARTMENT.Trim());
                columns.Add("<a class='title' href='#' data-id='" + i.IDEA_ID + "'>" + i.IDEA_TITLE.Trim() + "</a>");
                columns.Add(i.KPI_NAME.Trim());
                columns.Add(i.KPI_UNIT.Trim());
                columns.Add(i.FINAL.ToString());
                columns.Add("<a class='res' href='#' data-id='" + i.IDEA_ID + "'>View</a>");
                resultSet.data.Add(columns);
            }
            return Json(resultSet);

        }

        [HttpPost]
        public JsonResult GetNewProject(DataTableParameters dataTableParameters)
        {
            ProjectManager prj = ProjectManager.GetInstance();

            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = prj.SelectPaging(dataTableParameters.Start + 1, dataTableParameters.Start + dataTableParameters.Length + 1);
            resultSet.recordsTotal = resultSet.recordsFiltered = prj.GetCount();

            foreach (var i in lst)
            {
                var columns = new List<string>();
                columns.Add("<a class='title' href='#' data-id='" + i.IDEA_ID + "'>" + i.IDEA_TITLE.Trim() + "</a>");
                columns.Add(i.NAME);
                if (i.INS_DT == null) columns.Add(""); else columns.Add(i.INS_DT.ToShortDateString());
                resultSet.data.Add(columns);
            }
            return Json(resultSet);

        }
        [HttpPost]
        public JsonResult GetIdea(DataTableParameters dataTableParameters)
        {
            IdeaManager idea = IdeaManager.GetInstance();

            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = idea.SelectPaging(dataTableParameters.Start + 1, dataTableParameters.Start + dataTableParameters.Length + 1);
            resultSet.recordsTotal = resultSet.recordsFiltered = idea.GetCount();

            foreach (var i in lst)
            {
                var columns = new List<string>();
                columns.Add("<a class='title' href='#' id='" + i.ID + "'>" + i.IDEA_TITLE.Trim() + "</a>");
                columns.Add(i.EMP_NAME.Trim());
                if (i.DATE == null) columns.Add(""); else columns.Add(i.DATE.ToShortDateString());
                columns.Add("<span class='badge badge-pill badge-primary'>" + i.REP.ToString() + "</span>");

                resultSet.data.Add(columns);
            }
            return Json(resultSet);

        }

        //
        // POST: /Home/Create

        [HttpPost]
        public JsonResult GetIdeaReply(string ID)
        {
            IdeaReplyManager reply = IdeaReplyManager.GetInstance();
            var reps = reply.GetReplyByIdeaId(ID);
            List<string> html = new List<string>();
            var i = 1;
            foreach (var rep in reps)
            {
                var tr = "<tr><td>" + i.ToString() + "</td><td>" + rep.DEPARTMENT.Trim() + "</td><td>" + rep.EMP_NAME.Trim() +
                    "</td><td>" + rep.COMMENTS.Trim() + "</td><td>" + rep.INS_DATE.ToString() + "</td></tr>";
                html.Add(tr);
                i++;
            }
            return Json(html);
        }

        [HttpPost]
        public JsonResult GetProgressReply(string ID)
        {
            ProjectReplyManager reply = ProjectReplyManager.GetInstance();
            var reps = reply.GetReply(ID);
            List<string> html = new List<string>();
            var i = 1;
            foreach (var rep in reps)
            {
                var tr = "<tr><td>" + i.ToString() + "</td><td>" + rep.DEPARTMENT.Trim() + "</td><td>" + rep.EMP_NAME.Trim() +
                    "</td><td>" + rep.COMMENTS.Trim() + "</td><td>" + rep.INS_DATE.ToString() + "</td></tr>";
                html.Add(tr);
                i++;
            }
            return Json(html);
        }

        [HttpPost]
        public JsonResult InsertIdeaComment(IDEA_REPLY reply)
        {
            IdeaReplyManager repMng = IdeaReplyManager.GetInstance();
            int result = repMng.Insert(reply.IDEA_ID, reply.REP_EMP_ID, reply.COMMENTS, DateTime.Now);
            return Json(result);
        }

        [HttpPost]
        public JsonResult InsertPrjComment(PRJ_REPLY reply)
        {
            ProjectReplyManager repMng = ProjectReplyManager.GetInstance();
            int result = repMng.InsertReply(reply);
            return Json(result);
        }
        //
        // POST: /Home/Edit/5

        [HttpPost]
        public JsonResult GetIdeaById(string ID)
        {
            IdeaManager ideaMng = IdeaManager.GetInstance();
            var idea = ideaMng.GetIdea(ID);
            return Json(idea);
        }

        [HttpPost]
        public JsonResult GetProjectById(string ID)
        {
          
            ProjectManager pm = ProjectManager.GetInstance();
            var project = pm.GetProject(ID);
            return Json(project);
        }

        [HttpPost]
        public JsonResult GetKPI(string ID)
        {
            KpiManager km = KpiManager.GetInstance();
            List<KPI> kpis = km.Select(ID);
            return Json(kpis);
        }

        [HttpPost]
        public JsonResult GetPlan(string ID)
        {
            PlanManager pm = PlanManager.GetInstance();
            List<PROJECT_PLAN> plans = pm.Select(ID);
            return Json(plans);
        }

        [HttpPost]
        public JsonResult UpdateProject(PROJECT Project, List<PROJECT_PLAN> Plans, List<KPI> KPIs)
        {
            int result = 0;
            PlanManager plan = PlanManager.GetInstance();
            ProjectManager prj = ProjectManager.GetInstance();
            KpiManager km = KpiManager.GetInstance();

            string existID = prj.IsExist(Project.IDEA_ID);
            if (!string.IsNullOrEmpty(existID))
            {
                result = prj.UpdatePrj(Project.IDEA_ID, Project.EMP_ID, Project.IDEA_TITLE, Project.KPI_NAME, Project.KPI_UNIT, 
                    Project.REMARK, Project.NAME);
               
                if (result > 0)
                {
                    if (Plans != null)
                    {
                        foreach (var p in Plans)
                        {
                            plan.Update(p.IDEA_ID, p.PLAN_CONTENTS, p.PLAN_DATE, p.COMPLETE_YN, p.COMPLETE_DATE, p.ID);
                        }
                    }
                    if (KPIs != null)
                    {
                        foreach (var kpi in KPIs)
                        {
                            km.Update(kpi.IDEA_ID, kpi.PRJ_MONTH, kpi.TARGET_VALUE, kpi.RESULT_VALUE, kpi.ID);
                        }
                    }
                }
            }
         
            return Json(result);
        }

        [HttpPost]
        public JsonResult Login(EMPLOYEE emp)
        {
            EmployeeManager manager = EmployeeManager.GetInstance();
            return Json(manager.Login(emp.EMP_ID, emp.EMP_PW));
        }
    }
}
