using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class IDEA_REPLY
    {
        public string IDEA_ID { get; set; }
        public string REP_EMP_ID { get; set; }
      
        public string COMMENTS { get; set; }
        public DateTime INS_DATE { get; set; }
      
        public IDEA_REPLY(string IDEA_ID, string REP_EMP_ID, string REP_EMP_NAME, string COMMENTS, DateTime INS_DATE, string ID)
        {
            this.IDEA_ID = IDEA_ID;
            this.REP_EMP_ID = REP_EMP_ID;
          
            this.COMMENTS = COMMENTS;
            this.INS_DATE = INS_DATE;
           
        }

        public IDEA_REPLY() { }

        public virtual List<IDEA_REPLY> Select(string ID="")
        {
            var sql = "SELECT * FROM IDEA_REPLY ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<IDEA_REPLY>.ExecuteReader(sql);
            sql +=" WHERE ID=@ID";

            return DBManager<IDEA_REPLY>.ExecuteReader(sql, new { ID = ID});
        }

        public virtual List<IDEA_REPLY> SelectPaging(int start=0, int end=10)
        {
            var sql = "SELECT * FROM(SELECT ROW_NUMBER() OVER (order by id) AS ROWNUM, * FROM IDEA_REPLY) as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;";

            return DBManager<IDEA_REPLY>.ExecuteReader(sql, new { start=start, end = end});
        }

        public virtual int GetCount()
        {
            var sql = "SELECT COUNT(1) AS CNT FROM IDEA_REPLY;";
            return (int)DBManager<IDEA_REPLY>.ExecuteScalar(sql);
        }

        public virtual int Insert(string IDEA_ID,string REP_EMP_ID,string COMMENTS,DateTime INS_DATE)
        {
            var sql = "INSERT INTO IDEA_REPLY(IDEA_ID,REP_EMP_ID,COMMENTS,INS_DATE) VALUES(@IDEA_ID,@REP_EMP_ID,@COMMENTS,@INS_DATE)";
            return DBManager<IDEA_REPLY>.Execute(sql, new { IDEA_ID = IDEA_ID,REP_EMP_ID = REP_EMP_ID,COMMENTS = COMMENTS,INS_DATE = INS_DATE});
        }

        public virtual int Update(string IDEA_ID, string REP_EMP_ID, string COMMENTS, DateTime INS_DATE, string ID)
        {
            var sql = "UPDATE IDEA_REPLY SET IDEA_ID=@IDEA_ID,REP_EMP_ID=@REP_EMP_ID,COMMENTS=@COMMENTS,INS_DATE=@INS_DATE WHERE ID=@ID";

            return DBManager<IDEA_REPLY>.Execute(sql,  new { IDEA_ID = IDEA_ID,REP_EMP_ID = REP_EMP_ID,COMMENTS = COMMENTS,INS_DATE = INS_DATE,ID = ID});
        }

        public virtual int Delete(int ID=0)
        {
            var sql = "DELETE FROM IDEA_REPLY ";
            if (ID == 0) return DBManager<IDEA_REPLY>.Execute(sql);
            sql += " WHERE ID=@ID ";
            return DBManager<IDEA_REPLY>.Execute(sql, new { ID = ID });
        }


    }

}
