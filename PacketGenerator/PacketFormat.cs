namespace PacketGenerator
{
    class PacketFormat
    {
        // {0} : 패킷 이름/번호 목록
        // {1} : 패킷 목록
        public static string fileFormat =
@"using ServerCore;
using System.Text;

public enum PacketID
{{
    {0}
}}

{1}
";
        // {0} : 패킷 이름
        // {1} : 패킷 번호
        public static string packetEnumFormat = 
@"{0} = {1},";

        // {0} : 패킷이름
        // {1} : 멤버 변수들
        // {2} : 멤버 변수 Read 부분
        // {3} : 멤버 변수 Write 부분

        public static string packetFormat =
@"public class {0}
{{
    {1}

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
            
            //Size만큼 미리 건너뛰기
            count += sizeof(ushort);    

            success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.{0});
            count += sizeof(ushort);

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

        // {0} : 멤버 변수 타입
        // {1} : 멤버 변수 이름
        public static string stringMemberFormat =
@"public {0} {1} = string.Empty;";

        //{0} : 리스트 이름 [대문자]
        //{1} : 리스트 이름 [소문자]
        //{2} : 멤버 변수들
        //{3} : 멤버 변수 Read 부분
        //{4} : 멤버 변수 Write 부분
        public static string memberListFormat =
@"public class {0}
{{
    {2}

    public void Read(ReadOnlySpan<byte> s, ref ushort count)
    {{
        {3}
    }}

    public bool Write(Span<byte> s, ref ushort count)
    {{
        bool success = true;

        {4}
        return success;
    }}
}}

public List<{0}> {1}s = [];";

        // {0} : 변수 이름
        // {1} : To~ 변수 형식
        // {2} : 변수 형식
        public static string readFormat =
@"this.{0} = BitConverter.{1}(s[count..]);
count += sizeof({2});
";

        // {0} : 변수 이름
        // {1} : 변수 형식
        public static string readByteFormat =
@"this.{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});
";


        // {0} 변수 이름
        public static string readStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(s[count..]);
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Len));
count += {0}Len;
";

        //{0} : 리스트 이름 [대문자]
        //{1} : 리스트 이름 [소문자]
        public static string readListFormat =
@"this.{1}s.Clear();
ushort {1}Len = BitConverter.ToUInt16(s[count..]);
count += sizeof(ushort);
for (int i = 0; i < {1}Len; i++)
{{
    var {1} = new {0}();
    {1}.Read(s, ref count);
    {1}s.Add({1});
}}
";

        // {0} 변수 이름
        // {1} 변수 형식
        public static string writeFormat =
@"success &= BitConverter.TryWriteBytes(s[count..], this.{0});
count += sizeof({1});
";
        // {0} : 변수 이름
        // {1} : 변수 형식
        public static string writeByteFormat =
@"segment.Array[segment.Offset + count] = (byte)this.{0};
count += sizeof({1});
";

        // {0} 변수 이름
        public static string writeStringFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, s[(count + sizeof(ushort))..]);
success &= BitConverter.TryWriteBytes(s[count..], {0}Len);
count += sizeof(ushort);
count += {0}Len;
";

        //{0} : 리스트 이름 [대문자]
        //{1} : 리스트 이름 [소문자]
        public static string writeListFormat =
@"success &= BitConverter.TryWriteBytes(s[count..], (ushort)this.{1}s.Count);
count += sizeof(ushort);
foreach (var {1} in this.{1}s)
    success &= {1}.Write(s, ref count);
";

    }
}
