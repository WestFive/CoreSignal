using System;
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

        [HttpPost("{LaneJson}")]
        public string Post(string LaneJson)
        {
            
            try
            {
                var temp = JsonHelper.DeserializeJsonToObject<Pf_MessageStatus_Obj>(LaneJson);
                lock (MessageHub.messageContextList)
                {
                    if (MessageHub.messageContextList.Count(x => x.message_content.lane_id == temp.message_content.lane_id) > 0)
                    {
                        //var temptt = messageContextList.FirstOrDefault(x => x.message_content.LaneID == temp.message_content.LaneID);

                        MessageHub.messageContextList[MessageHub.messageContextList.FindIndex(x => x.message_content.lane_id == temp.message_content.lane_id)] = temp;

                    }
                }

                Loger.AddLogText(DateTime.Now.ToString() + "修改:" + temp.message_content.lane_name + "数据成功");
                return "修改成功";
            }

                
                
            catch (Exception ex)
            {
                Loger.AddErrorText("API修改车道数据失败", ex);
                return "修改失败";
                
            }
        }

        //// GET api/sessions/idindex
        //[HttpGet("{id}")]
        //public List<object> Get(int id)
        //{
        //    switch (id)
        //    {
        //        case 1:
        //            List<object> valuelist = new List<object>();
        //            foreach (var item in MessageHub.messageContextList)
        //            {
        //                valuelist.Add(item.message_content);
        //            }
        //            return valuelist;
        //        case 2:
        //            List<object> valuelist2 = new List<object>();
        //            foreach (var item in MessageHub.messageContextList)
        //            {
        //                valuelist2.Add(item.message_content.lane_status);
        //            }
        //            return valuelist2;
        //        case 3:
        //            List<object> valuelist3 = new List<object>();
        //            foreach (var item in MessageHub.messageContextList)
        //            {
        //                valuelist3.Add(item.message_content.lane_status.progress_bar);
        //            }
        //            return valuelist3;
        //        default:
        //            List<object> valuelist0 = new List<object>();
        //            foreach (var item in MessageHub.messageContextList)
        //            {
        //                valuelist0.Add(item.message_content);
        //            }
        //            return valuelist0;



        //    }


        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{

        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
