using Data.Common;
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
    public class MessageHub : Hub
    {
        #region 日志及初始化
        ///日志记录
        private readonly ILogger<MessageHub> _logger;

        public MessageHub(ILogger<MessageHub> logger)
        {
            _logger = logger;
            Loger.FilePath = "wwwroot/Log";
            //Data.MySqlHelper.GetList();、、



            if (messageContextList.Count == 0)
            {
                if (File.Exists("wwwroot/config/MessageStatusObj.txt"))
                    messageContextList = JsonHelper.DeserializeJsonToList<Pf_MessageStatus_Obj>(File.ReadAllText("wwwroot/config/MessageStatusObj.txt"));

            }
            //if (messageContextList.Count == 10)
            //{

            //    File.WriteAllText("wwwroot/config/MessageStatusObj.txt", JsonHelper.SerializeObject(messageContextList));
            //}//预留赋值的方法
        }
        #endregion
        #region 全局变量
        /// <summary>
        /// 消息信息列表。
        /// </summary>
        public static List<Pf_MessageStatus_Obj> messageContextList = new List<Pf_MessageStatus_Obj>();

        public static string ReCode;
        /// <summary>
        /// 车道信息列表。
        /// </summary>
        public static List<pf_MessageStatusContext_Obj> messageStatusContentList = new List<pf_MessageStatusContext_Obj>();
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
                messageStatusContentList.Clear();
                foreach (var item in messageContextList)
                {
                    messageStatusContentList.Add(item.message_content);
                }

                messageContextList.OrderBy(x => x.message_content.lane_code);
                //Clients.All.GetUserList(JsonHelper.SerializeObject(messageContextList));
                Clients.All.GetStatusList(JsonHelper.SerializeObject(messageStatusContentList));
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
        public void LaneStatusChange(string laneID, string JsonMessage)
        {
            var obj = DataHepler.Decoding(JsonMessage);
            if (obj is pf_MessageAction_Obj)//消息指令
            {
                try
                {
                    pf_MessageAction_Obj actionobj = (pf_MessageAction_Obj)obj;
                    if (actionobj.message_context.connection_id != "")
                    {
                        Clients.Client(actionobj.message_context.connection_id).reciveAction(JsonHelper.SerializeObject(actionobj));
                    }
                }
                catch (Exception ex)
                {
                    Loger.AddErrorText("更新消息指令失败", ex);
                }



            }
            else if (obj is Pf_MessageStatus_Obj)//消息状态。
            {
                try
                {
                    Pf_MessageStatus_Obj statusObj = (Pf_MessageStatus_Obj)obj;


                    if (messageContextList.Count(x => x.message_content.lane_code.ToString() == laneID) > 0)
                    {

                        lock (messageContextList)
                        {
                            if (messageContextList.Count(x => x.message_content.lane_code == statusObj.message_content.lane_code) > 0)
                            {
                                statusObj.message_content.update_time = DateTime.Now.ToString();
                                messageContextList[messageContextList.FindIndex(x => x.message_content.lane_code == statusObj.message_content.lane_code)].message_content = statusObj.message_content;
                                if (statusObj.message_content.connection_id != "")
                                {
                                    //如果有这个客户端就推送给这个客户端
                                    Clients.Client(statusObj.message_content.connection_id).reciveStatus(JsonHelper.SerializeObject(statusObj));

                                }

                                F5();//刷新
                                ReCode = "状态刷新/修改成功";
                                GetRe();//服务器返回
                                ReCode = "无状态";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ReCode = "状态刷新/修改失败";
                    GetRe();
                    Loger.AddErrorText("更新状态失败", ex);
                    ReCode = "无状态";
                }


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
        #region 连接事件
        public override Task OnConnected()
        {
            try
            {
                if (Context.QueryString["Type"] == "Client")//表示车道代理
                {


                    if (messageContextList.Count(x => x.message_content.lane_code == Context.QueryString["ID"]) > 0)
                    {

                        var temp = messageContextList.FirstOrDefault(x => x.message_content.lane_code == Context.QueryString["ID"]);

                        temp.message_content.connection_id = Context.ConnectionId;

                        //数据更新
                    }
                    sessionObjectList.Add(new SessionObj
                    {
                        ConnectionID = Context.ConnectionId,
                        IPAddress = Context.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                        Port = Context.Request.HttpContext.Connection.RemotePort.ToString(),
                        ClientName = Context.QueryString["ID"],
                        ClientType = "LaneAgent",
                        ConnectionTime = DateTime.Now.ToString()
                    });//添加会话对象

                    _logger.LogWarning("车道代理{0}连接了", Context.QueryString["ID"]);
                    Loger.AddLogText(DateTime.Now.ToString() + "车道代理+:" + Context.QueryString["ID"] + "连接了");



                    //Pf_MessageStatus_Obj obj = new Pf_MessageStatus_Obj();
                    //obj.message_content = new pf_MessageStatusContext_Obj();
                    //obj.message_content.lane_code = "offline";//保存ID
                    //obj.message_content.lane_code = Context.QueryString["ID"];
                    //messageContextList.Add(obj);


                }
                #region 调试赋值的方法





                #endregion

                else if (Context.QueryString["Type"] == "Watch")//表示是车道监控
                {
                    sessionObjectList.Add(new SessionObj
                    {
                        ConnectionID = Context.ConnectionId,
                        IPAddress = Context.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                        Port = Context.Request.HttpContext.Connection.RemotePort.ToString(),
                        ClientName = "Winform监控" + Context.Request.HttpContext.Connection.RemotePort.ToString(),//车道代理+名字=ClientName
                        ClientType = "Winform",
                        ConnectionTime = DateTime.Now.ToString()
                    });//添加会话对象

                    _logger.LogWarning("车道监控：{0},连接了", Context.Request.HttpContext.Connection.RemoteIpAddress);
                    Loger.AddLogText(DateTime.Now.ToString() + "车道监控+:" + Context.Request.HttpContext.Connection.RemoteIpAddress + "连接了");
                }
                else //表示为浏览器。
                {


                    sessionObjectList.Add(new SessionObj
                    {
                        ConnectionID = Context.ConnectionId,
                        //IPAddress = Context.,
                        IPAddress = Context.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                        Port = Context.Request.HttpContext.Connection.RemotePort.ToString(),
                        ClientName = "Broswer" + Context.Request.HttpContext.Connection.RemotePort.ToString(),//车道代理+名字=ClientName
                        ClientType = "Broswer",
                        ConnectionTime = DateTime.Now.ToString()

                    });//添加会话对象
                    _logger.LogWarning("浏览器或其他端口：{0},连接了", Context.Request.HttpContext.Connection.RemoteIpAddress);
                    Loger.AddLogText(DateTime.Now.ToString() + "浏览器或其他端口：" + Context.Request.HttpContext.Connection.RemoteIpAddress + "连接了");
                }
                F5();//刷新

            }
            catch (Exception ex)
            {
                //log.AddErrorText("连接模块", ex);
                _logger.LogError("连接模块", ex);
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
                if (messageContextList.Count(x => x.message_content.connection_id == Context.ConnectionId) > 0)
                {
                    var temp = messageContextList.FirstOrDefault(x => x.message_content.connection_id == Context.ConnectionId);

                    // messageContextList.Remove(temp);
                    //在就移除 退出
                    temp.message_content.connection_id = "offline";

                    _logger.LogWarning("车道代理：{0},与服务断开连接", Context.Request.HttpContext.Connection.RemoteIpAddress);
                    Loger.AddLogText(DateTime.Now.ToString() + temp.message_content.lane_name + "与服务断开连接");
                }
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


        public void GetRe()
        {
            Clients.Caller.ReciveRe(ReCode);
        }
        /*以下为冗余老代码
         */

        #region OLD


        //[HubMethodName("修改车道状态信息")]
        //public void ChangeGateMessage(string SendConnectionID, string JsonMessage)
        //{
        //    try
        //    {
        //        if (messageContextList.Count(x => x.message_content.connection_id.ToString() == SendConnectionID) > 0)
        //        {
        //            var temp = JsonHelper.DeserializeJsonToObject<Pf_MessageStatus_Obj>(JsonMessage);
        //            ///修改后并传递给车道。
        //            lock (messageContextList)
        //            {
        //                if (messageContextList.Count(x => x.message_content.lane_code == temp.message_content.lane_code) > 0)
        //                {
        //                    //var temptt = messageContextList.FirstOrDefault(x => x.message_content.LaneID == temp.message_content.LaneID);

        //                    messageContextList[messageContextList.FindIndex(x => x.message_content.lane_code == temp.message_content.lane_code)] = temp;

        //                    Clients.Client(SendConnectionID).reciveStatus(JsonHelper.SerializeObject(temp));
        //                }
        //            }
        //        }

        //        F5();///刷新
        //    }
        //    catch (Exception ex)
        //    {
        //        //log.AddErrorText("修改车道信息", ex);
        //        _logger.LogError("监控修改车道信息变化模块" + ex.ToString());

        //    }
        //}

        //[HubMethodName("发送指令")]
        //public void Action(string SendConnectionID, string jsonMessage)
        //{
        //    Clients.Client(SendConnectionID).reciveAction(jsonMessage);

        //}

        ///// <summary>
        ///// 车道变化
        ///// </summary>
        ///// <param name="JsonMessage">整个MessageContext对象</param>
        //[HubMethodName("车道状态改变")]
        //public void GateBoard(string JsonMessage)
        //{
        //    try
        //    {
        //        var temp = JsonHelper.DeserializeJsonToObject<Pf_MessageStatus_Obj>(JsonMessage);
        //        temp.message_content.connection_id = Context.ConnectionId;
        //        temp.message_content.update_time = DateTime.Now.ToString();
        //        lock (messageContextList)
        //        {
        //            if (messageContextList.Count(x => x.message_content.lane_code == temp.message_content.lane_code) > 0)
        //            {
        //                //var temptt = messageContextList.FirstOrDefault(x => x.message_content.LaneID == temp.message_content.LaneID);

        //                messageContextList[messageContextList.FindIndex(x => x.message_content.lane_code == temp.message_content.lane_code)] = temp;

        //                F5();//刷新
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //log.AddErrorText("客户端变化", ex);
        //        _logger.LogError("车道变化模块" + ex.ToString());
        //    }
        //}

        ///// <summary>
        ///// 发送通常点对点消息
        ///// </summary>
        ///// <param name="SendConnctionID"></param>
        ///// <param name="Message"></param>
        //[HubMethodName("发送点对点消息")]
        //public void NormalP2P(string SendConnctionID, string Message)
        //{
        //    Clients.Client(SendConnctionID).Normal(Message);

        //}

        #endregion




    }
}
