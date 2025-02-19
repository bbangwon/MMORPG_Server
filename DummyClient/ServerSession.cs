using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;
    }

    class PlayerInfoOk : Packet
    {
        public int hp;
        public int attack;
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }

    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            var packet = new PlayerInfoReq { size = 12, packetId = (ushort)PacketID.PlayerInfoReq, playerId = 1001 };

            //for (int i = 0; i < 5; i++)
            {
                var s = SendBufferHelper.Open(4096);
                if (s.Array != null)
                {
                    ushort count = 0;
                    bool success = true;

                    count += 2;
                    success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.packetId);
                    count += 2;
                    success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.playerId);
                    count += 8;                    
                    success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);   //size

                    var sendBuff = SendBufferHelper.Close(count);

                    if(success)
                        Send(sendBuff);
                }
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array!, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }

}
