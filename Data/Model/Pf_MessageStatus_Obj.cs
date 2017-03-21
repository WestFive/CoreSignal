using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    /// <summary>
    /// JSON总体消息对象
    /// </summary>
    /// 
    public class Pf_MessageStatus_Obj
    {
        /// <summary>
        /// 消息类型 指令或状态
        /// </summary>
        public readonly string message_type = "status";   
        /// <summary>
        /// 消息内容
        /// </summary>
        public pf_MessageStatusContext_Obj message_content { get; set; }

    }
}
