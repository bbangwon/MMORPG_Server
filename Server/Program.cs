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

class Packet
{
    public ushort size;
    public ushort packetId;
}

class GameSession : PacketSession
{
    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnConnected : {endPoint}");

        //var packet = new Packet { size = 100, packetId = 10 };

        //var openSegment = SendBufferHelper.Open(4096);

        //if(openSegment.Array != null)
        //{
        //    byte[] buffer = BitConverter.GetBytes(packet.size);
        //    byte[] buffer2 = BitConverter.GetBytes(packet.packetId);          

        //    Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
        //    Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

        //    var sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);
        //    Send(sendBuff);
        //}

        Thread.Sleep(5000);
        Disconnect();
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnDisconnected : {endPoint}");
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        if (buffer.Array == null)
            return;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

        Console.WriteLine($"RecvPacketId: {id} Size {size}");
    }

    public override void OnSend(int numOfBytes)
    {
        Console.WriteLine($"Transferred bytes: {numOfBytes}");
    }
}