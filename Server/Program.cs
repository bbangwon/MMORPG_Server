// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Text;
using ServerCore;

// DNS (Domain Name System)
string host = Dns.GetHostName();
IPHostEntry ipHost = Dns.GetHostEntry(host);
IPAddress ipAddr = ipHost.AddressList[0];
IPEndPoint endPoint = new(ipAddr, 7777);

Listener listener = new();
listener.Init(endPoint, () => new GameSession());
Console.WriteLine("Listening...");

while (true) ;

class Knight
{
    public int hp;
    public int attack;
}


class GameSession : Session
{
    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnConnected : {endPoint}");

        var knight = new Knight { hp = 100, attack = 10 };

        var openSegment = SendBufferHelper.Open(4096);

        if(openSegment.Array != null)
        {
            byte[] buffer = BitConverter.GetBytes(knight.hp);
            byte[] buffer2 = BitConverter.GetBytes(knight.attack);          

            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

            var sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);
            Send(sendBuff);
        }

        Thread.Sleep(1000);
        Disconnect();
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnDisconnected : {endPoint}");
    }

    public override int OnRecv(ArraySegment<byte> buffer)
    {
        string recvData = Encoding.UTF8.GetString(buffer.Array!, buffer.Offset, buffer.Count);
        Console.WriteLine($"[From Client] {recvData}");
        return buffer.Count;
    }

    public override void OnSend(int numOfBytes)
    {
        Console.WriteLine($"Transferred bytes: {numOfBytes}");
    }
}