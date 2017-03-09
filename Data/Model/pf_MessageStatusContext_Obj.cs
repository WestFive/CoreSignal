using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class pf_MessageStatusContext_Obj
    {
        /// <summary>
        /// 车道ID
        /// </summary>
        public string LaneID { get;  set; }
        /// <summary>
        /// 地区编号
        /// </summary>
        public string CountryCode { get;  set; }
        /// <summary>
        /// 区域编号
        /// </summary>
        public string CityCode { get;  set; }
        /// <summary>
        /// 场站编号
        /// </summary>
        public string TerminalCode { get; set; }
        /// <summary>
        /// 车道号
        /// </summary>
        public string LaneName { get;  set; }
        /// <summary>
        /// 车道类型
        /// </summary>
        public string Direction { get; set; }
        /// <summary>
        /// 连接ID
        /// </summary>
        public string ConncetionID { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateTime { get; set; }
        /// <summary>
        ///  车道状态（对象）
        /// </summary>
        public pf_LaneStatus_Obj LaneStatus { get; set; }



    }
}
