using ServerCore;
using System.Net;
using System.Text;

namespace Server
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

            if (!success)
                return null;

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

            //Packet knight = new Packet() { size = 100, packetId = 10 };
            
            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(knight.size);
            //byte[] buffer2 = BitConverter.GetBytes(knight.packetId);
            //Buffer.BlockCopy(buffer, 0, openSegment.Array!, openSegment.Offset, buffer.Length);
            //Buffer.BlockCopy(buffer2, 0, openSegment.Array!, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

            //Send(sendBuff);
            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array!, buffer.Offset + count);
            count += 2;

            ushort id = BitConverter.ToUInt16(buffer.Array!, buffer.Offset + count);
            count += 2;

            switch ((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);

                        Console.WriteLine($"PlayerInfoReq {p.playerId} {p.name}");

                        foreach (PlayerInfoReq.SkillInfo skill in p.skills)
                        {
                            Console.WriteLine($"Skill Id({skill.id}) Level({skill.level}) Duration({skill.duration})");
                        }
                    }
                    break;
                case PacketID.PlayerInfoOk:
                    break;
                default:
                    break;
            }

            Console.WriteLine($"RecvPacketId: {id}, Size: {size}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}