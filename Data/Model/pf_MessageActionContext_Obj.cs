using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class pf_MessageActionContext_Obj
    {
        public string lane_id{ get; set; }
        public string connection_id { get; set; }

        public DateTime send_time { get; set; }

        public string action_code { get; set; }


    }
}
