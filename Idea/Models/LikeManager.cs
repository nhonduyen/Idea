using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class LikeManager:PRJ_LIKE
    {
        private static LikeManager instance = new LikeManager();

        public static LikeManager GetInstance()
        {
            return instance;
        }

        public string GenerateId()
        {
            string id = DateTime.Now.ToString("yyyyMMdd");
            string sql = "select top 1 ID  from PRJ_LIKE WHERE ID LIKE @id + '%' order by ID desc";

            PRJ_LIKE like = DBManager<PRJ_LIKE>.ExecuteReader(sql, new { id = id }).FirstOrDefault();
            if (like == null)
            {
                id = id + "0001";
            }
            else
            {
                string str = like.ID.Trim().Substring(8);
                string num = (Convert.ToInt32(str) + 1).ToString();
                for (int i = 0; i < str.Length - num.Length; i++)
                {
                    id += "0";
                }
                id += num.ToString();
            }
            return id;
        }
        public int Insert(string IDEA_ID, string EMP_ID, string EMP_NAME)
        {
            string ID = this.GenerateId();
            var sql = "INSERT INTO PRJ_LIKE(ID,IDEA_ID,EMP_ID,EMP_NAME,INS_DATE) VALUES(@ID,@IDEA_ID,@EMP_ID,@EMP_NAME,GETDATE())";
            return DBManager<PRJ_LIKE>.Execute(sql, new { ID = ID, IDEA_ID = IDEA_ID, EMP_ID = EMP_ID, EMP_NAME = EMP_NAME });
        }

        public int CountLike(string IDEA_ID)
        {
            var sql = "SELECT COUNT(1) AS CNT FROM PRJ_LIKE WHERE IDEA_ID=@IDEA_ID";
            return (int)DBManager<PRJ_LIKE>.ExecuteScalar(sql, new{IDEA_ID=IDEA_ID});
        }

        public List<PRJ_LIKE> GetLike(string IDEA_ID)
        {
            var sql = "SELECT EMP_NAME  FROM PRJ_LIKE WHERE IDEA_ID=@IDEA_ID";
            return DBManager<PRJ_LIKE>.ExecuteReader(sql, new { IDEA_ID = IDEA_ID });
        }

        public bool CheckLike(string EMP_ID, string IDEA_ID)
        {
            var sql = "SELECT ID  FROM PRJ_LIKE WHERE IDEA_ID=@IDEA_ID AND EMP_ID=@EMP_ID";
            var result = DBManager<PRJ_LIKE>.ExecuteReader(sql, new { EMP_ID = EMP_ID, IDEA_ID = IDEA_ID });
            if (result == null || result.Count == 0)
                return false;
            return true;
        }
    }
}