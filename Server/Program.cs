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
listener.Init(endPoint, () => new ClientSession());
Console.WriteLine("Listening...");

while (true) ;

