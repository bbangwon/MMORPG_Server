namespace PacketGenerator
{
    class PacketFormat
    {

        // {0} : 패킷이름
        // {1} : 멤버 변수들
        // {2} : 멤버 변수 Read 부분
        // {3} : 멤버 변수 Write 부분

        public static string packetFormat =
@"
class {0}
{{
    {1}

    public struct SkillInfo
    {{
        public int id;
        public short level;
        public float duration;

        public bool Write(Span<byte> s, ref ushort count)
        {{
            bool success = true;

            success &= BitConverter.TryWriteBytes(s[count..], id);
            count += sizeof(int);

            success &= BitConverter.TryWriteBytes(s[count..], level);
            count += sizeof(short);

            success &= BitConverter.TryWriteBytes(s[count..], duration);
            count += sizeof(float);

            return success;
        }}

        public void Read(ReadOnlySpan<byte> s, ref ushort count)
        {{
            id = BitConverter.ToInt32(s[count..]);
            count += sizeof(int);

            level = BitConverter.ToInt16(s[count..]);
            count += sizeof(short);

            duration = BitConverter.ToSingle(s[count..]);
            count += sizeof(float);
        }}
    }}

    public List<SkillInfo> skills = new();

    public void Read(ArraySegment<byte> segment)
    {{
        if (segment.Array == null)
            return;

        var s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);

        {2}

    }}

    public ArraySegment<byte> Write()
    {{
        var segment = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool success = true;            

        if (segment.Array != null)
        {{
            var s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);    //Size만큼 미리 건너뛰기
            success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.{0});
            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s[count..], this.playerId);
            count += sizeof(long);

            {3}

            success &= BitConverter.TryWriteBytes(s, count);   //size
        }}         

        if (!success)
            return default;

        return SendBufferHelper.Close(count);
    }}
}}";

        // {0} : 멤버 변수 타입
        // {1} : 멤버 변수 이름
        public static string memberFormat =
@"public {0} {1};";

        // {0} : 멤버 변수 이름
        // {1} : To~ 변수 형식
        // {2} : 변수 형식
        public static string readFormat =
@"this.{0} = BitConverter.{1}(s[count..]);
count += sizeof({2});";

        // {0} 변수 이름
        public static string readStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(s[count..]);
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Len));
count += {0}Len;";

        // {0} 변수 이름
        // {1} 변수 형식
        public static string writeFormat =
@"success &= BitConverter.TryWriteBytes(s[count..], this.{0});
count += sizeof({1});";

        // {0} 변수 이름
        public static string writeStringFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, s[(count + sizeof(ushort))..]);
success &= BitConverter.TryWriteBytes(s[count..], {0}Len);
count += sizeof(ushort);
count += {0}Len;";

    }
}
