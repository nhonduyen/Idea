using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class ProjectReplyManager:PRJ_REPLY
    {
        private static ProjectReplyManager instance = new ProjectReplyManager();

        public static ProjectReplyManager GetInstance()
        {
            return instance;
        }

        public dynamic GetReply(string IDEA_ID)
        {
            var sql = "SELECT PR.*,E.DEPARTMENT,E.EMP_NAME FROM PRJ_REPLY AS PR INNER JOIN EMPLOYEE AS E ON PR.REP_EMP_ID=E.EMP_ID WHERE IDEA_ID=@IDEA_ID";
            return DBManager<PRJ_REPLY>.ExecuteDynamic(sql, new { IDEA_ID = IDEA_ID });
        }
        public string GenerateId()
        {
            string id = DateTime.Now.ToString("yyyyMMdd");
            string sql = "select top 1 ID  from PRJ_REPLY WHERE ID LIKE @id + '%' order by ID desc";

            PRJ_REPLY idea = DBManager<PRJ_REPLY>.ExecuteReader(sql, new { id = id }).FirstOrDefault();
            if (idea == null)
            {
                id = id + "0001";
            }
            else
            {
                string str = idea.ID.Trim().Substring(8);
                string num = (Convert.ToInt32(str) + 1).ToString();
                for (int i = 0; i < str.Length - num.Length; i++)
                {
                    id += "0";
                }
                id += num.ToString();
            }
            return id;
        }
        public int InsertReply(PRJ_REPLY rep)
        {
            string ID = GenerateId();
            var sql = "INSERT INTO PRJ_REPLY(ID,IDEA_ID,REP_EMP_ID,REP_EMP_NAME,COMMENTS,INS_DATE) VALUES(@ID,@IDEA_ID,@REP_EMP_ID,@REP_EMP_NAME,@COMMENTS,GETDATE())";
            return DBManager<PRJ_REPLY>.Execute(sql, new { ID = ID, IDEA_ID = rep.IDEA_ID, REP_EMP_ID = rep.REP_EMP_ID, REP_EMP_NAME = rep.REP_EMP_NAME, COMMENTS = rep.COMMENTS, INS_DATE = INS_DATE });
        }
       
    }
}