using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
   public struct pf_Damage_Struct
    {
        /// <summary>
        /// 残损面
        /// </summary>
        public string Side { get; set; }

        /// <summary>
        /// 残损代码
        /// </summary>
        public string DamageCode { get; set; }

        /// <summary>
        /// 残损等级
        /// </summary>
        public string DamgeGrade { get; set; }
    }
}
