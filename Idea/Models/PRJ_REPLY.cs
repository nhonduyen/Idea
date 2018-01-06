using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class PRJ_REPLY
    {
        public string IDEA_ID { get; set; }
        public string REP_EMP_ID { get; set; }
        public string REP_EMP_NAME { get; set; }
        public string COMMENTS { get; set; }
        public DateTime INS_DATE { get; set; }
        public string ID { get; set; }

        public PRJ_REPLY(string IDEA_ID, string REP_EMP_ID, string REP_EMP_NAME, string COMMENTS, DateTime INS_DATE, string ID)
        {
            this.IDEA_ID = IDEA_ID;
            this.REP_EMP_ID = REP_EMP_ID;
            this.REP_EMP_NAME = REP_EMP_NAME;
            this.COMMENTS = COMMENTS;
            this.INS_DATE = INS_DATE;
            this.ID = ID;
        }

        public PRJ_REPLY() { }

        public virtual List<PRJ_REPLY> Select(string ID="")
        {
            var sql = "SELECT * FROM PRJ_REPLY ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<PRJ_REPLY>.ExecuteReader(sql);
            sql += " WHERE IDEA_ID=@ID";

            return DBManager<PRJ_REPLY>.ExecuteReader(sql, new { ID = ID});
        }

        public virtual List<PRJ_REPLY> SelectPaging(int start=0, int end=10)
        {
            var sql = "SELECT * FROM(SELECT ROW_NUMBER() OVER (order by id) AS ROWNUM, * FROM PRJ_REPLY) as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;";

            return DBManager<PRJ_REPLY>.ExecuteReader(sql, new { start=start, end = end});
        }

        public virtual int GetCount()
        {
            var sql = "SELECT COUNT(1) AS CNT FROM PRJ_REPLY;";
            return (int)DBManager<PRJ_REPLY>.ExecuteScalar(sql);
        }

        public virtual int Insert(string IDEA_ID,string REP_EMP_ID,string REP_EMP_NAME,string COMMENTS,DateTime INS_DATE)
        {
            var sql = "INSERT INTO PRJ_REPLY(IDEA_ID,REP_EMP_ID,REP_EMP_NAME,COMMENTS,INS_DATE) VALUES(@IDEA_ID,@REP_EMP_ID,@REP_EMP_NAME,@COMMENTS,@INS_DATE)";
            return DBManager<PRJ_REPLY>.Execute(sql, new { IDEA_ID = IDEA_ID,REP_EMP_ID = REP_EMP_ID,REP_EMP_NAME = REP_EMP_NAME,COMMENTS = COMMENTS,INS_DATE = INS_DATE});
        }

        public virtual int Update(string IDEA_ID, string REP_EMP_ID, string REP_EMP_NAME, string COMMENTS, DateTime INS_DATE, string ID)
        {
            var sql = "UPDATE PRJ_REPLY SET IDEA_ID=@IDEA_ID,REP_EMP_ID=@REP_EMP_ID,REP_EMP_NAME=@REP_EMP_NAME,COMMENTS=@COMMENTS,INS_DATE=@INS_DATE WHERE ID=@ID";

            return DBManager<PRJ_REPLY>.Execute(sql,  new { IDEA_ID = IDEA_ID,REP_EMP_ID = REP_EMP_ID,REP_EMP_NAME = REP_EMP_NAME,COMMENTS = COMMENTS,INS_DATE = INS_DATE,ID = ID});
        }

        public virtual int Delete(int ID=0)
        {
            var sql = "DELETE FROM PRJ_REPLY ";
            if (ID == 0) return DBManager<PRJ_REPLY>.Execute(sql);
            sql += " WHERE ID=@ID ";
            return DBManager<PRJ_REPLY>.Execute(sql, new { ID = ID });
        }


    }

}
