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
        public string? name;

        public struct SkillInfo
        {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte> s, ref ushort count)
            {
                bool success = true;

                success &= BitConverter.TryWriteBytes(s[count..], this.id);
                count += sizeof(int);

                success &= BitConverter.TryWriteBytes(s[count..], this.level);
                count += sizeof(short);

                success &= BitConverter.TryWriteBytes(s[count..], this.duration);
                count += sizeof(float);

                return success;
            }

            public void Read(ReadOnlySpan<byte> s, ref ushort count)
            {
                this.id = BitConverter.ToInt32(s[count..]);
                count += sizeof(int);

                this.level = BitConverter.ToInt16(s[count..]);
                count += sizeof(short);

                this.duration = BitConverter.ToSingle(s[count..]);
                count += sizeof(float);
            }
        }

        public List<SkillInfo> skills = new List<SkillInfo>();

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;

            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array!, segment.Offset, segment.Count);
            
            //ushort size = BitConverter.ToUInt16(s.Array!, s.Offset + count);
            count += sizeof(ushort);
            //ushort id = BitConverter.ToUInt16(s.Array!, s.Offset + count);
            count += sizeof(ushort);

            this.playerId = BitConverter.ToInt64(s[count..]);
            count += sizeof(long);

            ushort nameLen = BitConverter.ToUInt16(s[count..]);
            count += sizeof(ushort);

            this.name = Encoding.Unicode.GetString(s[count..(count + nameLen)]);
            count += nameLen;

            skills.Clear();
            ushort skillLen = BitConverter.ToUInt16(s[count..]);
            count += sizeof(ushort);

            for (int i = 0; i < skillLen; i++)
            {
                SkillInfo skill = new SkillInfo();
                skill.Read(s, ref count);
                this.skills.Add(skill);
            }
        }

        public override ArraySegment<byte>? Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            //바로 직접 쓰는 방법
            bool success = true;

            Span<byte> s = new Span<byte>(segment.Array!, segment.Offset, segment.Count);

            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s[count..], this.packetId);
            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s[count..], this.playerId);
            count += sizeof(long);

            //ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.name!);
            //success &= BitConverter.TryWriteBytes(s[count..], nameLen);
            //count += sizeof(ushort);

            //Array.Copy(Encoding.Unicode.GetBytes(this.name!), 0, segment.Array!, segment.Offset + count, nameLen);
            //count += nameLen;

            //string
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name!, 0, this.name!.Length, segment.Array!, segment.Offset + count + sizeof(ushort));

            success &= BitConverter.TryWriteBytes(s[count..], nameLen);
            count += sizeof(ushort);

            count += nameLen;

            // Skill List
            success &= BitConverter.TryWriteBytes(s[count..], (ushort)this.skills.Count);
            count += sizeof(ushort);

            foreach (SkillInfo skill in this.skills)
            {
                success &= skill.Write(s, ref count);
            }



            //Size 채워주기
            success &= BitConverter.TryWriteBytes(s, count);

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
                playerId = 1001,
                name = "ABCD"
            };

            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 101, level = 1, duration = 3.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 102, level = 2, duration = 5.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 103, level = 3, duration = 7.0f });


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
