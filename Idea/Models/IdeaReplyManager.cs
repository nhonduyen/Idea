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
            var sql = "SELECT E.EMP_NAME,E.DEPARTMENT,R.COMMENTS,R.INS_DATE FROM IDEA_REPLY AS R INNER JOIN EMPLOYEE AS E ON R.REP_EMP_ID=E.EMP_ID WHERE IDEA_ID=@ID";
            return DBManager<dynamic>.ExecuteDynamic(sql, new { ID = ID });
        }

        public dynamic GetReplyDetail(string ID)
        {
            var sql = "SELECT E.EMP_NAME FROM IDEA_REPLY AS R INNER JOIN EMPLOYEE AS E ON R.REP_EMP_ID=E.EMP_ID WHERE IDEA_ID=@ID";
            return DBManager<dynamic>.ExecuteDynamic(sql, new { ID = ID });
        }
    }
}