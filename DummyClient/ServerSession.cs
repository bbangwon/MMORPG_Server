using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
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






    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            var packet = new PlayerInfoReq { playerId = 1001, name = "ABCD" };
            
            var skill = new PlayerInfoReq.Skill { id = 101, level = 1, duration = 3.0f };
            skill.attributes.Add(new PlayerInfoReq.Skill.Attribute { att = 77 });
            packet.skills.Add(skill);

            packet.skills.Add(new PlayerInfoReq.Skill { id = 201, level = 1, duration = 4.0f });
            packet.skills.Add(new PlayerInfoReq.Skill { id = 301, level = 1, duration = 5.0f });
            packet.skills.Add(new PlayerInfoReq.Skill { id = 401, level = 1, duration = 6.0f });


            //for (int i = 0; i < 5; i++)
            {
                var s = packet.Write();
                if(s != default)
                    Send(s);
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
