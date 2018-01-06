using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class KPI
    {
        public string IDEA_ID { get; set; }
        public string PRJ_MONTH { get; set; }
        public int TARGET_VALUE { get; set; }
        public int RESULT_VALUE { get; set; }
        public string ID { get; set; }

        public KPI(string IDEA_ID, string PRJ_MONTH, int TARGET_VALUE, int RESULT_VALUE, string ID)
        {
            this.IDEA_ID = IDEA_ID;
            this.PRJ_MONTH = PRJ_MONTH;
            this.TARGET_VALUE = TARGET_VALUE;
            this.RESULT_VALUE = RESULT_VALUE;
            this.ID = ID;
        }
        public KPI() { }

        public virtual List<KPI> Select(string ID="")
        {
            var sql = "SELECT * FROM KPI ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<KPI>.ExecuteReader(sql);
            sql += " WHERE IDEA_ID=@ID";

            return DBManager<KPI>.ExecuteReader(sql, new { ID = ID });
        }

        public virtual List<KPI> SelectPaging(int start=0, int end=10)
        {
            var sql = "SELECT * FROM(SELECT ROW_NUMBER() OVER (order by id) AS ROWNUM, * FROM KPI) as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;";

            return DBManager<KPI>.ExecuteReader(sql, new { start=start, end = end});
        }

        public virtual int GetCount()
        {
            var sql = "SELECT COUNT(1) AS CNT FROM KPI;";
            return (int) DBManager<KPI>.ExecuteScalar(sql);
        }

        public virtual int Insert(string IDEA_ID,string PRJ_MONTH,int TARGET_VALUE,int RESULT_VALUE)
        {
            var sql = "INSERT INTO KPI(IDEA_ID,PRJ_MONTH,TARGET_VALUE,RESULT_VALUE) VALUES(@IDEA_ID,@PRJ_MONTH,@TARGET_VALUE,@RESULT_VALUE)";
            return DBManager<KPI>.Execute(sql, new { IDEA_ID = IDEA_ID,PRJ_MONTH = PRJ_MONTH,TARGET_VALUE = TARGET_VALUE,RESULT_VALUE = RESULT_VALUE});
        }

        public virtual int Update(string IDEA_ID, string PRJ_MONTH, int TARGET_VALUE, int RESULT_VALUE, string ID)
        {
            var sql = "UPDATE KPI SET IDEA_ID=@IDEA_ID,PRJ_MONTH=@PRJ_MONTH,TARGET_VALUE=@TARGET_VALUE,RESULT_VALUE=@RESULT_VALUE WHERE ID=@ID";

            return DBManager<KPI>.Execute(sql,  new { IDEA_ID = IDEA_ID,PRJ_MONTH = PRJ_MONTH,TARGET_VALUE = TARGET_VALUE,RESULT_VALUE = RESULT_VALUE,ID = ID});
        }

        public virtual int Delete(int ID=0)
        {
            var sql = "DELETE FROM KPI ";
            if (ID == 0) return DBManager<KPI>.Execute(sql);
            sql += " WHERE ID=@ID ";
            return DBManager<KPI>.Execute(sql, new { ID = ID });
        }


    }

}
