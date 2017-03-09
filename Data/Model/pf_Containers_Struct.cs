using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public struct pf_Containers_Struct
    {
        /// <summary>
        /// 位置
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 集装箱号
        /// </summary>
        public string ContainerNo { get; set; }
        /// <summary>
        /// OCR识别集装箱号
        /// </summary>
        public string OcrContainerNo { get; set; }

        /// <summary>
        /// ISO码
        /// </summary>
        public string IsoCode { get; set; }
        /// <summary>
        /// 残损信息数组
        /// </summary>
        public pf_Damage_Struct[] Damage { get;set;}
        
           
    }
}
