﻿// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;

// DNS (Domain Name System)
string host = Dns.GetHostName();
IPHostEntry ipHost = Dns.GetHostEntry(host);
IPAddress ipAddr = ipHost.AddressList[0];
IPEndPoint endPoint = new(ipAddr, 7777);

Socket listenSocket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

try
{
    listenSocket.Bind(endPoint);
    listenSocket.Listen(10);

    while (true)
    {
        Console.WriteLine("Listening...");

        Socket clientSocket = listenSocket.Accept();

        byte[] recvBuff = new byte[1024];
        int recvBytes = clientSocket.Receive(recvBuff);
        string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
        Console.WriteLine($"[From Client] {recvData}");

        byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
        clientSocket.Send(sendBuff);

        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
    }
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}
