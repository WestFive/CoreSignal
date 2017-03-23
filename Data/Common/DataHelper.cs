using Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Common
{
    public static class DataHepler
    {
        ///// <summary>
        ///// 解码MessageStatusObj
        ///// </summary>
        ///// <param name="json"></param>
        ///// <returns></returns>
        //public static Pf_MessageStatus_Obj DecodingMessageStatus(string json)
        //{
        //    return JsonHelper.DeserializeJsonToObject<Pf_MessageStatus_Obj>(json);
        //}


        ///// <summary>
        ///// 编码MessageStatusObj
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static string EncodingMessageStatus(Pf_MessageStatus_Obj obj)
        //{
        //    return JsonHelper.SerializeObject(obj);
        //}
        ///// <summary>
        ///// 解码MessageStatusObj集合
        ///// </summary>
        ///// <param name="json"></param>
        ///// <returns></returns>
        //public static List<Pf_MessageStatus_Obj> DecodingMessageStatusList(string json)
        //{
        //    return JsonHelper.DeserializeJsonToList<Pf_MessageStatus_Obj>(json);
        //}


        ///// <summary>
        ///// 编码MessageStatusObj集合
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static string EncodingMessageStatusList(List<Pf_MessageStatus_Obj> obj)
        //{
        //    return JsonHelper.SerializeObject(obj);
        //}


            /// <summary>
            /// 编码
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
        public static string Encoding(object obj)
        {
            return JsonHelper.SerializeObject(obj);
        }

        //解码

        public static Object Decoding(string json)
        {

            try
            {
                Pf_MessageStatus_Obj statusObj = JsonHelper.DeserializeJsonToObject<Pf_MessageStatus_Obj>(json);
                return statusObj;
            }
            catch 
            {
                pf_MessageAction_Obj actionObj = JsonHelper.DeserializeJsonToObject<pf_MessageAction_Obj>(json);
                return actionObj;
            }

           
            
        }

       


    }
}
