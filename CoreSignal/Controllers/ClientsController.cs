﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreSignal.signalr;
using Data.Common;
using Data.Model;

namespace CoreSiganl.Controllers
{
    [Route("api/[controller]")]
    public class ClientsController : Controller
    {
        // GET api/Clients
        [HttpGet]
        public List<Pf_MessageStatus_Obj> Get()
        {
            //return JsonHelper.SerializeObject(MessageHub.messageContextList);
            var value = MessageHub.messageContextList;
            return value;
        }

        // GET api/sessions/idindex
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
