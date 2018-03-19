using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data;

namespace Idea
{
    public class PROJECT_PLAN
    {
        public string IDEA_ID { get; set; }
        public string PLAN_CONTENTS { get; set; }
        public DateTime PLAN_DATE { get; set; }
        public int COMPLETE_YN { get; set; }
        public DateTime COMPLETE_DATE { get; set; }
        public string ID { get; set; }

        public PROJECT_PLAN(string IDEA_ID, string PLAN_CONTENTS, DateTime PLAN_DATE, int COMPLETE_YN, DateTime COMPLETE_DATE, string ID)
        {
            this.IDEA_ID = IDEA_ID;
            this.PLAN_CONTENTS = PLAN_CONTENTS;
            this.PLAN_DATE = PLAN_DATE;
            this.COMPLETE_YN = COMPLETE_YN;
            this.COMPLETE_DATE = COMPLETE_DATE;
            this.ID = ID;
        }

        public PROJECT_PLAN() { }

        public virtual List<PROJECT_PLAN> Select(string ID="")
        {
            var sql = "SELECT * FROM PROJECT_PLAN ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<PROJECT_PLAN>.ExecuteReader(sql);
            sql += " WHERE IDEA_ID=@ID ORDER BY PLAN_DATE DESC";

            return DBManager<PROJECT_PLAN>.ExecuteReader(sql, new { ID = ID});
        }

        public virtual List<PROJECT_PLAN> SelectPaging(int start=0, int end=10)
        {
            var sql = "SELECT * FROM(SELECT ROW_NUMBER() OVER (order by id) AS ROWNUM, * FROM PROJECT_PLAN) as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;";

            return DBManager<PROJECT_PLAN>.ExecuteReader(sql, new { start=start, end = end});
        }

        public virtual int GetCount()
        {
            var sql = "SELECT COUNT(1) AS CNT FROM PROJECT_PLAN;";
            return (int)DBManager<PROJECT_PLAN>.ExecuteScalar(sql);
        }

        public virtual int Insert(string IDEA_ID,string PLAN_CONTENTS,DateTime PLAN_DATE,int COMPLETE_YN,DateTime COMPLETE_DATE)
        {
            var sql = "INSERT INTO PROJECT_PLAN(IDEA_ID,PLAN_CONTENTS,PLAN_DATE,COMPLETE_YN,COMPLETE_DATE) VALUES(@IDEA_ID,@PLAN_CONTENTS,@PLAN_DATE,@COMPLETE_YN,@COMPLETE_DATE)";
            return DBManager<PROJECT_PLAN>.Execute(sql, new { IDEA_ID = IDEA_ID,PLAN_CONTENTS = PLAN_CONTENTS,PLAN_DATE = PLAN_DATE,COMPLETE_YN = COMPLETE_YN,COMPLETE_DATE = COMPLETE_DATE});
        }

        public virtual int Update(string IDEA_ID, string PLAN_CONTENTS, DateTime PLAN_DATE, int COMPLETE_YN, DateTime COMPLETE_DATE, string ID)
        {
            var sql = "UPDATE PROJECT_PLAN SET IDEA_ID=@IDEA_ID,PLAN_CONTENTS=@PLAN_CONTENTS,PLAN_DATE=@PLAN_DATE,COMPLETE_YN=@COMPLETE_YN,COMPLETE_DATE=@COMPLETE_DATE WHERE ID=@ID";
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
            return DBManager<PROJECT_PLAN>.Execute(sql, parameter);
        }

        public virtual int Delete(string ID="")
        {
            var sql = "DELETE FROM PROJECT_PLAN ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<PROJECT_PLAN>.Execute(sql);
            sql += " WHERE ID=@ID ";
            return DBManager<PROJECT_PLAN>.Execute(sql, new { ID = ID });
        }


    }

}
