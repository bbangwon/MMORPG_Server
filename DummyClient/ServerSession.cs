using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte>? Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;
        public int hp;

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> s)
        {
            ushort count = 0;
            
            //ushort size = BitConverter.ToUInt16(s.Array!, s.Offset + count);
            count += 2;
            //ushort id = BitConverter.ToUInt16(s.Array!, s.Offset + count);
            count += 2;

            this.playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array!, s.Offset + count, s.Count - count));
            count += 8;
        }

        public override ArraySegment<byte>? Write()
        {
            ArraySegment<byte> s = SendBufferHelper.Open(4096);

            ushort count = 0;
            //바로 직접 쓰는 방법
            bool success = true;

            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array!, s.Offset + count, s.Count - count), this.packetId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array!, s.Offset + count, s.Count - count), this.playerId);
            count += 8;

            //Size 채워주기
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array!, s.Offset, s.Count), count);

            if(!success)
                return null;

            return SendBufferHelper.Close(count);
        }
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }

    class ServerSession : Session
    {
        // unsafe구문을 이용한 코드
        //static unsafe void ToBytes(ArraySegment<byte> s, int offset, ulong value)
        //{
        //    fixed (byte* ptr = &s.Array![offset])
        //    {
        //        *(ulong*)ptr = value;
        //    }
        //}

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
            PlayerInfoReq packet = new PlayerInfoReq() { 
                playerId = 1001 
            };

            ArraySegment<byte>? openSegment = packet.Write();
            if(openSegment != null)
                Send(openSegment.Value);

            //for (int i = 0; i < 5; i++)
            {

                //Send
                //ArraySegment<byte> s = SendBufferHelper.Open(4096);

                //ushort count = 0;

                ////바로 직접 쓰는 방법
                //bool success = true;
                
                //count += 2;
                //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array!, s.Offset + count, s.Count - count), packet.packetId);
                //count += 2;
                //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array!, s.Offset + count, s.Count - count), packet.playerId);
                //count += 8;

                ////Size 채워주기
                //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array!, s.Offset, s.Count), count);

                //안정성을 위해 만들어졌으나.. 속도가 느려짐
                //byte[] size = BitConverter.GetBytes(packet.size);
                //byte[] packetId = BitConverter.GetBytes(packet.packetId);
                //byte[] playerId = BitConverter.GetBytes(packet.playerId);

                //Buffer.BlockCopy(size, 0, s.Array!, s.Offset + count, 2);
                //count += 2;

                //Buffer.BlockCopy(packetId, 0, s.Array!, s.Offset + count, 2);
                //count += 2;

                //Buffer.BlockCopy(playerId, 0, s.Array!, s.Offset + count, 8);
                //count += 8;

                //ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);

                //if(success)
                //    Send(sendBuff);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array!, buffer.Offset, buffer.Count).Trim();
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
