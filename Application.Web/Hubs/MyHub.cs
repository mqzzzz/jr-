﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Application.Web
{
    public class MyHub : Hub
    {
        /// <summary>
        /// 用户的connectionID与用户名对照表
        /// </summary>
        private readonly static Dictionary<string, string> connections = new Dictionary<string, string>();

        public void Send(string name, string message)
        {
            message = string.Format("消息:{0} 时间:{1}", message, DateTime.Now.ToString());
            //调用所有客户端的addNewMessageToPage function  
            Clients.All.addNewMessageToPage(name, message);
        }

        public static void Show(string name, string content)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();

            //广播，所有连接到服务器的客户端都会收到该通知
            string message = "服务器广播的消息：hello";
            context.Clients.All.hello(message);

            //推送到某个用户
            string toUserId = "";
            if (connections.ContainsKey(name))
            {
                toUserId = connections[name];
                context.Clients.Client(toUserId).SendMessage(content == "" ? "服务器给你推送消息了" : content);
            }
        }
        /// <summary>
        /// 用户上线函数
        /// </summary>
        /// <param name="name"></param>
        public void SendLogin(string name)
        {
            if (!connections.ContainsKey(name))
            {
                //这里是将用户id和姓名联系起来
                connections.Add(name, Context.ConnectionId);
            }
            else
            {
                //每次登陆id会发生变化
                connections[name] = Context.ConnectionId;
            }
        }
    }
}