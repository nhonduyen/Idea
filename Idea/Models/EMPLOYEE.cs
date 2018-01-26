using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class EMPLOYEE
    {
        public string EMP_ID { get; set; }
        public string EMP_PW { get; set; }
        public string EMP_NAME { get; set; }
        public string EMP_NO { get; set; }
        public string DIVISION { get; set; }
        public string DEPARTMENT { get; set; }
        public string EMAIL { get; set; }
        public int ROLE { get; set; }

        public EMPLOYEE(string EMP_ID, string EMP_PW, string EMP_NAME, string EMP_NO, string DIVISION, string DEPARTMENT)
        {
            this.EMP_ID = EMP_ID;
            this.EMP_PW = EMP_PW;
            this.EMP_NAME = EMP_NAME;
            this.EMP_NO = EMP_NO;
            this.DIVISION = DIVISION;
            this.DEPARTMENT = DEPARTMENT;
        }
        public EMPLOYEE() { }

        public virtual List<EMPLOYEE> Select(string ID="")
        {
            var sql = "SELECT * FROM EMPLOYEE ";
            if (ID == "") return DBManager<EMPLOYEE>.ExecuteReader(sql);
            sql +=" WHERE EMP_ID=@ID";

            return DBManager<EMPLOYEE>.ExecuteReader(sql, new { ID = ID});
        }

        public virtual List<EMPLOYEE> SelectPaging(int start=0, int end=10)
        {
            var sql = "SELECT * FROM(SELECT ROW_NUMBER() OVER (order by id) AS ROWNUM, * FROM EMPLOYEE) as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;";

            return DBManager<EMPLOYEE>.ExecuteReader(sql, new { start=start, end = end});
        }

        public virtual int GetCount()
        {
            var sql = "SELECT COUNT(1) AS CNT FROM EMPLOYEE;";
            return (int) DBManager<EMPLOYEE>.ExecuteScalar(sql);
        }

        public virtual int Insert(string EMP_ID,string EMP_PW,string EMP_NAME,string EMP_NO,string DIVISION,string DEPARTMENT)
        {
            var sql = "INSERT INTO EMPLOYEE(EMP_ID,EMP_PW,EMP_NAME,EMP_NO,DIVISION,DEPARTMENT) VALUES(@EMP_ID,@EMP_PW,@EMP_NAME,@EMP_NO,@DIVISION,@DEPARTMENT)";
            return DBManager<EMPLOYEE>.Execute(sql, new { EMP_ID = EMP_ID,EMP_PW = EMP_PW,EMP_NAME = EMP_NAME,EMP_NO = EMP_NO,DIVISION = DIVISION,DEPARTMENT = DEPARTMENT});
        }

        public virtual int Update(string EMP_ID, string EMP_PW, string EMP_NAME, string EMP_NO, string DIVISION, string DEPARTMENT)
        {
            var sql = "UPDATE EMPLOYEE SET EMP_ID=@EMP_ID,EMP_PW=@EMP_PW,EMP_NAME=@EMP_NAME,EMP_NO=@EMP_NO,DIVISION=@DIVISION,DEPARTMENT=@DEPARTMENT WHERE ID=@ID";

            return DBManager<EMPLOYEE>.Execute(sql,  new { EMP_ID = EMP_ID,EMP_PW = EMP_PW,EMP_NAME = EMP_NAME,EMP_NO = EMP_NO,DIVISION = DIVISION,DEPARTMENT = DEPARTMENT});
        }

        public virtual int Delete(int ID=0)
        {
            var sql = "DELETE FROM EMPLOYEE ";
            if (ID == 0) return DBManager<EMPLOYEE>.Execute(sql);
            sql += " WHERE ID=@ID ";
            return DBManager<EMPLOYEE>.Execute(sql, new { ID = ID });
        }
    }

}
