using ServerCore;
using System.Net;
using System.Text;

namespace Server
{
    public enum PacketID
    {
        PlayerInfoReq = 1,
        Test = 2,

    }

    public class PlayerInfoReq
    {
        public byte testByte;
        public long playerId;
        public string name = string.Empty;
        public class Skill
        {
            public int id;
            public short level;
            public float duration;
            public class Attribute
            {
                public int att;

                public void Read(ReadOnlySpan<byte> s, ref ushort count)
                {
                    this.att = BitConverter.ToInt32(s[count..]);
                    count += sizeof(int);

                }

                public bool Write(Span<byte> s, ref ushort count)
                {
                    bool success = true;

                    success &= BitConverter.TryWriteBytes(s[count..], this.att);
                    count += sizeof(int);

                    return success;
                }
            }

            public List<Attribute> attributes = [];

            public void Read(ReadOnlySpan<byte> s, ref ushort count)
            {
                this.id = BitConverter.ToInt32(s[count..]);
                count += sizeof(int);

                this.level = BitConverter.ToInt16(s[count..]);
                count += sizeof(short);

                this.duration = BitConverter.ToSingle(s[count..]);
                count += sizeof(float);

                this.attributes.Clear();
                ushort attributeLen = BitConverter.ToUInt16(s[count..]);
                count += sizeof(ushort);
                for (int i = 0; i < attributeLen; i++)
                {
                    var attribute = new Attribute();
                    attribute.Read(s, ref count);
                    attributes.Add(attribute);
                }

            }

            public bool Write(Span<byte> s, ref ushort count)
            {
                bool success = true;

                success &= BitConverter.TryWriteBytes(s[count..], this.id);
                count += sizeof(int);

                success &= BitConverter.TryWriteBytes(s[count..], this.level);
                count += sizeof(short);

                success &= BitConverter.TryWriteBytes(s[count..], this.duration);
                count += sizeof(float);

                success &= BitConverter.TryWriteBytes(s[count..], (ushort)this.attributes.Count);
                count += sizeof(ushort);
                foreach (var attribute in this.attributes)
                    success &= attribute.Write(s, ref count);

                return success;
            }
        }

        public List<Skill> skills = [];

        public void Read(ArraySegment<byte> segment)
        {
            if (segment.Array == null)
                return;

            var s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            ushort count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);

            this.testByte = (byte)segment.Array[segment.Offset + count];
            count += sizeof(byte);

            this.playerId = BitConverter.ToInt64(s[count..]);
            count += sizeof(long);

            ushort nameLen = BitConverter.ToUInt16(s[count..]);
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            count += nameLen;

            this.skills.Clear();
            ushort skillLen = BitConverter.ToUInt16(s[count..]);
            count += sizeof(ushort);
            for (int i = 0; i < skillLen; i++)
            {
                var skill = new Skill();
                skill.Read(s, ref count);
                skills.Add(skill);
            }

        }

        public ArraySegment<byte> Write()
        {
            var segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            if (segment.Array != null)
            {
                var s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

                //Size만큼 미리 건너뛰기
                count += sizeof(ushort);

                success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.PlayerInfoReq);
                count += sizeof(ushort);

                segment.Array[segment.Offset + count] = (byte)this.testByte;
                count += sizeof(byte);

                success &= BitConverter.TryWriteBytes(s[count..], this.playerId);
                count += sizeof(long);

                ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, s[(count + sizeof(ushort))..]);
                success &= BitConverter.TryWriteBytes(s[count..], nameLen);
                count += sizeof(ushort);
                count += nameLen;

                success &= BitConverter.TryWriteBytes(s[count..], (ushort)this.skills.Count);
                count += sizeof(ushort);
                foreach (var skill in this.skills)
                    success &= skill.Write(s, ref count);

                success &= BitConverter.TryWriteBytes(s, count);   //size
            }

            if (!success)
                return default;

            return SendBufferHelper.Close(count);
        }
    }
    public class Test
    {
        public int testInt;

        public void Read(ArraySegment<byte> segment)
        {
            if (segment.Array == null)
                return;

            var s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            ushort count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);

            this.testInt = BitConverter.ToInt32(s[count..]);
            count += sizeof(int);

        }

        public ArraySegment<byte> Write()
        {
            var segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            if (segment.Array != null)
            {
                var s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

                //Size만큼 미리 건너뛰기
                count += sizeof(ushort);

                success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.Test);
                count += sizeof(ushort);

                success &= BitConverter.TryWriteBytes(s[count..], this.testInt);
                count += sizeof(int);

                success &= BitConverter.TryWriteBytes(s, count);   //size
            }

            if (!success)
                return default;

            return SendBufferHelper.Close(count);
        }
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

                        Console.WriteLine($"PlayerInfoReq: playerId: {p.playerId} Name: {p.name}");

                        foreach (var skill in p.skills)
                        {
                            Console.WriteLine($"SkillId: {skill.id} Level: {skill.level} Duration: {skill.duration}");
                        }
                    }
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
