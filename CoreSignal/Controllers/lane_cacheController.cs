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
    public class lane_cacheController : Controller
    {
      

        // GET api/Clients
        [HttpGet]
        public List<Pf_MessageStatus_Obj> Get()
        {
            //return JsonHelper.SerializeObject(MessageHub.messageContextList);
            var value = MessageHub.messageContextList;
            return value;
        }

        [HttpPost("{message_status}")]
        public string Post([FromBody]Pf_MessageStatus_Obj message_status)
        {
            
            try
            {
                //var temp = JsonHelper.DeserializeJsonToObject<Pf_MessageStatus_Obj>(LaneJson);
                //lock (MessageHub.messageContextList)
                //{
                //    if (MessageHub.messageContextList.Count(x => x.message_content.lane_id == temp.message_content.lane_id) > 0)
                //    {
                //        //var temptt = messageContextList.FirstOrDefault(x => x.message_content.LaneID == temp.message_content.LaneID);

                //        MessageHub.messageContextList[MessageHub.messageContextList.FindIndex(x => x.message_content.lane_id == temp.message_content.lane_id)] = temp;

                //    }
                //}
               lock(MessageHub.messageContextList)
                {
                    if(MessageHub.messageContextList.Count(x=>x.message_content.lane_id==message_status.message_content.lane_id)>0)
                    {
                        MessageHub.messageContextList[MessageHub.messageContextList.FindIndex(x => x.message_content.lane_id == message_status.message_content.lane_id)] = message_status;
                    }
                }

                Loger.AddLogText(DateTime.Now.ToString() + "修改:" + message_status.message_content.lane_name + "数据成功");
                return "修改成功";
            }

                
                
            catch (Exception ex)
            {
                Loger.AddErrorText("API修改车道数据失败", ex);
                return "修改失败"+ex.ToString();
                
            }
        }

        [HttpPost("{message_status_json}")]
        public string Post(string message_status_json)
        {
            try
            {
                var temp = JsonHelper.DeserializeJsonToObject<Pf_MessageStatus_Obj>(message_status_json);
                lock (MessageHub.messageContextList)
                {
                    if (MessageHub.messageContextList.Count(x => x.message_content.lane_id == temp.message_content.lane_id) > 0)
                    {
                      

                        MessageHub.messageContextList[MessageHub.messageContextList.FindIndex(x => x.message_content.lane_id == temp.message_content.lane_id)] = temp;

                    }
                }
                Loger.AddLogText(DateTime.Now.ToString() + "修改:" + temp.message_content.lane_name + "数据成功");
                return "修改成功";
            }
            catch (Exception ex)
            {
                Loger.AddErrorText("API修改车道数据失败", ex);
                return "修改失败" + ex.ToString();
            }
        }

    
    }
}
