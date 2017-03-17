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
   
    public class MessageHub : Hub
    {
        /// <summary>
        /// 根目录保存日志
        /// </summary>
        //Log log = new Log(AppContext.BaseDirectory);
        
        ///日志记录
        private readonly ILogger<MessageHub> _logger;

        public MessageHub(ILogger<MessageHub> logger)
        {
            _logger = logger;
            if(File.Exists("wwwroot/config/MessageStatusObj.txt")&&File.Exists("wwwroot/config/LaneID.txt"))
            {
                if (messageDic.Count != 0)
                {
                    List<object> valuelist = JsonHelper.DeserializeJsonToList<object>(File.ReadAllText("wwwroot/config/MessageStatusObj.txt"));
                    List<string> valueKey = JsonHelper.DeserializeJsonToList<string>(File.ReadAllText("wwwroot/config/LaneID.txt"));
                    for (int i = 0; i < valuelist.Count; i++)
                    {
                        messageDic.Add(valueKey[i], valuelist[i]);
                    }
                }

            }
        }


      
        /// <summary>
        /// 车道信息列表。
        /// </summary>
        public static Dictionary<string, object> messageDic = new Dictionary<string, object>();

        public static List<object> messageContextList = new List<object>();
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
                foreach (var item in messageDic)
                {
                    messageContextList.Add(item);//添加到messageContextList中

                }
                Clients.All.GetUserList(JsonHelper.SerializeObject(messageContextList));
                Clients.All.GetSessionList(JsonHelper.SerializeObject(sessionObjectList));
            }
            catch (Exception ex)
            {
                //log.AddErrorText("刷新模块", ex);
                _logger.LogError("刷新模块", ex.ToString());
                
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
                _logger.LogError("会话列表信息推送模块" + ex.ToString());

            }
        }




        [HubMethodName("修改车道状态信息")]
        public void ChangeGateMessage(string SendConnectionID , string LaneID, string JsonMessage)
        {
            try
            {
                var temp = JsonHelper.DeserializeJsonToObject<Object>(JsonMessage);
                if (messageDic.ContainsKey(LaneID))
                {
                    messageDic[LaneID] = temp;


                }
                Clients.Client(SendConnectionID).reciveStatus();
                F5();///刷新
            }
            catch (Exception ex)
            {
                //log.AddErrorText("修改车道信息", ex);
                _logger.LogError("监控修改车道信息变化模块" + ex.ToString());

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
        public void GateBoard(string LaneID,string JsonGateTheSelf)
        {
            try
            {
                var temp = JsonHelper.DeserializeJsonToObject<Object>(JsonGateTheSelf);
                if (messageDic.ContainsKey(LaneID))
                {
                    messageDic[LaneID] = temp;

                }

            }
            catch (Exception ex)
            {
                //log.AddErrorText("客户端变化", ex);
                _logger.LogError("车道变化模块"+ex.ToString());
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

                    Clients.Caller.StatusCode("已作为车道服务登录");
                    sessionObjectList.Add(new SessionObj
                    {
                        ConnectionID = Context.ConnectionId,
                        IPAddress = Context.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                        Port = Context.Request.HttpContext.Connection.RemotePort.ToString(),
                        ClientName = "车道代理" + Context.Request.HttpContext.Connection.RemotePort.ToString(),//车道代理+名字=ClientName
                        ClientType = "Winform",
                        ConnectionTime = DateTime.Now.ToString()
                    });//添加会话对象
                    _logger.LogWarning("车道服务：{0},连接了", Context.Request.HttpContext.Connection.RemoteIpAddress);
                }
            
            
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
                    _logger.LogWarning("浏览器或其他端口：{0},连接了",Context.Request.HttpContext.Connection.RemoteIpAddress);
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


        public override Task OnDisconnected(bool stopCalled)
       {
            try
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
                _logger.LogWarning("浏览器或其他端口：{0},断开了", Context.Request.HttpContext.Connection.RemoteIpAddress);

                if(sessionObjectList.Count(x=>x.ConnectionID==Context.ConnectionId)>0)
                {
                    sessionObjectList.Remove(sessionObjectList.FirstOrDefault(x => x.ConnectionID == Context.ConnectionId));
                }
                ///掉线广播方式
                Clients.All.BoardExit(Context.ConnectionId);

                 

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

    
    }
}
