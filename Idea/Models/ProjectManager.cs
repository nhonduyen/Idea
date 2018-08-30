using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class ProjectManager : PROJECT
    {
        private static ProjectManager instance = new ProjectManager();

        public static ProjectManager GetInstance()
        {
            return instance;
        }

        public dynamic GetMainProject(int start = 0, int end = 10)
        {
            var sql = string.Format(@"SELECT * FROM(SELECT ROW_NUMBER() OVER (order by IDEA_ID desc) AS ROWNUM, P.IDEA_ID,P.EMP_ID,P.IDEA_TITLE,P.PRJECT_GRADE,
P.KPI_NAME,P.KPI_UNIT,E.DIVISION, E.DEPARTMENT, 
(SELECT COUNT(1) FROM PRJ_REPLY AS PR WHERE PR.IDEA_ID=P.IDEA_ID) AS REP,
(SELECT COUNT(1) FROM PRJ_LIKE AS L WHERE L.IDEA_ID=P.IDEA_ID) AS L,
(SELECT TOP 1 TARGET_VALUE FROM KPI AS K WHERE K.IDEA_ID=P.IDEA_ID ORDER BY PRJ_MONTH DESC) AS FINAL
FROM PROJECT AS P INNER JOIN EMPLOYEE AS E ON E.EMP_ID=P.EMP_ID WHERE PRJECT_GRADE='S' OR PRJECT_GRADE='A') as u 
WHERE RowNum >= @start   AND RowNum < @end ORDER BY RowNum;");

            return DBManager<PROJECT>.ExecuteDynamic(sql, new { start = start, end = end });
        }
        public dynamic SearchNewPrj(string div, string dep, string grade, int start = 0, int end = 10)
        {
            var condition = string.Empty;
            if (!string.IsNullOrWhiteSpace(div))
                condition += "AND DIVISION='" + div + "' ";
            if (!string.IsNullOrWhiteSpace(dep))
                condition += "AND DEPARTMENT='" + dep + "' ";
            if (!string.IsNullOrWhiteSpace(grade))
                condition += "AND PRJECT_GRADE='" + grade + "' ";
            var sql = string.Format(@"SELECT * FROM(SELECT ROW_NUMBER() OVER (order by IDEA_ID desc) AS ROWNUM, P.IDEA_ID,P.EMP_ID,P.IDEA_TITLE,P.NAME,
P.INS_DT,
(SELECT COUNT(1) FROM PRJ_REPLY AS PR WHERE PR.IDEA_ID=P.IDEA_ID) AS REP,
(SELECT COUNT(1) FROM PRJ_LIKE AS L WHERE L.IDEA_ID=P.IDEA_ID) AS L
,(SELECT TOP 1 TARGET_VALUE FROM KPI AS K WHERE K.IDEA_ID=P.IDEA_ID ORDER BY PRJ_MONTH DESC) AS FINAL
FROM PROJECT AS P INNER JOIN EMPLOYEE AS E ON E.EMP_ID=P.EMP_ID WHERE 1=1 condition) as u 
WHERE RowNum >= @start   AND RowNum < @end ORDER BY RowNum;");
            if (!string.IsNullOrEmpty(condition))
                sql = sql.Replace("condition", condition);
            return DBManager<PROJECT>.ExecuteDynamic(sql, new { start = start, end = end });
        }

        public int GetSearchCount(string div, string dep, string grade)
        {
            var condition = string.Empty;
            if (!string.IsNullOrWhiteSpace(div))
                condition += "AND DIVISION LIKE '%" + div + "%' ";
            if (!string.IsNullOrWhiteSpace(dep))
                condition += "AND DEPARTMENT LIKE '%" + dep + "%' ";
            if (!string.IsNullOrWhiteSpace(grade))
                condition += "AND PRJECT_GRADE LIKE '%" + grade + "%' ";
            var sql = string.Format(@"SELECT COUNT(1) AS CNT
FROM PROJECT AS P INNER JOIN EMPLOYEE AS E ON E.EMP_ID=P.EMP_ID WHERE 1=1 {0}",condition);
           
            return (int)DBManager<PROJECT>.ExecuteScalar(sql);
        }
      
        public dynamic GetProject(string ID)
        {
            var sql = "SELECT I.*, E.DEPARTMENT,E.DIVISION,E.EMP_NAME FROM PROJECT AS I INNER JOIN EMPLOYEE AS E ON I.EMP_ID=E.EMP_ID WHERE IDEA_ID=@ID";
            return DBManager<dynamic>.ExecuteReader(sql, new { ID = ID }).FirstOrDefault();
        }
        public dynamic GetProject( string div = "", string dept = "", string grade = "", int page=0)
        {
            var start = (page-1) * 5 + 1;
            var end = start + 5 - 1;
            var sql = string.Format(@"SELECT * FROM(SELECT ROW_NUMBER() OVER (order by IDEA_ID desc) AS ROWNUM, P.IDEA_ID,P.EMP_ID,E.EMP_NAME,E.DIVISION,E.DEPARTMENT,P.IDEA_TITLE,P.NAME,
P.PRJECT_GRADE,P.KPI_UNIT,P.KPI_NAME,P.INS_DT,
(SELECT COUNT(1) FROM PRJ_REPLY AS PR WHERE PR.IDEA_ID=P.IDEA_ID) AS REP,
(SELECT COUNT(1) FROM PRJ_LIKE AS L WHERE L.IDEA_ID=P.IDEA_ID) AS L
FROM PROJECT AS P INNER JOIN EMPLOYEE AS E ON E.EMP_ID=P.EMP_ID WHERE 
(@div='' OR DIVISION LIKE '%'+@div+'%') 
AND (@dept='' OR DEPARTMENT LIKE '%'+@dept+'%') 
AND (@grade='' OR PRJECT_GRADE=@grade) ) as u 
WHERE RowNum >= @start   AND RowNum <= @end ORDER BY RowNum");
            return DBManager<dynamic>.ExecuteDynamic(sql, new
            {
               
                div = div,
                dept = dept,
                grade = grade,
                start=start,
                end=end
            });
        }
        public List<string> GetKPIUnit()
        {
            var sql = "SELECT DISTINCT(KPI_UNIT) FROM PROJECT WHERE KPI_UNIT IS NOT NULL";
            List<PROJECT> prjs = DBManager<PROJECT>.ExecuteReader(sql);
            List<string> units = new List<string>();
            foreach (var prj in prjs)
            {
                units.Add(prj.KPI_UNIT);
            }
            return units;
        }
        public string IsExist(string ID)
        {
            var sql = "SELECT TOP 1 IDEA_ID FROM PROJECT WHERE IDEA_ID=@ID";
            PROJECT prj = DBManager<PROJECT>.ExecuteReader(sql, new { ID = ID }).FirstOrDefault();
            if (prj == null)
                return string.Empty;
            return prj.IDEA_ID;
        }
        public string GetLastId()
        {
            var sql = "SELECT TOP 1 IDEA_ID FROM PROJECT ORDER BY IDEA_ID DESC";
            PROJECT prj = DBManager<PROJECT>.ExecuteReader(sql).FirstOrDefault();
            if (prj == null)
                return string.Empty;
            return prj.IDEA_ID;
        }

        public int UpdatePrj(string IDEA_ID, string EMP_ID, string IDEA_TITLE, string KPI_NAME, string KPI_UNIT, string REMARK,
            string NAME, string PRJ_CURR, decimal CURR_VALUE, string PRJECT_GRADE, string ATTACHMENT, string ISSUE, string REQUEST)
        {
            var sql = string.Format(@"UPDATE PROJECT SET IDEA_ID=@IDEA_ID,EMP_ID=@EMP_ID,IDEA_TITLE=@IDEA_TITLE,KPI_NAME=@KPI_NAME,
                                   NAME=@NAME,KPI_UNIT=@KPI_UNIT,REMARK=@REMARK,CURR_VALUE=@CURR_VALUE,PRJ_CURR=@PRJ_CURR,
                                   PRJECT_GRADE=@PRJECT_GRADE,ATTACHMENT=@ATTACHMENT,ISSUE=@REMARK,REQUEST=@REQUEST WHERE IDEA_ID=@IDEA_ID");

            return DBManager<PROJECT>.Execute(sql, new
            {
                IDEA_ID = IDEA_ID,
                EMP_ID = EMP_ID,
                IDEA_TITLE = IDEA_TITLE,
                KPI_NAME = KPI_NAME,
                KPI_UNIT = KPI_UNIT,
                REMARK = REMARK,
                NAME = NAME,
                PRJ_CURR = PRJ_CURR,
                CURR_VALUE = CURR_VALUE,
                PRJECT_GRADE = PRJECT_GRADE,
                ATTACHMENT = ATTACHMENT,
                REQUEST = REQUEST
            });
        }

        public int UpdateTranslate(string IDEA_ID, string REQUEST, string ISSUE)
        {
            var sql = string.Format(@"UPDATE PROJECT SET REQUEST_KOREAN=@REQUEST,ISSUE_KOREAN=@ISSUE WHERE IDEA_ID=@IDEA_ID");

            return DBManager<PROJECT>.Execute(sql, new
            {
                IDEA_ID = IDEA_ID,
                REQUEST = REQUEST,
                ISSUE = ISSUE
            });
        }

        public int InsertProject(PROJECT project)
        {
            var sql = string.Format(@"INSERT INTO PROJECT(IDEA_ID,EMP_ID,IDEA_TITLE,PRJECT_GRADE,KPI_NAME,KPI_UNIT,BACKGROUND,NAME,
            INS_DT,PRJ_CURR,CURR_VALUE,ATTACHMENT) VALUES(@IDEA_ID,@EMP_ID,@IDEA_TITLE,@PRJECT_GRADE,@KPI_NAME,@KPI_UNIT,@BACKGROUND,@NAME,
            GETDATE(),@PRJ_CURR,@CURR_VALUE,@ATTACHMENT)");
            return DBManager<PROJECT>.Execute(sql, project);
        }

        public void DeleteLikeReply(string IdeaId)
        {
            var sql = "DELETE FROM PRJ_LIKE WHERE IDEA_ID=@ID;";
            var sql1 = "DELETE FROM PRJ_REPLY WHERE IDEA_ID=@ID;";
            DBManager<PRJ_LIKE>.Execute(sql, new { ID = IdeaId });
            DBManager<PRJ_REPLY>.Execute(sql1, new { ID = IdeaId });
        }
    }
}