using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class pf_MessageActionContext_Obj
    {
        public string LaneId { get; set; }
        public string ConnectionID { get; set; }

        public DateTime SendTime { get; set; }

        public string ActionCode { get; set; }


    }
}
