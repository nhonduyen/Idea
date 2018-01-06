using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class PROJECT
    {
        public string IDEA_ID { get; set; }
        public string EMP_ID { get; set; }
        public string NAME { get; set; }
        public string IDEA_TITLE { get; set; }
        public string PRJECT_GRADE { get; set; }
        public string KPI_NAME { get; set; }
        public string KPI_UNIT { get; set; }
        public string BACKGROUND { get; set; }
        public string REMARK { get; set; }
        public DateTime INS_DT { get; set; }

        public PROJECT(string IDEA_ID, string EMP_ID, string IDEA_TITLE, string PRJECT_GRADE, string KPI_NAME, string KPI_UNIT, string BACKGROUND)
        {
            this.IDEA_ID = IDEA_ID;
            this.EMP_ID = EMP_ID;
            this.IDEA_TITLE = IDEA_TITLE;
            this.PRJECT_GRADE = PRJECT_GRADE;
            this.KPI_NAME = KPI_NAME;
            this.KPI_UNIT = KPI_UNIT;
            this.BACKGROUND = BACKGROUND;
        }
        public PROJECT() { }

        public virtual List<PROJECT> Select(string ID="")
        {
            var sql = "SELECT * FROM PROJECT ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<PROJECT>.ExecuteReader(sql);
            sql +=" WHERE ID=@ID";

            return DBManager<PROJECT>.ExecuteReader(sql, new { ID = ID});
        }

        public virtual List<PROJECT> SelectPaging(int start=0, int end=10)
        {
            var sql = string.Format(@"SELECT * FROM(SELECT ROW_NUMBER() OVER (order by IDEA_ID desc) AS ROWNUM, IDEA_ID,IDEA_TITLE,NAME,
INS_DT FROM PROJECT AS P WHERE NOT EXISTS(SELECT IDEA_ID FROM PROJECT_PLAN AS PL WHERE P.IDEA_ID=PL.IDEA_ID AND COMPLETE_YN=1)) as u 
WHERE RowNum >= @start   AND RowNum < @end ORDER BY RowNum;");

            return DBManager<PROJECT>.ExecuteReader(sql, new { start=start, end = end});
        }

        public virtual int GetCount(int main=0)
        {
            var sql = "SELECT COUNT(1) AS CNT FROM PROJECT AS P WHERE NOT EXISTS(SELECT IDEA_ID FROM PROJECT_PLAN AS PL WHERE P.IDEA_ID=PL.IDEA_ID AND COMPLETE_YN=1)";
            if (main == 1)
                sql = sql.Replace("NOT", "");
            return (int)DBManager<PROJECT>.ExecuteScalar(sql);
        }

        public virtual int Insert(string IDEA_ID,string EMP_ID,string IDEA_TITLE,string PRJECT_GRADE,string KPI_NAME,string KPI_UNIT,string BACKGROUND, string NAME)
        {
            var sql = "INSERT INTO PROJECT(IDEA_ID,EMP_ID,IDEA_TITLE,PRJECT_GRADE,KPI_NAME,KPI_UNIT,BACKGROUND,NAME,INS_DT) VALUES(@IDEA_ID,@EMP_ID,@IDEA_TITLE,@PRJECT_GRADE,@KPI_NAME,@KPI_UNIT,@BACKGROUND,@NAME,GETDATE())";
            return DBManager<PROJECT>.Execute(sql, new { IDEA_ID = IDEA_ID,EMP_ID = EMP_ID,IDEA_TITLE = IDEA_TITLE,PRJECT_GRADE = PRJECT_GRADE,KPI_NAME = KPI_NAME,KPI_UNIT = KPI_UNIT,BACKGROUND = BACKGROUND, NAME=NAME});
        }

        public virtual int Update(string IDEA_ID, string EMP_ID, string IDEA_TITLE, string PRJECT_GRADE, string KPI_NAME, string KPI_UNIT, string BACKGROUND)
        {
            var sql = "UPDATE PROJECT SET IDEA_ID=@IDEA_ID,EMP_ID=@EMP_ID,IDEA_TITLE=@IDEA_TITLE,PRJECT_GRADE=@PRJECT_GRADE,KPI_NAME=@KPI_NAME,KPI_UNIT=@KPI_UNIT,BACKGROUND=@BACKGROUND WHERE ID=@ID";

            return DBManager<PROJECT>.Execute(sql,  new { IDEA_ID = IDEA_ID,EMP_ID = EMP_ID,IDEA_TITLE = IDEA_TITLE,PRJECT_GRADE = PRJECT_GRADE,KPI_NAME = KPI_NAME,KPI_UNIT = KPI_UNIT,BACKGROUND = BACKGROUND});
        }

        public virtual int Delete(int ID=0)
        {
            var sql = "DELETE FROM PROJECT ";
            if (ID == 0) return DBManager<PROJECT>.Execute(sql);
            sql += " WHERE ID=@ID ";
            return DBManager<PROJECT>.Execute(sql, new { ID = ID });
        }


    }

}
