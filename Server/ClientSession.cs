using ServerCore;
using System.Net;

namespace Server
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> s)
        {
            if (s.Array == null)
                return;

            ushort count = 0;
            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += 2;

            //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += 2;

            //ReadOnlySpan 지정된 메모리 영역을 읽기 전용으로 제공하는 구조체
            this.playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count));
            count += 8;
        }

        public override ArraySegment<byte> Write()
        {
            var s = SendBufferHelper.Open(4096);
            ushort count = 0;
            bool success = true;

            if (s.Array != null)
            {

                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId);
                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);
                count += 8;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);   //size                
            }

            if (!success)
                return default;

            return SendBufferHelper.Close(count);
        }
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }

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
            if (buffer.Array == null)
                return;

            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch ((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        var p = new PlayerInfoReq();
                        p.Read(buffer);

                        Console.WriteLine($"PlayerInfoReq: {p.playerId}");
                    }
                    break;
                case PacketID.PlayerInfoOk:
                    break;
                default:
                    break;
            }

            Console.WriteLine($"RecvPacketId: {id} Size {size}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
