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