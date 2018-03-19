using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class IDEA
    {
        public string ID { get; set; }
        public string EMP_ID { get; set; }
        public string EMP_NAME { get; set; }
        public string IDEA_TITLE { get; set; }
        public string DETAIL { get; set; }
        public string QUANTITATIVE { get; set; }
        public string QUALITATIVE { get; set; }
        public DateTime DATE { get; set; }

        public IDEA() { }
        public IDEA(string ID, string EMP_ID, string EMP_NAME, string IDEA_TITLE, string DETAIL, string QUANTITATIVE, string QUALITATIVE)
        {
            this.ID = ID;
            this.EMP_ID = EMP_ID;
            this.IDEA_TITLE = IDEA_TITLE;
            this.DETAIL = DETAIL;
            this.QUANTITATIVE = QUANTITATIVE;
            this.QUALITATIVE = QUALITATIVE;
            this.EMP_NAME = EMP_NAME;
        }

        public virtual List<IDEA> Select(string ID = "")
        {
            var sql = "SELECT * FROM IDEA ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<IDEA>.ExecuteReader(sql);
            sql += " WHERE ID=@ID";

            return DBManager<IDEA>.ExecuteReader(sql, new { ID = ID });
        }

        public virtual dynamic SelectPaging(int start = 0, int end = 10, string div = "", string dep = "")
        {
            var sql = string.Format(@"
SELECT * FROM(SELECT ROW_NUMBER() OVER (order by id desc) AS ROWNUM, I.*, E.EMP_NAME,E.DIVISION,E.DEPARTMENT,
(SELECT COUNT(1) FROM IDEA_REPLY WHERE IDEA_ID=I.ID) AS REP,
(SELECT COUNT(1) FROM IDEA_LIKE WHERE IDEA_ID=I.ID) AS L 
FROM IDEA AS I INNER JOIN EMPLOYEE AS E ON I.EMP_ID=E.EMP_ID
WHERE NOT EXISTS(SELECT IDEA_ID FROM PROJECT WHERE IDEA_ID=I.ID) AND(@div='' OR DIVISION LIKE '%'+@div+'%') AND(@dep='' OR DEPARTMENT LIKE '%'+@dep+'%'))
as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;");

            return DBManager<IDEA>.ExecuteDynamic(sql, new
            {
                start = start,
                end = end,
                div = div,
                dep = dep
            });
        }

        public virtual int GetCount(string div = "", string dep = "")
        {
            var sql = "SELECT COUNT(1) AS CNT FROM IDEA AS I INNER JOIN EMPLOYEE as E ON E.EMP_ID=I.EMP_ID WHERE NOT EXISTS(SELECT IDEA_ID FROM PROJECT WHERE IDEA_ID=ID) ";
            if (string.IsNullOrWhiteSpace(div) && string.IsNullOrWhiteSpace(dep))
                return (int)DBManager<IDEA>.ExecuteScalar(sql);
            sql += "AND(@div='' OR DIVISION LIKE '%'+@div+'%') AND(@dep='' OR DEPARTMENT LIKE '%'+@dep+'%')";
            return (int) DBManager<IDEA>.ExecuteScalar(sql, new
            {
              
                div = div,
                dep = dep
            });
        }

        public virtual int Insert(string EMP_ID, string IDEA_TITLE, string DETAIL, string QUANTITATIVE, string QUALITATIVE)
        {
            var sql = "INSERT INTO IDEA(EMP_ID,IDEA_TITLE,DETAIL,QUANTITATIVE,QUALITATIVE,DATE) VALUES(@EMP_ID,@IDEA_TITLE,@DETAIL,@QUANTITATIVE,@QUALITATIVE,GETDATE())";
            return DBManager<IDEA>.Execute(sql, new { EMP_ID = EMP_ID, IDEA_TITLE = IDEA_TITLE, DETAIL = DETAIL, QUANTITATIVE = QUANTITATIVE, QUALITATIVE = QUALITATIVE });
        }

        public virtual int Update(string ID, string EMP_ID, string IDEA_TITLE, string DETAIL, string QUANTITATIVE, string QUALITATIVE)
        {
            var sql = "UPDATE IDEA SET EMP_ID=@EMP_ID,IDEA_TITLE=@IDEA_TITLE,DETAIL=@DETAIL,QUANTITATIVE=@QUANTITATIVE,QUALITATIVE=@QUALITATIVE WHERE ID=@ID";

            return DBManager<IDEA>.Execute(sql, new { ID = ID, EMP_ID = EMP_ID, IDEA_TITLE = IDEA_TITLE, DETAIL = DETAIL, QUANTITATIVE = QUANTITATIVE, QUALITATIVE = QUALITATIVE });
        }

        public virtual int Delete(string ID = "")
        {
            var sql = "DELETE FROM IDEA ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<IDEA>.Execute(sql);
            sql += " WHERE ID=@ID ";
            return DBManager<IDEA>.Execute(sql, new { ID = ID });
        }


    }

}
