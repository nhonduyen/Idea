﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class ProjectManager:PROJECT
    {
        private static ProjectManager instance = new ProjectManager();

        public static ProjectManager GetInstance()
        {
            return instance;
        }

        public dynamic GetMainProject(int start = 0, int end = 10)
        {
            var sql = string.Format(@"SELECT * FROM(SELECT ROW_NUMBER() OVER (order by IDEA_ID desc) AS ROWNUM, P.IDEA_ID,P.EMP_ID,P.IDEA_TITLE,
P.KPI_NAME,P.KPI_UNIT,E.DIVISION, E.DEPARTMENT, (SELECT TOP 1 TARGET_VALUE FROM KPI AS K WHERE K.IDEA_ID=P.IDEA_ID ORDER BY PRJ_MONTH DESC) AS FINAL
FROM PROJECT AS P INNER JOIN EMPLOYEE AS E ON E.EMP_ID=P.EMP_ID WHERE EXISTS(SELECT IDEA_ID FROM PROJECT_PLAN AS PL WHERE P.IDEA_ID=PL.IDEA_ID AND COMPLETE_YN=1)) as u 
WHERE RowNum >= @start   AND RowNum < @end ORDER BY RowNum;");

            return DBManager<PROJECT>.ExecuteDynamic(sql, new { start = start, end = end });
        }
        public dynamic GetProject(string ID)
        {
            var sql = "SELECT I.*, E.DEPARTMENT,E.DIVISION,E.EMP_NAME FROM PROJECT AS I INNER JOIN EMPLOYEE AS E ON I.EMP_ID=E.EMP_ID WHERE IDEA_ID=@ID";
            return DBManager<dynamic>.ExecuteReader(sql, new { ID = ID }).FirstOrDefault();
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

        public int UpdatePrj(string IDEA_ID, string EMP_ID, string IDEA_TITLE, string KPI_NAME, string KPI_UNIT, string REMARK, string NAME)
        {
            var sql =string.Format(@"UPDATE PROJECT SET IDEA_ID=@IDEA_ID,EMP_ID=@EMP_ID,IDEA_TITLE=@IDEA_TITLE,KPI_NAME=@KPI_NAME,
                                   NAME=@NAME,KPI_UNIT=@KPI_UNIT,REMARK=@REMARK WHERE IDEA_ID=@IDEA_ID");

            return DBManager<PROJECT>.Execute(sql, new { IDEA_ID = IDEA_ID, EMP_ID = EMP_ID, IDEA_TITLE = IDEA_TITLE, 
                KPI_NAME = KPI_NAME, KPI_UNIT = KPI_UNIT, REMARK = REMARK, NAME=NAME });
        }

      
    }
}