﻿using Data.Common;
using Data.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace CoreSignal.signalr
{
    #region 调用说明
    /*
     * 调用说明：
     * 车道监控： 获取车道状态列表：hub.On("GetStatusList",(data)=>{  处理data });
     *            获取在线会话列表：Hub.On("GetSessionList",(data)=>{处理data});
     *            修改车道 hub.Invoke("ChangeStatus","车道ID","修改后的JSON内容");
     *            刷新 hub.Invoke("F5");
     *            角色注册：创建connection连接的时候加入QS键值
     *            1. Dictionary<string, string> dic = new Dictionary<string, string>();
     *            2. dic.Add("Type", "Watch");
     *            3. connection = new HubConnection(服务地址,dic);
     *            
     * 车道代理:  监听获取最新状态（他人更新） hub.On("reciveStatus",(data)=>{  处理DATA });
     *            监听获取指令                 hub.On("reciveAction",(data)=>{ 处理DATA});
     *            修改车道 hub.Invoke("ChangeStatus","车道ID"，"修改后的JSON内容");
     *            角色注册：创建connection连接的时候加入QS键值
     *            1. Dictionary<string, string> dic = new Dictionary<string, string>();
     *            2. dic.Add("Type", "Client");
     *            3. connection = new HubConnection(服务地址,dic);
     *                     
     * Web客户端：获取在线会话列表： proxy.client.GetSessionList = function(datas){ 处理datas}; 
     *            获取车道状态列表： proxy.client.GetStatusList = function(datas){处理datas};
     *                            或者：Get请求  url/api/lane_cache 
     *            修改车道：         Post请求 url/api/lane_cache/{lane_name} //qs参数为: 修改后车道状态的JSON表示
     *            角色注册： 在 $.connection.messageHub 之前：
     *                       $.connection.hub.qs ="Type ={类型}";
     *                       例如 注册 Client 
     *                       $.connection.hub.qs="Type = Client";
     *                       注册Watch
     *                       $.connection.hub.qs = "Type = Watch";
     *                       默认不注册。
     *            
     */
    #endregion
    public class MessageHub : Hub
    {
        #region 日志及初始化
        ///日志记录
        private readonly ILogger<MessageHub> _logger;

        private bool SetValue = false;//调试赋值此项设正
        public MessageHub(ILogger<MessageHub> logger)
        {
            _logger = logger;
            Loger.FilePath = "wwwroot/Log";
            //Data.MySqlHelper.GetList();、、



            if (StatusList.Count == 0)
            {
                if (File.Exists("wwwroot/config/MessageStatusObj.txt"))
                    StatusList = JsonHelper.DeserializeJsonToList<pf_MessageStatusContext_Obj>(File.ReadAllText("wwwroot/config/MessageStatusObj.txt"));

            }
            if (SetValue)//调试用
            {
                if (StatusList.Count == 10)
                {

                    File.WriteAllText("wwwroot/config/MessageStatusObj.txt", JsonHelper.SerializeObject(StatusList));
                }//预留赋值的方法
            }
        }
        #endregion
        #region 全局变量
        ///// <summary>
        ///// 消息信息列表。
        ///// </summary>
        //public static List<Pf_Message_Obj> StatusList = new List<Pf_Message_Obj>();

        public static string ReCode;
        /// <summary>
        /// 车道信息列表。
        /// </summary>
        public static List<pf_MessageStatusContext_Obj> StatusList = new List<pf_MessageStatusContext_Obj>();

        /// <summary>
        /// 作业信息列表
        /// </summary>
        public static List<pf_JobContet_Obj> JobList = new List<pf_JobContet_Obj>();
        /// <summary>
        /// 会话信息列表。
        /// </summary>
        public static List<SessionObj> sessionObjectList = new List<SessionObj>();

        #endregion
        #region 刷新
        /// <summary>
        /// 刷新列表并推送至所有已连接上的车道客户端。
        /// </summary>
        public void F5()
        {
            try
            {

                Clients.All.GetStatusList(JsonHelper.SerializeObject(StatusList));
                Clients.All.GetSessionList(JsonHelper.SerializeObject(sessionObjectList));
            }
            catch (Exception ex)
            {
                //log.AddErrorText("刷新模块", ex);
                _logger.LogError("刷新模块", ex.ToString());

            }
        }
        #endregion
        #region 发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="laneID"></param>
        /// <param name="JsonMessage"></param>
        [HubMethodName("ChangeStatus")]
        public void LaneStatusChange(string laneCode, string JsonMessage)
        {
            try
            {
                Pf_Message_Obj obj = (Pf_Message_Obj)DataHepler.Decoding(JsonMessage);
                switch (obj.message_type)
                {
                    case "Status":
                        var statusObj = JsonHelper.DeserializeJsonToObject<pf_MessageStatusContext_Obj>(JsonHelper.SerializeObject(obj.message_content));
                        if (StatusList.Count(x => x.lane_code == laneCode) > 0)
                        {

                            lock (StatusList)
                            {
                                if (StatusList.Count(x => x.lane_code == statusObj.lane_code) > 0)
                                {
                                    statusObj.update_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    StatusList[StatusList.FindIndex(x => x.lane_code == statusObj.lane_code)] = statusObj;//车道对象
                                    if (statusObj.connection_id != "")
                                    {
                                        var reobj = new Pf_Message_Obj { message_type = "Status", message_content = statusObj };//编码成消息对象

                                        Clients.Client(statusObj.connection_id).reciveStatus(JsonHelper.SerializeObject(reobj));  //如果有这个客户端就推送给这个客户端

                                    }

                                    F5();//刷新
                                    ReCode = "状态刷新/修改成功";
                                    GetRe();//服务器返回
                                    ReCode = "无状态";
                                }
                            }
                        }
                        break;
                    case "Action":
                        var actionObj = JsonHelper.DeserializeJsonToObject<pf_MessageActionContext_Obj>(JsonHelper.SerializeObject(obj.message_content));
                        if (actionObj.connection_id != "")
                        {
                            var reobj = new Pf_Message_Obj { message_type = "Action", message_content = actionObj };
                            Clients.Client(actionObj.connection_id).reciveAction(JsonHelper.SerializeObject(reobj));//返回JSON对象
                        }
                        break;
                    case "Job":
                        var jobObj = JsonHelper.DeserializeJsonToObject<pf_JobContet_Obj>(JsonHelper.SerializeObject(obj.message_content));


                        break;
                }
            }
            catch (Exception ex)
            {

                ReCode = "状态刷新/修改失败";
                GetRe();
                Loger.AddErrorText("更新状态失败", ex);

            }






        }
        #endregion
        #region 会话列表

        /// <summary>
        /// 刷新推送获取会话列表。
        /// </summary>
        public void SessionList()
        {
            try
            {
                Clients.All.GetSessionList(JsonHelper.SerializeObject(sessionObjectList));
            }
            catch (Exception ex)
            {
                //log.AddErrorText("刷新模块", ex);
                _logger.LogError("会话列表信息推送模块" + ex.ToString());

            }
        }

        #endregion
        #region  添加到会话缓存列表
        private void AddToSession()
        {
            sessionObjectList.Add(new SessionObj
            {
                ConnectionID = Context.ConnectionId,
                IPAddress = Context.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                Port = Context.Request.HttpContext.Connection.RemotePort.ToString(),
                ClientName = Context.QueryString["Name"],
                ClientType = Context.QueryString["Type"],
                ConnectionTime = DateTime.Now.ToString()
            });//添加会话对象
            _logger.LogWarning(DateTime.Now.ToString() + "{0}连接了", Context.QueryString["Name"]);
            Loger.AddLogText(DateTime.Now.ToString() + Context.QueryString["Type"] + ":" + Context.QueryString["ID"] + "连接了");
        }
        #endregion
        #region 连接事件
        public override Task OnConnected()
        {

            //连接角色判断。
            try
            {

                switch (Context.QueryString["Type"])
                {
                    case "Client":

                        if (StatusList.Count(x => x.lane_code == Context.QueryString["Name"]) > 0)
                        {

                            var temp = StatusList.FirstOrDefault(x => x.lane_code == Context.QueryString["Name"]);

                            temp.connection_id = Context.ConnectionId;


                            //数据更新
                        }
                        if (SetValue)//调试用赋值方法
                        {
                            pf_MessageStatusContext_Obj obj = new pf_MessageStatusContext_Obj();
                            obj.lane_code = Context.QueryString["Name"];
                            obj.connection_id = "offline";
                            StatusList.Add(obj);
                        }
                        break;
                    case "LaneWatch":

                        break;
                    case "Broswer":

                        break;
                    case "WorkWatch":

                        break;
                    default:

                        break;


                }
                AddToSession();//加入车道缓存。
                F5();//刷新
                #region 测试用

                #endregion
            }
            catch (Exception ex)
            {
                Loger.AddErrorText("连接方法", ex);
            }

            return base.OnConnected();
        }

        #endregion
        #region 断开连接事件
        public override Task OnDisconnected(bool stopCalled)
        {
            try
            {
                ///判断是否已经存在该条车道
                if (StatusList.Count(x => x.connection_id == Context.ConnectionId) > 0)
                {
                    var temp = StatusList.FirstOrDefault(x => x.connection_id == Context.ConnectionId);

                    // StatusList.Remove(temp);
                    //在就移除 退出
                    temp.connection_id = "offline";
                    temp.lane_status = "{}";

                    _logger.LogWarning("车道代理：{0},与服务断开连接", Context.Request.HttpContext.Connection.RemoteIpAddress);
                    Loger.AddLogText(DateTime.Now.ToString() + temp.lane_name + "与服务断开连接");
                }

                //判断是否存在会话。
                if (sessionObjectList.Count(x => x.ConnectionID == Context.ConnectionId) > 0)
                {
                    var temp = sessionObjectList.FirstOrDefault(x => x.ConnectionID == Context.ConnectionId);
                    sessionObjectList.Remove(temp);//包含则移除。
                    Loger.AddLogText(DateTime.Now.ToString() + temp.ConnectionID + "与服务断开连接");
                }

                F5();
            }
            catch (Exception ex)
            {
                //log.AddErrorText("断开连接模块", ex);
                _logger.LogError("断开连接模块", ex);
            }

            //addTolog("断开服务器");
            return base.OnDisconnected(stopCalled);
        }
        #endregion
        #region 给予前端修改的执行结果反馈
        //给予前端执行结果指令。
        public void GetRe()
        {
            Clients.Caller.ReciveRe(ReCode);
        }
        #endregion




    }
}
