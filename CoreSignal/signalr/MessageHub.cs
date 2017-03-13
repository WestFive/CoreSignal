using Data.Common;
using Data.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;



namespace CoreSignal.signalr
{
   
    public class MessageHub : Hub
    {
        /// <summary>
        /// 根目录保存日志
        /// </summary>
        //Log log = new Log(AppContext.BaseDirectory);
        /// <summary>
        /// 车道信息列表。
        /// </summary>
        public static List<Pf_MessageStatus_Obj> messageContextList = new List<Pf_MessageStatus_Obj>();
        /// <summary>
        /// 会话信息列表。
        /// </summary>
        public static List<SessionObj> sessionObjectList = new List<SessionObj>();

        /// <summary>
        /// 刷新列表并推送至所有已连接上的车道客户端。
        /// </summary>
        public void F5()
        {
            try
            {
                Clients.All.GetUserList(DataHepler.EncodingMessageStatusList(messageContextList));
                Clients.All.GetSessionList(JsonHelper.SerializeObject(sessionObjectList));
            }
            catch (Exception ex)
            {
                //log.AddErrorText("刷新模块", ex);
            }
        }

        

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
            }
        }




        [HubMethodName("修改车道状态信息")]
        public void ChangeGateMessage(string SendConnectionID, string JsonMessage)
        {
            try
            {
                if (messageContextList.Count(x => x.MessageContent.ConnectionID.ToString() == SendConnectionID) > 0)
                {
                    Pf_MessageStatus_Obj temp = messageContextList.FirstOrDefault(x => x.MessageContent.ConnectionID.ToString() == SendConnectionID);
                    temp.MessageContent.LaneStatus = JsonHelper.DeserializeJsonToObject<pf_LaneStatus_Obj>(JsonMessage);
                    Clients.Client(SendConnectionID).reciveStatus(DataHepler.EncodingMessageStatus(temp));
                    ///修改后并传递给车道。
                }

                F5();///刷新
            }
            catch (Exception ex)
            {
                //log.AddErrorText("修改车道信息", ex);

            }
        }

        [HubMethodName("发送指令")]
        public void Action(string SendConnectionID, string jsonMessage)
        {
            Clients.Client(SendConnectionID).reciveAction(jsonMessage);

        }

        /// <summary>
        /// 车道变化
        /// </summary>
        /// <param name="JsonMessage">整个MessageContext对象</param>
        [HubMethodName("车道状态改变")]
        public void GateBoard(string JsonMessage)
        {
            try
            {
                var temp = DataHepler.DecodingMessageStatus(JsonMessage);
                temp.MessageContent.ConnectionID = Context.ConnectionId;
                temp.MessageContent.UpdateTime = DateTime.Now.ToString();
                lock (messageContextList)
                {
                    if (messageContextList.Count(x => x.MessageContent.LaneID == temp.MessageContent.LaneID) > 0)
                    {
                        var temptt = messageContextList.FirstOrDefault(x => x.MessageContent.LaneID == temp.MessageContent.LaneID);
                        messageContextList.Remove(temptt);
                        messageContextList.Add(temp);//替换

                        F5();//刷新
                    }
                }
            }
            catch (Exception ex)
            {
                //log.AddErrorText("客户端变化", ex);
            }
        }

        /// <summary>
        /// 发送通常点对点消息
        /// </summary>
        /// <param name="SendConnctionID"></param>
        /// <param name="Message"></param>
        [HubMethodName("发送点对点消息")]
        public void NormalP2P(string SendConnctionID, string Message)
        {
            Clients.Client(SendConnctionID).Normal(Message);

        }


        public override Task OnConnected()
        {
            try
            {
                if (Context.QueryString["Type"] == "Client")//表示车道代理
                {


                    Pf_MessageStatus_Obj obj = new Pf_MessageStatus_Obj();
                    obj.MessageContent = new pf_MessageStatusContext_Obj();
                    obj.MessageContent.LaneStatus = new pf_LaneStatus_Obj();
                    obj.MessageContent.ConnectionID = Context.ConnectionId;//保存ID
                    obj.MessageContent.LaneID = Context.QueryString["ID"];
                    if (messageContextList.Count(x => x.MessageContent.LaneID == obj.MessageContent.LaneID) > 0)//数据更新
                    {
                        var temp = messageContextList.FirstOrDefault(x => x.MessageContent.LaneID == obj.MessageContent.LaneID);
                        //gateList.Remove(temp);
                        temp = obj;
                        // gateList.Add(temp);

                    }
                    else//数据添加
                    {
                        messageContextList.Add(obj);
                        sessionObjectList.Add(new SessionObj
                        {
                            ConnectionID = Context.ConnectionId,
                            IPAddress = Context.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                            Port = Context.Request.HttpContext.Connection.RemotePort.ToString(),
                            ClientName = obj.MessageContent.LaneID,
                            ClientType = "LaneAgent",
                            ConnectionTime = DateTime.Now.ToString()
                        });//添加会话对象
                    }
                }
                else if (Context.QueryString["Type"] == "Watch")//表示是车道监控
                {
                    sessionObjectList.Add(new SessionObj
                    {
                        ConnectionID = Context.ConnectionId,
                        IPAddress = Context.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                        Port = Context.Request.HttpContext.Connection.RemotePort.ToString(),
                        ClientName = "Winform车道代理" + Context.Request.HttpContext.Connection.RemotePort.ToString(),//车道代理+名字=ClientName
                        ClientType = "Winform",
                        ConnectionTime = DateTime.Now.ToString()
                    });//添加会话对象

                }
                else //表示为浏览器。
                {


                    sessionObjectList.Add(new SessionObj
                    {
                        ConnectionID = Context.ConnectionId,
                        //IPAddress = Context.,
                        IPAddress = Context.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                        Port = Context.Request.HttpContext.Connection.RemotePort.ToString(),
                        ClientName = "Broswer" + Context.Request.HttpContext.Connection.RemotePort.ToString(),//车道代理+名字=ClientName
                        ClientType = "Broswer",
                        ConnectionTime = DateTime.Now.ToString()
                    });//添加会话对象
                }
                F5();//刷新
            }
            catch (Exception ex)
            {
                //log.AddErrorText("连接模块", ex);
            }

            return base.OnConnected();
        }


        public override Task OnDisconnected(bool stopCalled)
        {
            try
            {
                ///判断是否已经存在该条车道
                if (messageContextList.Count(x => x.MessageContent.ConnectionID == Context.ConnectionId) > 0)
                {
                    var temp = messageContextList.FirstOrDefault(x => x.MessageContent.ConnectionID == Context.ConnectionId);

                    messageContextList.Remove(temp);
                    //在就移除 退出
                    //  log.AddLogText("车道" + temp.LanelID+ "退出了服务器");
                }
                if (sessionObjectList.Count(x => x.ConnectionID == Context.ConnectionId) > 0)
                {
                    var temp = sessionObjectList.FirstOrDefault(x => x.ConnectionID == Context.ConnectionId);


                    sessionObjectList.Remove(temp);//包含则移除。

                }

                F5();
            }
            catch (Exception ex)
            {
                //log.AddErrorText("断开连接模块", ex);
            }

            //addTolog("断开服务器");
            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// 连接心跳。
        /// </summary>
        [HubMethodName("心跳")]
        public void HeartBeat()
        {
            try
            {
                Clients.Caller.ListenHeartBeat("True");
            }
            catch (Exception ex)
            {
                //log.AddErrorText("心跳", ex);
            }
        }

    }
}
