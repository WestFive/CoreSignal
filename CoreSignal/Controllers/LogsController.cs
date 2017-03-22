using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data.Common;

namespace CoreSignal.Controllers
{
    [Produces("application/json")]
    [Route("api/Logs")]
    public class LogsController : Controller
    {

        // GET: api/Logs
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return Loger.ReadFromLogTxt(DateTime.Now, 0);
        }

        [HttpGet("{days}")]
        public IEnumerable<string> Get(int days)
        {
            Loger.FilePath = "wwwroot/Log";
            return Loger.ReadFromLogTxt(DateTime.Now, days);


        }

        //// GET: api/Logs/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Logs
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/Logs/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
