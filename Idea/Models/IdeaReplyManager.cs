using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class IdeaReplyManager:IDEA_REPLY
    {
        private static IdeaReplyManager instance = new IdeaReplyManager();

        public static IdeaReplyManager GetInstance()
        {
            return instance;
        }

       
        public dynamic GetReplyByIdeaId(string ID)
        {
            var sql = "SELECT E.EMP_NAME,E.DEPARTMENT,R.COMMENTS,R.INS_DATE FROM IDEA_REPLY AS R INNER JOIN EMPLOYEE AS E ON R.REP_EMP_ID=E.EMP_ID WHERE IDEA_ID=@ID ORDER BY INS_DATE DESC";
            return DBManager<dynamic>.ExecuteDynamic(sql, new { ID = ID });
        }

        public dynamic GetReplyDetail(string ID)
        {
            var sql = "SELECT E.EMP_NAME FROM IDEA_REPLY AS R INNER JOIN EMPLOYEE AS E ON R.REP_EMP_ID=E.EMP_ID WHERE IDEA_ID=@ID";
            return DBManager<dynamic>.ExecuteDynamic(sql, new { ID = ID });
        }

        public dynamic GetReplySummary(string from = "", string to = "", int start = 0, int end = 10)
        {
            var sql = string.Format(@"
SELECT * FROM(SELECT ROW_NUMBER() OVER (order by P.ID) AS ROWNUM,
(SELECT COUNT(1) FROM IDEA_LIKE AS L WHERE L.IDEA_ID=P.ID) AS LI,
(SELECT COUNT(1) FROM IDEA_REPLY AS R WHERE R.IDEA_ID=P.ID) AS REP,
P.IDEA_TITLE,E.DEPARTMENT,E.DIVISION,E.EMP_NAME,PR.REP_EMP_NAME,PR.COMMENTS
FROM IDEA AS P
LEFT JOIN IDEA_REPLY AS PR 
ON P.ID=PR.IDEA_ID
INNER JOIN EMPLOYEE AS E 
ON P.EMP_ID=E.EMP_ID
WHERE NOT EXISTS(SELECT IDEA_ID FROM PROJECT AS PJ WHERE PJ.IDEA_ID=P.ID ) AND (@FROM='' OR P.DATE BETWEEN @FROM AND @TO)
) AS U
WHERE RowNum >= @start   AND RowNum < @end ORDER BY LI,REP DESC
");
            return DBManager<PRJ_REPLY>.ExecuteDynamic(sql, new { FROM = from, TO = to, start = start, end = end });
        }

        public dynamic GetReplySummaryExport(string from = "", string to = "")
        {
            var sql = string.Format(@"
SELECT
(SELECT COUNT(1) FROM IDEA_LIKE AS L WHERE L.IDEA_ID=P.ID) AS numLike,
(SELECT COUNT(1) FROM IDEA_REPLY AS R WHERE R.IDEA_ID=P.ID) AS REP,
P.IDEA_TITLE,E.DEPARTMENT,E.DIVISION,E.EMP_NAME,PR.REP_EMP_NAME,PR.COMMENTS
FROM IDEA AS P
LEFT JOIN IDEA_REPLY AS PR 
ON P.ID=PR.IDEA_ID
INNER JOIN EMPLOYEE AS E 
ON P.EMP_ID=E.EMP_ID
WHERE NOT EXISTS(SELECT IDEA_ID FROM PROJECT AS PJ WHERE PJ.IDEA_ID=P.ID ) AND (@FROM ='' OR P.DATE BETWEEN @FROM AND @TO)
ORDER BY numLike,REP DESC
");
            return DBManager<PRJ_REPLY>.ExecuteDynamic(sql, new { FROM = from, TO = to});
        }
        public int GetReplySumCount(string from = "", string to = "")
        {
            var sql = string.Format(@"
SELECT COUNT(*)

FROM IDEA AS P
LEFT JOIN IDEA_REPLY AS PR 
ON P.ID=PR.IDEA_ID
INNER JOIN EMPLOYEE AS E 
ON P.EMP_ID=E.EMP_ID
WHERE NOT EXISTS(SELECT IDEA_ID FROM PROJECT AS PJ WHERE PJ.IDEA_ID=P.ID ) AND (@FROM='' OR P.DATE BETWEEN @FROM AND @TO)
");
            return (int)DBManager<PRJ_REPLY>.ExecuteScalar(sql, new { FROM = from, TO = to });
        }

    }
}