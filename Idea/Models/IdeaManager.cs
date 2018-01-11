using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class IdeaManager:IDEA
    {
        private static IdeaManager instance = new IdeaManager();

        public static IdeaManager GetInstance()
        {
            return instance;
        }

        public string GenerateId()
        {
            string id = DateTime.Now.ToString("yyMMdd");
            string sql = "select top 1 ID  from IDEA WHERE ID LIKE @id + '%' order by ID desc";

            IDEA idea = DBManager<IDEA>.ExecuteReader(sql, new { id = id }).FirstOrDefault();
            if (idea == null)
            {
                id = id + "0001";
            }
            else
            {
                string str = idea.ID.Trim().Substring(6);
                string num = (Convert.ToInt32(str) + 1).ToString();
                for (int i = 0; i < str.Length - num.Length; i++)
                {
                    id += "0";
                }
                id += num.ToString();
            }
            return id;
        }
        public dynamic GetIdea(string ID)
        {
            var sql = "SELECT I.*, E.DEPARTMENT,E.DIVISION,E.EMP_NAME FROM IDEA AS I INNER JOIN EMPLOYEE AS E ON I.EMP_ID=E.EMP_ID WHERE ID=@ID";
            return DBManager<dynamic>.ExecuteReader(sql, new { ID = ID }).FirstOrDefault();
        }
        public override int Insert(string EMP_ID, string IDEA_TITLE, string DETAIL, string QUANTITATIVE, string QUALITATIVE)
        {
            var ID = this.GenerateId();
            var sql = "INSERT INTO IDEA(ID,EMP_ID,IDEA_TITLE,DETAIL,QUANTITATIVE,QUALITATIVE,DATE) VALUES(@ID,@EMP_ID,@IDEA_TITLE,@DETAIL,@QUANTITATIVE,@QUALITATIVE,GETDATE())";
            return DBManager<IDEA>.Execute(sql, new {ID=ID, EMP_ID = EMP_ID, IDEA_TITLE = IDEA_TITLE, DETAIL = DETAIL, QUANTITATIVE = QUANTITATIVE, QUALITATIVE = QUALITATIVE });
        }
    }
}