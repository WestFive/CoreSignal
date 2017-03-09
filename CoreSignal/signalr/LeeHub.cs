using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreSignal.signalr
{
    public class LeeHub:Hub
    {

        public static List<string> List = new List<string>();




        public void Send(string message)
        {
            Clients.All.MessageRecive(message);
        }

     

        public override Task OnConnected()
        {


            List.Add(Context.ConnectionId);
            Clients.All.MessageRecive(ListToString());
           
            return base.OnConnected();
        }


        public override Task OnDisconnected(bool stopCalled)
        {
            if( List.Count(x => x == Context.ConnectionId)>0)
            {
                List.Remove(Context.ConnectionId);
            }
            Clients.All.MessageReive(Context.ConnectionId + "退出了");

            return base.OnDisconnected(stopCalled);
        }

        string ListToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in List)
            {
                sb.AppendFormat(item);
            }

            return string.Format("目前加入的用户列表：{0}",$"{sb}");
        }

    }
}
