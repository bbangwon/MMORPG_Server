using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
{
    class PlayerInfoReq
    {
        public long playerId;
        public string name = string.Empty;

        public struct SkillInfo
        {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte> s, ref ushort count)
            {
                bool success = true;

                success &= BitConverter.TryWriteBytes(s[count..], id);
                count += sizeof(int);

                success &= BitConverter.TryWriteBytes(s[count..], level);
                count += sizeof(short);

                success &= BitConverter.TryWriteBytes(s[count..], duration);
                count += sizeof(float);

                return success;
            }

            public void Read(ReadOnlySpan<byte> s, ref ushort count)
            {
                id = BitConverter.ToInt32(s[count..]);
                count += sizeof(int);

                level = BitConverter.ToInt16(s[count..]);
                count += sizeof(short);

                duration = BitConverter.ToSingle(s[count..]);
                count += sizeof(float);
            }
        }

        public List<SkillInfo> skills = new();

        public void Read(ArraySegment<byte> segment)
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

            skills.Clear();
            ushort skillLen = BitConverter.ToUInt16(s[count..]);
            count += sizeof(ushort);

            for (int i = 0; i < skillLen; i++)
            {
                SkillInfo skill = new();
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

                count += sizeof(ushort);    //Size만큼 미리 건너뛰기

                success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.PlayerInfoReq);
                count += sizeof(ushort);

                success &= BitConverter.TryWriteBytes(s[count..], this.playerId);
                count += sizeof(long);

                ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, s[(count + sizeof(ushort))..]);
                success &= BitConverter.TryWriteBytes(s[count..], nameLen);
                count += sizeof(ushort);
                count += nameLen;

                //skill list
                success &= BitConverter.TryWriteBytes(s[count..], (ushort)skills.Count);
                count += sizeof(ushort);
                foreach (var skillinfo in skills)
                    success &= skillinfo.Write(s, ref count);

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

    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            var packet = new PlayerInfoReq { playerId = 1001, name = "ABCD" };
            packet.skills.Add(new PlayerInfoReq.SkillInfo { id = 101, level = 1, duration = 3.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo { id = 201, level = 1, duration = 4.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo { id = 301, level = 1, duration = 5.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo { id = 401, level = 1, duration = 6.0f });


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
