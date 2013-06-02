﻿using Fleck;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fleck
{
    public class FuncModuleManager
    {
        public static List<IWebSocketConnection> FModuleList = new List<IWebSocketConnection>();

        public static void removeClient(IWebSocketConnection client)
        {
            IWebSocketConnection c = FModuleList.Find((_client) =>
            {
                return _client.ConnectionInfo.Id == client.ConnectionInfo.Id;
                //return _client.ConnectionInfo.Origin == client.ConnectionInfo.Origin;
            });
            if (c != null)
            {
                FModuleList.Remove(client);
            }
            Debug.WriteLine("*****  FM Client --  => " + FModuleList.Count.ToString());

        }
        public static void addClient(IWebSocketConnection client)
        {
            IWebSocketConnection c = FModuleList.Find((_client) =>
            {
                //return _client.ConnectionInfo.Origin == client.ConnectionInfo.Origin;
                return _client.ConnectionInfo.Id == client.ConnectionInfo.Id;
            });
            if (c == null)
            {
                FModuleList.Add(client);
            }
            Debug.WriteLine("*****  FM Client ++  => " + FModuleList.Count.ToString());
        }

        public static void OnMessage(string data)
        {
            FModuleList.ForEach(s => { s.Send(data); });
        }
    }
}
