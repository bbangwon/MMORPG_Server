// See https://aka.ms/new-console-template for more information

using System.Net;
using Server;
using ServerCore;

PacketManager.Instance.Register();

// DNS (Domain Name System)
string host = Dns.GetHostName();
IPHostEntry ipHost = Dns.GetHostEntry(host);
IPAddress ipAddr = ipHost.AddressList[0];
IPEndPoint endPoint = new(ipAddr, 7777);

Listener listener = new();
listener.Init(endPoint, SessionManager.Instance.Generate);
Console.WriteLine("Listening...");

while (true) ;

public partial class Program
{
    public static GameRoom Room = new();
}

