using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlTypes;
using Dapper;
using System.Data;

namespace Idea
{
    public class PlanManager : PROJECT_PLAN
    {
        private static PlanManager instance = new PlanManager();

        public static PlanManager GetInstance()
        {
            return instance;
        }
        public string GenerateId()
        {
            string id = DateTime.Now.ToString("yyMMdd");
            string sql = "select top 1 ID  from PROJECT_PLAN WHERE ID LIKE @id + '%' order by ID desc";

            PROJECT_PLAN idea = DBManager<PROJECT_PLAN>.ExecuteReader(sql, new { id = id }).FirstOrDefault();
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
        public int InsertPlan(string IDEA_ID, string PLAN_CONTENTS, DateTime PLAN_DATE)
        {
            var ID = GenerateId();
            var sql = "INSERT INTO PROJECT_PLAN(ID,IDEA_ID,PLAN_CONTENTS,PLAN_DATE) VALUES(@ID,@IDEA_ID,@PLAN_CONTENTS,@PLAN_DATE)";
            if (PLAN_DATE == DateTime.MinValue)
            {
                return DBManager<PROJECT_PLAN>.Execute(sql, new { ID = ID, IDEA_ID = IDEA_ID, PLAN_CONTENTS = PLAN_CONTENTS, PLAN_DATE = DBNull.Value });
            }
            return DBManager<PROJECT_PLAN>.Execute(sql, new { ID = ID, IDEA_ID = IDEA_ID, PLAN_CONTENTS = PLAN_CONTENTS, PLAN_DATE = PLAN_DATE });
        }
        public int InsertPlan(string IDEA_ID, string PLAN_CONTENTS, DateTime PLAN_DATE, int COMPLETE_YN, DateTime COMPLETE_DATE)
        {
            var ID = GenerateId();
            var sql = "INSERT INTO PROJECT_PLAN(ID,IDEA_ID,PLAN_CONTENTS,PLAN_DATE,COMPLETE_YN, COMPLETE_DATE) VALUES(@ID,@IDEA_ID,@PLAN_CONTENTS,@PLAN_DATE,@COMPLETE_YN, @COMPLETE_DATE)";
          
            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@ID", ID, DbType.String, ParameterDirection.Input);
            parameter.Add("@IDEA_ID", IDEA_ID, DbType.String, ParameterDirection.Input);
            parameter.Add("@PLAN_CONTENTS", PLAN_CONTENTS, DbType.String, ParameterDirection.Input);
            if (PLAN_DATE == DateTime.MinValue)
            {
                parameter.Add("@PLAN_DATE", DBNull.Value, DbType.Date, ParameterDirection.Input);
            }
            else
            {
                parameter.Add("@PLAN_DATE", PLAN_DATE, DbType.Date, ParameterDirection.Input);
            }
            parameter.Add("@COMPLETE_YN", COMPLETE_YN, DbType.Int32, ParameterDirection.Input);
            if (COMPLETE_DATE == DateTime.MinValue)
            {
                parameter.Add("@COMPLETE_DATE", DBNull.Value, DbType.Date, ParameterDirection.Input);
            }
            else
            {
                parameter.Add("@COMPLETE_DATE", COMPLETE_DATE, DbType.Date, ParameterDirection.Input);
            }

            return DBManager<PROJECT_PLAN>.Execute(sql,parameter);
        }

        public int DeletePlanByIdeaId(string ID)
        {
            var sql = "DELETE FROM PROJECT_PLAN WHERE IDEA_ID=@ID";
            return DBManager<PROJECT_PLAN>.Execute(sql, new { ID = ID });
        }
    }
}