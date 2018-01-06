using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Idea.Controllers
{
    public class AjaxController : ApiController
    {
        public bool Login([FromBody] EMPLOYEE emp)
        {
            EmployeeManager manager = EmployeeManager.GetInstance();
            return manager.Login(emp.EMP_ID, emp.EMP_PW);
        }
        // GET api/ajax
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/ajax/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/ajax
        public void Post([FromBody]string value)
        {
        }

        // PUT api/ajax/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/ajax/5
        public void Delete(int id)
        {
        }
    }
}
