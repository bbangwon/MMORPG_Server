﻿// See https://aka.ms/new-console-template for more information

using System.Net;
using DummyClient;
using ServerCore;

// DNS (Domain Name System)
string host = Dns.GetHostName();
IPHostEntry ipHost = Dns.GetHostEntry(host);
IPAddress ipAddr = ipHost.AddressList[0];
IPEndPoint endPoint = new(ipAddr, 7777);

Connector connector = new();
connector.Connect(endPoint, () => new ServerSession());


while (true)
{
    try
    {
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
    }

    Thread.Sleep(100);
}




