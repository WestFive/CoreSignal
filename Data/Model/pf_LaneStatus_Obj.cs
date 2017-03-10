using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class pf_LaneStatus_Obj
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        public string LaneType { get; set; }
        /// <summary>
        /// 作业开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 作业结束时间 指放行时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 车牌号，第一次赋值与OcrTruckNo一致，如果识别错误，用户可修改
        /// </summary>
        public string TrunkNo { get; set; }
        /// <summary>
        /// OCR识别车牌号，第一次赋值后不再修改，用于日后在后台数据库与TruckNo进行比较，计算车牌识别率
        /// </summary>
        public string OcrTruckNo { get; set; }
        /// <summary>
        /// rfid电子车牌号
        /// </summary>
        public string RfidTruckNo { get; set; }
        /// <summary>
        /// 地磅重量
        /// </summary>
        public float WeightBridge { get; set; }
        /// <summary>
        /// 起落杆状态，down为落杆，up为起杆，与图标关联
        /// </summary>
        public string Barrier { get; set; }
        /// <summary>
        /// IC卡读卡号
        /// </summary>
        public int IcCardNo { get; set; }
        /// <summary>
        /// 与进度条“车道就绪”关联
        /// </summary>
        public bool LaneReady { get; set; }
        /// <summary>
        /// 与进度条“感应来车”关联，以及车斗图标关联
        /// </summary>
        public bool HasTruck { get; set; }
        /// <summary>
        /// 与进度条“车牌读卡”关联
        /// </summary>
        public bool TruckRfid { get; set; }
        /// <summary>
        /// 与进度条“车牌识别”关连
        /// </summary>
        public bool TruckOrc { get; set; }
        /// <summary>
        ///  与进度条“箱号识别”关联
        /// </summary>
        public bool ContainerOcr { get; set; }
        /// <summary>
        /// \\与进度条“司机读卡”关联
        /// </summary>
        public bool ReadIcCard { get; set; }
        /// <summary>
        ///   \\与进度条“箱体验残”关联
        /// </summary>
        public bool DamageCheck { get; set; }
        /// <summary>
        /// \\与进度条“提交TOS”关联
        /// </summary>
        public bool SubmitToTos { get; set; }
        /// <summary>
        ///  \\与进度条“TOS反馈”关联
        /// </summary>
        public bool ReplyFromTos { get; set; }
        /// <summary>
        /// \\与进度条“打印小票”关联
        /// </summary>
        public bool PrintRecipt { get; set; }
        /// <summary>
        ///    \\与进度条“抬杆放行”关联
        /// </summary>
        public bool LiftBarrier { get; set; }
        /// <summary>
        /// 验残次数
        /// </summary>
        public int DamageChecks { get; set; } 
        /// <summary>
        /// 残损记录数
        /// </summary>
        public int DamageCounts { get; set;}
        /// <summary>
        /// 车头图片URL
        /// </summary>
        public  string TruckPicUrl { get; set; }
        /// <summary>
        /// 前顶图片URL
        /// </summary>
        public string FrontTopPicUrl { get; set; }
        /// <summary>
        /// 后顶图片URL
        /// </summary>
        public string BackTopPicUrl { get; set; }
        /// <summary>
        /// 左前图片URL
        /// </summary>
        public string LeftFrontPicUrl { get; set; }
        /// <summary>
        /// 左后图片URL
        /// </summary>
        public string LeftBackPicUrl { get; set; }
        /// <summary>
        /// 右前图片URL
        /// </summary>
        public string RightFrontPicUrl { get; set; }
        /// <summary>
        /// 右后图片URL
        /// </summary>
        public string RightBackPicUrl { get; set; }
        /// <summary>
        /// 左面箱体拼接图片URL
        /// </summary>
         public string LeftDamagePicUrl { get; set; }
        /// <summary>
        /// 右面箱体拼接图片URL
        /// </summary>
        public string RightDamagePicUrl { get; set; }
        /// <summary>
        /// 顶面箱体拼接图片URL
        /// </summary>
        public string TopDamagePicUrl { get; set; }
        
        public  pf_Containers_Struct []Containers { get; set; }



    }
}
