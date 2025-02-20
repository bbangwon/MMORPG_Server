using ServerCore;
using System.Net;
using System.Text;

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
        public string name = string.Empty;

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            if (segment.Array == null)
                return;

            var s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            ushort count = 0;
            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += sizeof(ushort);

            //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += sizeof(ushort);

            this.playerId = BitConverter.ToInt64(s[count..]);
            count += sizeof(long);

            ushort nameLen = BitConverter.ToUInt16(s[count..]);
            count += sizeof(ushort);

            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            count += nameLen;
        }

        public override ArraySegment<byte> Write()
        {
            var segment = SendBufferHelper.Open(4096);
            ushort count = 0;
            bool success = true;

            if (segment.Array != null)
            {
                var s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

                count += sizeof(ushort);    //Size만큼 미리 건너뛰기

                success &= BitConverter.TryWriteBytes(s[count..], this.packetId);
                count += sizeof(ushort);

                success &= BitConverter.TryWriteBytes(s[count..], this.playerId);
                count += sizeof(long);

                ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, s[(count + sizeof(ushort))..]);
                success &= BitConverter.TryWriteBytes(s[count..], nameLen);
                count += sizeof(ushort);
                count += nameLen;

                success &= BitConverter.TryWriteBytes(s, count);   //size
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

                        Console.WriteLine($"PlayerInfoReq: {p.playerId} {p.name}");
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
