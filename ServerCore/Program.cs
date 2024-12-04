// See https://aka.ms/new-console-template for more information

using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

// DNS (Domain Name System)
string host = Dns.GetHostName();
IPHostEntry ipHost = Dns.GetHostEntry(host);
IPAddress ipAddr = ipHost.AddressList[0];
IPEndPoint endPoint = new(ipAddr, 7777);

Listener listener = new();
listener.Init(endPoint, OnAcceptHandler);
Console.WriteLine("Listening...");

while (true);

void OnAcceptHandler(Socket clientSocket)
{
    try
    {
        Session session = new();
        session.Start(clientSocket);

        byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
        session.Send(sendBuff);

        Thread.Sleep(1000);

        session.Disconnect();
        session.Disconnect();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
    }
}