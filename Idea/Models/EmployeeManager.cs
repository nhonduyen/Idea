using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class EmployeeManager:EMPLOYEE
    {
        private static EmployeeManager instance = new EmployeeManager();

        private EmployeeManager()
        {
        }
        public static EmployeeManager GetInstance()
        {
            return instance;
        }
        public List<EMPLOYEE> GetDepartment()
        {
            var sql = "SELECT DISTINCT(DEPARTMENT) FROM EMPLOYEE WHERE DEPARTMENT IS NOT NULL AND DEPARTMENT <> ''";
            return DBManager<EMPLOYEE>.ExecuteReader(sql);
        }

        public List<EMPLOYEE> GetDivision()
        {
            var sql = "SELECT DISTINCT(DIVISION) FROM EMPLOYEE WHERE DIVISION IS NOT NULL AND DIVISION <> ''";
            return DBManager<EMPLOYEE>.ExecuteReader(sql);
        }
        public List<EMPLOYEE> GetListEmail()
        {
            var sql = "SELECT EMAIL FROM EMPLOYEE WHERE ROLE >= 1";
            return DBManager<EMPLOYEE>.ExecuteReader(sql);
        }
        public int UpdateDivision(string EmpId, string Division)
        {
            var sql = "UPDATE EMPLOYEE SET DIVISION=@DIVISION WHERE EMP_ID=@EMP_ID";
            return DBManager<EMPLOYEE>.Execute(sql, new { DIVISION = Division, EMP_ID =EmpId});
        }
        public int UpdateDepartment(string EmpId, string DEPARTMENT)
        {
            var sql = "UPDATE EMPLOYEE SET DEPARTMENT=@DEPARTMENT WHERE EMP_ID=@EMP_ID";
            return DBManager<EMPLOYEE>.Execute(sql, new { DEPARTMENT = DEPARTMENT, EMP_ID = EmpId });
        }
        public string Encode(string value)
        {
            var hash = System.Security.Cryptography.SHA1.Create();
            var encoder = new System.Text.ASCIIEncoding();
            var combined = encoder.GetBytes(value ?? "");
            return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }
        public bool Login(string EMP_ID, string Password)
        {
            var sql = "SELECT EMP_ID,EMP_NAME,DIVISION,DEPARTMENT,ROLE FROM EMPLOYEE WHERE EMP_ID=@EMP_ID AND EMP_PW=@PASSWORD";
            EMPLOYEE employee = DBManager<EMPLOYEE>.ExecuteReader(sql, new { EMP_ID = EMP_ID, PASSWORD = Password }).FirstOrDefault();
            if (employee == null)
                return false;
            HttpContext.Current.Session["Username"] = employee.EMP_ID.Trim();
            HttpContext.Current.Session["Name"] = employee.EMP_NAME.Trim();
            HttpContext.Current.Session["Dept"] = employee.DEPARTMENT.Trim();
            HttpContext.Current.Session["Division"] = employee.DIVISION.Trim();
            HttpContext.Current.Session["Role"] = employee.ROLE;
            return true;
        }

        public int ChangePassword(string EmpId, string pass, string newpass)
        {
            var sql = "UPDATE EMPLOYEE SET EMP_PW=@NEWPASS WHERE EMP_ID=@EMP_ID AND EMP_PW=@PASS";
            return DBManager<EMPLOYEE>.Execute(sql, new { NEWPASS = newpass, EMP_ID = EmpId, PASS=pass });
        }
    }
}