using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class ProjectReplyManager : PRJ_REPLY
    {
        private static ProjectReplyManager instance = new ProjectReplyManager();

        public static ProjectReplyManager GetInstance()
        {
            return instance;
        }

        public dynamic GetReplySummary(string from="", string to="", int start=0, int end=10)
        {
            var sql =string.Format(@"
SELECT * FROM(SELECT ROW_NUMBER() OVER (order by P.IDEA_ID) AS ROWNUM,
(SELECT COUNT(1) FROM PRJ_LIKE AS L WHERE L.IDEA_ID=P.IDEA_ID) AS LI,
(SELECT COUNT(1) FROM PRJ_REPLY AS R WHERE R.IDEA_ID=P.IDEA_ID) AS REP,
P.IDEA_TITLE,E.DEPARTMENT,E.DIVISION,PR.REP_EMP_NAME,PR.COMMENTS
FROM PROJECT AS P
LEFT JOIN PRJ_REPLY AS PR 
ON P.IDEA_ID=PR.IDEA_ID
INNER JOIN EMPLOYEE AS E 
ON PR.REP_EMP_ID=E.EMP_ID
WHERE (@FROM='' OR P.INS_DT BETWEEN @FROM AND @TO)
) AS U
WHERE RowNum >= @start   AND RowNum < @end ORDER BY LI,REP DESC
");
            return DBManager<PRJ_REPLY>.ExecuteDynamic(sql, new { FROM = from, TO=to, start=start, end=end });
        }

        public dynamic GetReplySummaryExport(string from = "", string to = "")
        {
            var sql = string.Format(@"
SELECT * FROM( SELECT
(SELECT COUNT(1) FROM PRJ_LIKE AS L WHERE L.IDEA_ID=P.IDEA_ID) AS numLike,
(SELECT COUNT(1) FROM PRJ_REPLY AS R WHERE R.IDEA_ID=P.IDEA_ID) AS REP,
P.IDEA_TITLE,E.DEPARTMENT,E.DIVISION,E.EMP_NAME,PR.REP_EMP_NAME,PR.COMMENTS
FROM PROJECT AS P
LEFT JOIN PRJ_REPLY AS PR 
ON P.IDEA_ID=PR.IDEA_ID
INNER JOIN EMPLOYEE AS E 
ON PR.REP_EMP_ID=E.EMP_ID
WHERE (@FROM ='' OR P.INS_DT BETWEEN @FROM AND @TO)
) AS U ORDER BY numLike,REP DESC
");
            return DBManager<PRJ_REPLY>.ExecuteDynamic(sql, new { FROM = from, TO = to });
        }

        public int GetReplySumCount(string from = "", string to = "")
        {
            var sql = string.Format(@"
SELECT count(*) 
FROM PROJECT AS P
LEFT JOIN PRJ_REPLY AS PR 
ON P.IDEA_ID=PR.IDEA_ID
INNER JOIN EMPLOYEE AS E 
ON PR.REP_EMP_ID=E.EMP_ID
WHERE (@FROM='' OR P.INS_DT BETWEEN @FROM AND @TO)
");
            return (int) DBManager<PRJ_REPLY>.ExecuteScalar(sql, new { FROM = from, TO = to });
        }

        public dynamic GetReply(string IDEA_ID)
        {
            var sql = "SELECT PR.*,E.DEPARTMENT,E.EMP_NAME FROM PRJ_REPLY AS PR INNER JOIN EMPLOYEE AS E ON PR.REP_EMP_ID=E.EMP_ID WHERE IDEA_ID=@IDEA_ID ORDER BY INS_DATE DESC";
            return DBManager<PRJ_REPLY>.ExecuteDynamic(sql, new { IDEA_ID = IDEA_ID });
        }

        public int GetReplyCount(string IDEA_ID)
        {
            var sql = "SELECT COUNT(1) FROM PRJ_REPLY WHERE IDEA_ID=@IDEA_ID";
            return (int)DBManager<PRJ_REPLY>.ExecuteScalar(sql, new { IDEA_ID = IDEA_ID });
        }

        public int InsertReply(PRJ_REPLY rep)
        {
            //string ID = GenerateId();
            var sql = "INSERT INTO PRJ_REPLY(IDEA_ID,REP_EMP_ID,REP_EMP_NAME,COMMENTS,INS_DATE) VALUES(@IDEA_ID,@REP_EMP_ID,@REP_EMP_NAME,@COMMENTS,GETDATE())";
            return DBManager<PRJ_REPLY>.Execute(sql, new
            {
                IDEA_ID = rep.IDEA_ID,
                REP_EMP_ID = rep.REP_EMP_ID,
                REP_EMP_NAME = rep.REP_EMP_NAME,
                COMMENTS = rep.COMMENTS,
                INS_DATE = INS_DATE
            });
        }
        public List<PRJ_REPLY> GetReplyName(string IDEA_ID)
        {
            var sql = "SELECT REP_EMP_NAME FROM PRJ_REPLY WHERE IDEA_ID=@IDEA_ID";
            return DBManager<PRJ_REPLY>.ExecuteReader(sql, new { IDEA_ID = IDEA_ID });
        }
    }
}