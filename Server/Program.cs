// See https://aka.ms/new-console-template for more information

using System.Net;
using Server;
using ServerCore;

// DNS (Domain Name System)
string host = Dns.GetHostName();
IPHostEntry ipHost = Dns.GetHostEntry(host);
IPAddress ipAddr = ipHost.AddressList[0];
IPEndPoint endPoint = new(ipAddr, 7777);

Listener listener = new();
listener.Init(endPoint, SessionManager.Instance.Generate);
Console.WriteLine("Listening...");

while (true)
{
    Room.Push(() => Room.Flush());
    Thread.Sleep(250);
}

public partial class Program
{
    public static readonly GameRoom Room = new();
}

