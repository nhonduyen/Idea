﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Data;
using OfficeOpenXml.Style;

namespace Idea.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Users()
        {
            EmployeeManager employeeManager = EmployeeManager.GetInstance();
            ViewBag.Divisions = employeeManager.GetDivision();
            ViewBag.Departments = employeeManager.GetDepartment();
            return View();
        }
        public JsonResult ResetPassword(string EMP_ID)
        {
            EmployeeManager employeeManager = EmployeeManager.GetInstance();
            int result = employeeManager.ResetPassword(EMP_ID);
            return Json(result);
        }
        public JsonResult InsertUser(EMPLOYEE EMP, string Action)
        {
            EmployeeManager employeeManager = EmployeeManager.GetInstance();
            var result = 0;
            if (Action == "0")
            {
                var em = employeeManager.Select(EMP.EMP_ID);
                if (em.Count > 0)
                    return Json(-1);
                result = employeeManager.Insert(EMP.EMP_ID, "123", EMP.EMP_NAME, EMP.DIVISION, EMP.DEPARTMENT, EMP.EMAIL, EMP.ROLE);
            }
            else
            {
                result = employeeManager.Update(EMP.EMP_ID, EMP.EMP_NAME, EMP.DIVISION, EMP.DEPARTMENT, EMP.EMAIL, EMP.ROLE);
            }
            return Json(result);
        }
        public JsonResult GetUserById(string EMP_ID)
        {
            EmployeeManager employeeManager = EmployeeManager.GetInstance();
            EMPLOYEE em = employeeManager.Select(EMP_ID).FirstOrDefault();
            return Json(em);
        }
        public JsonResult GetUsers(DataTableParameters dataTableParameters)
        {
            EmployeeManager employeeManager = EmployeeManager.GetInstance();

            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            List<EMPLOYEE> lst = new List<EMPLOYEE>();
            if (!string.IsNullOrWhiteSpace(dataTableParameters.Search.Value))
            {
                lst = employeeManager.Search(dataTableParameters.Search.Value);
                resultSet.recordsTotal = resultSet.recordsFiltered = employeeManager.GetSearchCount(dataTableParameters.Search.Value);
            }
            else
            {
                lst = employeeManager.SelectPaging(dataTableParameters.Start + 1, dataTableParameters.Start + dataTableParameters.Length + 1);
                resultSet.recordsTotal = resultSet.recordsFiltered = employeeManager.GetCount();
            }
            foreach (var i in lst)
            {
                var role = "User";
                if (i.ROLE == 1) role = "Leader";
                if (i.ROLE == 2) role = "Admin";
                var columns = new List<string>();
                columns.Add("<a class='empId' href='#' data-emp='" + i.EMP_ID.Trim() + "'>" + i.EMP_ID.Trim() + "</a>");
                columns.Add((i.EMP_NAME == null) ? "" : i.EMP_NAME.Trim());
                columns.Add((i.DIVISION == null) ? "" : i.DIVISION.Trim());
                columns.Add((i.DEPARTMENT == null) ? "" : i.DEPARTMENT.Trim());
                columns.Add((i.EMAIL == null) ? "" : i.EMAIL.Trim());
                columns.Add(role);
                resultSet.data.Add(columns);
            }
            return Json(resultSet);
        }
        public ActionResult Summary()
        {
            return View();
        }
        public ActionResult Index()
        {
            ProjectManager pm = ProjectManager.GetInstance();
            EmployeeManager employeeManager = EmployeeManager.GetInstance();
            ViewBag.Divisions = employeeManager.GetDivision();
            ViewBag.Departments = employeeManager.GetDepartment();

            //var from = DateTime.Now.AddMonths(-2);
            //var to = from.AddMonths(11);
            //var colspan1 = 0;
            //var colspan2 = 0;
            //for (DateTime d = from; d <= to; d = d.AddMonths(1))
            //{
            //    if (d.Year == from.Year)
            //        colspan1++;
            //    if (d.Year == to.Year)
            //        colspan2++;
            //}
            //if (from.Year == to.Year)
            //{
            //    colspan1 = 12;
            //    colspan2 = 0;
            //}
            ViewBag.Colspan1 = 12;
            ViewBag.Colspan2 = 0;
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
                if (affectedRows > 0)
                {
                    try
                    {
                        var emails = em.GetListEmail();
                        GMailer.GmailUsername = "nnduyen@gmail.com";
                        GMailer.GmailPassword = "gachip0864034";

                        GMailer mailer = new GMailer();

                        mailer.Subject = "[Project Management - New idea uploaded]";
                        mailer.Body = " Employee " + idea.EMP_ID + " Has uploaded an idea: " + idea.IDEA_TITLE + ".<br> Please check it out at<br> <a href='http://172.25.215.17/idea'>Project Management</a>";
                        mailer.IsHtml = true;
                        foreach (var email in emails)
                        {
                            mailer.ToEmail = email.EMAIL.Trim();
                            mailer.Send();
                        }
                    }

                    catch
                    {
                        return Json(affectedRows);
                    }
                }
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
            EmployeeManager em = EmployeeManager.GetInstance();
            IdeaManager idea = IdeaManager.GetInstance();

            string existID = prj.IsExist(Project.IDEA_ID);
            if (string.IsNullOrEmpty(existID))
            {
                // generate id
                if (string.IsNullOrWhiteSpace(Project.IDEA_ID))
                {
                    Project.IDEA_ID = idea.GenerateId();
                    idea.Insert(Project.EMP_ID, Project.IDEA_TITLE, Project.BACKGROUND, "", "");
                }
                result = prj.InsertProject(Project);
                if (result > 0)
                {
                    foreach (var p in Plans)
                    {
                        plan.InsertPlan(Project.IDEA_ID, p.PLAN_CONTENTS, p.PLAN_DATE);
                    }
                    foreach (var kpi in KPIs)
                    {
                        km.InsertKPI(Project.IDEA_ID, kpi.PRJ_MONTH, kpi.TARGET_VALUE);
                    }
                    try
                    {
                        var emails = em.GetListEmail();
                        GMailer.GmailUsername = "nnduyen@gmail.com";
                        GMailer.GmailPassword = "gachip0864034";

                        GMailer mailer = new GMailer();

                        mailer.Subject = "[Project Management - New project uploaded]";
                        mailer.Body = " Employee " + Project.EMP_ID + " Has uploaded a project: " + Project.IDEA_TITLE + ".<br> Please check it out at<br> <a href='http://172.25.215.17/idea'>Project Management</a>";
                        mailer.IsHtml = true;
                        foreach (var email in emails)
                        {
                            mailer.ToEmail = email.EMAIL.Trim();
                            mailer.Send();
                        }
                    }
                    catch
                    {
                        return Json(result);
                    }
                }
            }
            else
            {

            }
            return Json(result);

        }


        [HttpPost]
        public JsonResult GetIdea(DataTableParameters dataTableParameters, string div, string dep)
        {
            IdeaManager idea = IdeaManager.GetInstance();

            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = idea.SelectPaging(dataTableParameters.Start + 1, dataTableParameters.Start + dataTableParameters.Length + 1, div, dep);
            resultSet.recordsTotal = resultSet.recordsFiltered = idea.GetCount(div, dep);

            foreach (var i in lst)
            {
                var columns = new List<string>();
                columns.Add(i.ROWNUM.ToString());
                columns.Add((i.DIVISION == null) ? "" : i.DIVISION.Trim());
                columns.Add((i.DEPARTMENT == null) ? "" : i.DEPARTMENT.Trim());
                columns.Add((i.EMP_NAME == null) ? "" : i.EMP_NAME.Trim());
                columns.Add("<a class='title' href='#' id='" + i.ID + "' data-emp='" + i.EMP_ID.Trim() + "'>" + i.IDEA_TITLE.Trim() + "</a>");
                columns.Add((i.DATE == null) ? "" : i.DATE.ToShortDateString());
                columns.Add("<a href='#' class='rep' title='Reply' data-id='" + i.ID + "' data-trigger='focus'><span class='badge badge-pill badge-primary'>" + i.REP.ToString() + "</span></a>");
                columns.Add("<a href='#' class='like' title='Like' data-id='" + i.ID + "' data-table='1' data-trigger='focus'><span class='badge badge-pill badge-primary'>" + i.L.ToString() + "</span></a>");
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
            int result = repMng.Insert(reply.IDEA_ID, reply.REP_EMP_ID, reply.REP_EMP_NAME, reply.COMMENTS, DateTime.Now);
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
                    Project.REMARK, Project.NAME, Project.PRJ_CURR, Project.CURR_VALUE, Project.PRJECT_GRADE, Project.ATTACHMENT,
                    Project.ISSUE, Project.REQUEST);

                if (result > 0)
                {
                    if (Plans != null)
                    {
                        foreach (var p in Plans)
                        {
                            if (string.IsNullOrWhiteSpace(p.ID))
                            {
                                plan.InsertPlan(p.IDEA_ID, p.PLAN_CONTENTS, p.PLAN_DATE, p.COMPLETE_YN, p.COMPLETE_DATE);
                            }
                            else
                            {
                                plan.Update(p.IDEA_ID, p.PLAN_CONTENTS, p.PLAN_DATE, p.COMPLETE_YN, p.COMPLETE_DATE, p.ID);
                            }
                        }
                    }
                    if (KPIs != null)
                    {
                        foreach (var kpi in KPIs)
                        {
                            if (!string.IsNullOrWhiteSpace(kpi.ID))
                            {
                                km.Update(kpi.IDEA_ID, kpi.PRJ_MONTH, kpi.TARGET_VALUE, kpi.RESULT_VALUE, kpi.ID);
                            }
                            else
                            {
                                km.InsertKPI(kpi.IDEA_ID, kpi.PRJ_MONTH, kpi.TARGET_VALUE, kpi.RESULT_VALUE);
                            }
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

        [HttpPost]
        public JsonResult Like(EMPLOYEE EMP, string IDEA_ID)
        {
            LikeManager manager = LikeManager.GetInstance();
            bool checkLiked = manager.CheckLike(EMP.EMP_ID, IDEA_ID);
            if (checkLiked)
                return Json(-1);
            return Json(manager.Insert(IDEA_ID, EMP.EMP_ID, EMP.EMP_NAME));
        }

        [HttpPost]
        public JsonResult LikeIdea(EMPLOYEE EMP, string IDEA_ID)
        {
            LikeManager manager = LikeManager.GetInstance();
            bool checkLiked = manager.CheckLikeIdea(EMP.EMP_ID, IDEA_ID);
            if (checkLiked)
                return Json(-1);
            return Json(manager.InsertLikeIdea(IDEA_ID, EMP.EMP_ID, EMP.EMP_NAME));
        }

        [HttpPost]
        public JsonResult GetLikeDetail(string IDEA_ID, int table = 0)
        {
            LikeManager manager = LikeManager.GetInstance();
            if (table == 1)
                return Json(manager.GetLike1(IDEA_ID));
            return Json(manager.GetLike(IDEA_ID));
        }

        [HttpPost]
        public JsonResult GetRepDetail(string IDEA_ID, int table)
        {
            if (table == 0)
            {
                IdeaReplyManager manager = IdeaReplyManager.GetInstance();
                return Json(manager.GetReplyDetail(IDEA_ID));
            }
            else
            {
                ProjectReplyManager rep = ProjectReplyManager.GetInstance();
                List<PRJ_REPLY> lst = rep.GetReplyName(IDEA_ID);
                return Json(lst);
            }
        }

        [HttpPost]
        public JsonResult GetPrjSum(DataTableParameters dataTableParameters, string from = "", string to = "")
        {
            ProjectReplyManager prj = ProjectReplyManager.GetInstance();
            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = prj.GetReplySummary(from, to, dataTableParameters.Start + 1, dataTableParameters.Start + dataTableParameters.Length + 1);
            resultSet.recordsTotal = resultSet.recordsFiltered = prj.GetReplySumCount(from, to);

            foreach (var i in lst)
            {
                var columns = new List<string>();
                columns.Add((i.DIVISION == null) ? "" : i.DIVISION.Trim());
                columns.Add((i.DEPARTMENT == null) ? "" : i.DEPARTMENT.Trim());
                columns.Add(i.IDEA_TITLE.Trim());
                columns.Add((i.EMP_NAME == null) ? "" : i.EMP_NAME.Trim());
                columns.Add((i.LI == null) ? "" : i.LI.ToString());
                columns.Add((i.REP_EMP_NAME == null) ? "" : i.REP_EMP_NAME.ToString());
                columns.Add((i.COMMENTS == null) ? "" : i.COMMENTS.ToString());
                resultSet.data.Add(columns);

            }
            return Json(resultSet);

        }
        [HttpPost]
        public JsonResult GetIdeaSum(DataTableParameters dataTableParameters, string from = "", string to = "")
        {
            IdeaReplyManager prj = IdeaReplyManager.GetInstance();
            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = prj.GetReplySummary(from, to, dataTableParameters.Start + 1, dataTableParameters.Start + dataTableParameters.Length + 1);
            resultSet.recordsTotal = resultSet.recordsFiltered = prj.GetReplySumCount(from, to);

            foreach (var i in lst)
            {
                var columns = new List<string>();
                columns.Add((i.DIVISION == null) ? "" : i.DIVISION.Trim());
                columns.Add((i.DEPARTMENT == null) ? "" : i.DEPARTMENT.Trim());
                columns.Add(i.IDEA_TITLE.Trim());
                columns.Add((i.EMP_NAME == null) ? "" : i.EMP_NAME.Trim());
                columns.Add((i.LI == null) ? "" : i.LI.ToString());
                columns.Add((i.REP_EMP_NAME == null) ? "" : i.REP_EMP_NAME.ToString());
                columns.Add((i.COMMENTS == null) ? "" : i.COMMENTS.ToString());
                resultSet.data.Add(columns);

            }
            return Json(resultSet);

        }
        [HttpPost]
        public JsonResult GetProjectSum(DataTableParameters dataTableParameters, string from = "", string to = "")
        {
            ProjectReplyManager prj = ProjectReplyManager.GetInstance();
            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = prj.GetReplySummary(from, to, dataTableParameters.Start + 1, dataTableParameters.Start + dataTableParameters.Length + 1);
            resultSet.recordsTotal = resultSet.recordsFiltered = prj.GetReplySumCount(from, to);

            foreach (var i in lst)
            {
                var columns = new List<string>();
                columns.Add((i.DIVISION == null) ? "" : i.DIVISION.Trim());
                columns.Add((i.DEPARTMENT == null) ? "" : i.DEPARTMENT.Trim());
                columns.Add(i.IDEA_TITLE.Trim());
                columns.Add((i.EMP_NAME == null) ? "" : i.EMP_NAME.Trim());
                columns.Add((i.numLike == null) ? "" : i.numLike.ToString());
                columns.Add((i.REP_EMP_NAME == null) ? "" : i.REP_EMP_NAME.ToString());
                columns.Add((i.COMMENTS == null) ? "" : i.COMMENTS.ToString());
                resultSet.data.Add(columns);

            }
            return Json(resultSet);

        }

        [HttpPost]
        public JsonResult DeleteIdea(string ID)
        {
            if (string.IsNullOrWhiteSpace(ID))
                return Json(-1);
            var result = 0;
            IdeaManager lm = IdeaManager.GetInstance();
            lm.DeleteLikeReply(ID);
            result = lm.Delete(ID);
            return Json(result);
        }

        [HttpPost]
        public JsonResult DeleteProject(string ID)
        {
            if (string.IsNullOrWhiteSpace(ID))
                return Json(-1);
            var result = 0;
            ProjectManager pm = ProjectManager.GetInstance();
            PlanManager plan = PlanManager.GetInstance();
            KpiManager kpi = KpiManager.GetInstance();

            plan.DeletePlanByIdeaId(ID);
            kpi.DeleteKPIByIdeaId(ID);
            pm.DeleteLikeReply(ID);
            result = pm.Delete(ID);
            return Json(result);
        }

        [HttpPost]
        public JsonResult RemovePlan(string ID)
        {
            if (string.IsNullOrWhiteSpace(ID))
                return Json(-1);
            var result = 0;
            PlanManager pm = PlanManager.GetInstance();
            result = pm.Delete(ID);
            return Json(result);
        }

        [HttpPost]
        public JsonResult Delegate(string ID, string EMP_ID)
        {
            if (string.IsNullOrWhiteSpace(ID))
                return Json(-1);
            var result = 0;
            IdeaManager im = IdeaManager.GetInstance();
            result = im.Delegate(EMP_ID, ID);
            return Json(result);
        }

        [HttpPost]
        public ActionResult ExportProject(string from = "", string to = "")
        {
            ProjectReplyManager ir = ProjectReplyManager.GetInstance();

            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            var list = ir.GetReplySummaryExport(from, to);
            DataTable dtb = new DataTable();

            dtb.Clear();

            dtb.Columns.Add("IDEA_TITLE");
            dtb.Columns.Add("EMP_NAME");
            dtb.Columns.Add("DEPARTMENT");
            dtb.Columns.Add("DIVISION");
            dtb.Columns.Add("REP_EMP_NAME");
            dtb.Columns.Add("LIKE");
            dtb.Columns.Add("COMMENTS");
            foreach (var item in list)
            {
                DataRow r = dtb.NewRow();
                r["IDEA_TITLE"] = item.IDEA_TITLE;
                r["EMP_NAME"] = item.EMP_NAME;
                r["DEPARTMENT"] = item.DEPARTMENT;
                r["DIVISION"] = item.DIVISION;
                r["REP_EMP_NAME"] = item.REP_EMP_NAME;
                r["COMMENTS"] = item.COMMENTS;
                r["LIKE"] = item.numLike;
                dtb.Rows.Add(r);
            }
            // Gọi lại hàm để tạo file excel
            var stream = CreateExcelFile(dtb);
            // Tạo buffer memory strean để hứng file excel
            var buffer = stream as MemoryStream;
            // Đây là content Type dành cho file excel, còn rất nhiều content-type khác nhưng cái này mình thấy okay nhất
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            // Dòng này rất quan trọng, vì chạy trên firefox hay IE thì dòng này sẽ hiện Save As dialog cho người dùng chọn thư mục để lưu
            // File name của Excel này là ExcelDemo
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".xlsx");
            // Lưu file excel của chúng ta như 1 mảng byte để trả về response
            Response.BinaryWrite(buffer.ToArray());
            // Send tất cả ouput bytes về phía clients
            Response.Flush();
            Response.End();
            return RedirectToAction("Summary");
        }

        [HttpPost]
        public ActionResult ExportIdea(string from = "", string to = "")
        {
            IdeaReplyManager ir = IdeaReplyManager.GetInstance();

            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            var list = ir.GetReplySummaryExport(from, to);
            DataTable dtb = new DataTable();

            dtb.Clear();

            dtb.Columns.Add("IDEA_TITLE");
            dtb.Columns.Add("EMP_NAME");
            dtb.Columns.Add("DEPARTMENT");
            dtb.Columns.Add("DIVISION");
            dtb.Columns.Add("REP_EMP_NAME");
            dtb.Columns.Add("COMMENTS");
            dtb.Columns.Add("LIKE");
            foreach (var item in list)
            {
                DataRow r = dtb.NewRow();
                r["IDEA_TITLE"] = item.IDEA_TITLE;
                r["EMP_NAME"] = item.EMP_NAME;
                r["DEPARTMENT"] = item.DEPARTMENT;
                r["DIVISION"] = item.DIVISION;
                r["REP_EMP_NAME"] = item.REP_EMP_NAME;
                r["COMMENTS"] = item.COMMENTS;
                r["LIKE"] = item.numLike;
                dtb.Rows.Add(r);
            }
            // Gọi lại hàm để tạo file excel
            var stream = CreateExcelFile(dtb);
            // Tạo buffer memory strean để hứng file excel
            var buffer = stream as MemoryStream;
            // Đây là content Type dành cho file excel, còn rất nhiều content-type khác nhưng cái này mình thấy okay nhất
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            // Dòng này rất quan trọng, vì chạy trên firefox hay IE thì dòng này sẽ hiện Save As dialog cho người dùng chọn thư mục để lưu
            // File name của Excel này là ExcelDemo
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".xlsx");
            // Lưu file excel của chúng ta như 1 mảng byte để trả về response
            Response.BinaryWrite(buffer.ToArray());
            // Send tất cả ouput bytes về phía clients
            Response.Flush();
            Response.End();
            return RedirectToAction("Summary");
        }

        private Stream CreateExcelFile(DataTable dtb, Stream stream = null)
        {
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                // Tạo author cho file Excel
                excelPackage.Workbook.Properties.Author = "Export";
                // Tạo title cho file Excel
                excelPackage.Workbook.Properties.Title = "Export";
                // Add Sheet vào file Excel
                excelPackage.Workbook.Worksheets.Add("First Sheet");
                // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                var workSheet = excelPackage.Workbook.Worksheets[1];
                // Đổ data vào Excel file
                workSheet.Cells[1, 1].LoadFromDataTable(dtb, true);
                // BindingFormatForExcel(workSheet, list);
                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        public ActionResult ExportProgress(string IDEA_ID)
        {
            ProjectManager pm = ProjectManager.GetInstance();
            PlanManager plan = PlanManager.GetInstance();
            KpiManager km = KpiManager.GetInstance();
            EmployeeManager em = EmployeeManager.GetInstance();

            List<KPI> KPIs = km.Select(IDEA_ID);
            List<PROJECT_PLAN> Plans = plan.Select(IDEA_ID);
            PROJECT Project = pm.Select(IDEA_ID).FirstOrDefault();
            EMPLOYEE EMP = em.Select(Project.EMP_ID).FirstOrDefault();

            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var template = Server.MapPath("~/Template/Progress.xlsx");
            using (ExcelPackage package = new ExcelPackage(new FileInfo(template)))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.FirstOrDefault();
                ws.Cells["B9:N18"].Value = "";
                ws.Cells["B1"].Value = EMP.DIVISION.Trim();
                ws.Cells["G1"].Value = EMP.DEPARTMENT.Trim();
                ws.Cells["L1"].Value = EMP.EMP_NAME.Trim();
                ws.Cells["B2"].Value = Project.IDEA_TITLE.Trim();
                ws.Cells["L2"].Value = Project.PRJECT_GRADE;
                ws.Cells["B19"].Value = Project.REMARK == null ? Project.REMARK : Project.REMARK.Trim();
                ws.Cells["D3"].Value = Project.KPI_NAME == null ? Project.KPI_NAME : Project.KPI_NAME.Trim();
                ws.Cells["L3"].Value = Project.KPI_UNIT == null ? Project.KPI_UNIT : Project.KPI_UNIT.Trim();
                ws.Cells["C4"].Value = Project.PRJ_CURR;
                ws.Cells["C6"].Value = Project.PRJ_CURR;
                ws.Cells["C7"].Value = Project.CURR_VALUE;

                var year1 = KPIs[0].PRJ_MONTH.Substring(0, 4);
                var year2 = KPIs[KPIs.Count - 1].PRJ_MONTH.Substring(0, 4);
                if (KPIs[0].PRJ_MONTH.Substring(0, 4) != KPIs[KPIs.Count - 1].PRJ_MONTH.Substring(0, 4))
                {

                    var c = 'D';
                    foreach (var item in KPIs)
                    {
                        if (year1 == item.PRJ_MONTH.Substring(0, 4))
                        {
                            ws.Cells["D4:" + c + "4"].Merge = true;
                            c++;
                        }
                    }
                    ws.Cells["D4"].Value = year1;
                    ws.Cells["D4:" + c + "4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    c++;

                    ws.Cells[c + "4:O4"].Merge = true;
                    ws.Cells[c + "4"].Value = year2;
                    ws.Cells[c + "4:O4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                else
                {
                    ws.Cells["D4:O4"].Merge = true;
                    ws.Cells["D4:O4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["D4"].Value = year1;
                }
                var firstMonth = new DateTime(Convert.ToInt32(year1),1, 1);
                //var firstMonth = new DateTime(Convert.ToInt32(year1), Convert.ToInt32(KPIs[0].PRJ_MONTH.Substring(5, 2)), 1);
                for (var i = 'D'; i <= 'O'; i++)
                {
                    ws.Cells[i + 5.ToString()].Value = firstMonth.Month;
                    foreach (var kpi in KPIs)
                    {
                        if (kpi.PRJ_MONTH.Equals(firstMonth.ToString("yyyy-MM")))
                        {
                            ws.Cells[i + 6.ToString()].Value = kpi.TARGET_VALUE;
                            ws.Cells[i + 7.ToString()].Value = kpi.RESULT_VALUE;
                        }

                    }
                    firstMonth = firstMonth.AddMonths(1);
                }

                var seq = 9;
                foreach (var item in Plans)
                {
                    var complete = item.COMPLETE_YN == 0 ? "NO" : "YES";
                    ws.Cells["B" + seq.ToString()].Value = complete;
                    ws.Cells["C" + seq.ToString()].Value = item.COMPLETE_DATE == DateTime.MinValue ? "" : item.COMPLETE_DATE.ToString("yyyy-MM-dd");
                    ws.Cells["E" + seq.ToString()].Value = item.PLAN_CONTENTS.Trim();
                    ws.Cells["N" + seq.ToString()].Value = item.PLAN_DATE == DateTime.MinValue ? "" : item.PLAN_DATE.ToString("yyyy-MM-dd");
                    seq++;
                }
                //buffer = package.Stream as MemoryStream;
                Byte[] fileBytes = package.GetAsByteArray();
                Response.ClearContent();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=" + fileName);
                Response.BinaryWrite(fileBytes);
                Response.Flush();
                Response.End();

            }

            return RedirectToAction("Index");
        }

    }
}
