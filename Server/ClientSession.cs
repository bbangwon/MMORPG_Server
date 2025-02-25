using ServerCore;
using System.Net;

namespace Server
{  
    class ClientSession : PacketSession
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
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
