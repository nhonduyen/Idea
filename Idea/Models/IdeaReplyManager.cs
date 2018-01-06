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

        public string GenerateId()
        {
            string id = DateTime.Now.ToString("yyyyMMdd");
            string sql = "select top 1 ID  from IDEA_REPLY WHERE ID LIKE @id + '%' order by ID desc";
            
            IDEA_REPLY idea = DBManager<IDEA_REPLY>.ExecuteReader(sql, new { id = id }).FirstOrDefault();
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
        public override int Insert(string IDEA_ID, string REP_EMP_ID, string COMMENTS, DateTime INS_DATE)
        {
            string ID = this.GenerateId();
            var sql = "INSERT INTO IDEA_REPLY(ID,IDEA_ID,REP_EMP_ID,COMMENTS,INS_DATE) VALUES(@ID,@IDEA_ID,@REP_EMP_ID,@COMMENTS,@INS_DATE)";
            return DBManager<IDEA_REPLY>.Execute(sql, new {ID=ID, IDEA_ID = IDEA_ID, REP_EMP_ID = REP_EMP_ID, COMMENTS = COMMENTS, INS_DATE = INS_DATE });
        }
        public dynamic GetReplyByIdeaId(string ID)
        {
            var sql = "SELECT E.EMP_NAME,E.DEPARTMENT,R.COMMENTS,R.INS_DATE FROM IDEA_REPLY AS R INNER JOIN EMPLOYEE AS E ON R.REP_EMP_ID=E.EMP_ID WHERE IDEA_ID=@ID";
            return DBManager<dynamic>.ExecuteDynamic(sql, new { ID = ID });
        }
    }
}