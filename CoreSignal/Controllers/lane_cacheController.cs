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
        public List<pf_MessageStatusContext_Obj> Get()
        {
            //return JsonHelper.SerializeObject(MessageHub.StatusList);
            var value = MessageHub.StatusList;
            return value;
        }

        [HttpPost("{lane_code}")]
        public string Post([FromQuery]string lane_code,[FromBody]pf_MessageStatusContext_Obj message_status)
        {
            
            try
            {
               

               lock(MessageHub.StatusList)
                {
                    if(MessageHub.StatusList.Count(x=>x.lane_code==message_status.lane_code)>0)
                    {
                        
                        MessageHub.StatusList[MessageHub.StatusList.FindIndex(x =>x.lane_code == message_status.lane_code)] = message_status;
                    }
                }

                Loger.AddLogText(DateTime.Now.ToString() + "修改:" + message_status.lane_name + "数据成功");
                return "修改成功";
            }

                
                
            catch (Exception ex)
            {
                Loger.AddErrorText("API修改车道数据失败", ex);
                return "修改失败"+ex.ToString();
                
            }
        }

     

    
    }
}
