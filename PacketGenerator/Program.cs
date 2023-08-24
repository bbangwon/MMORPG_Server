using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static string? genPacket;
        static short packetId;
        static string? packetEnums;

        static void Main(string[] args)
        {
            string pdlPath = "../PDL.xml";

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,  //주석 무시
                IgnoreWhitespace = true //스페이스 무시
            };

            if(args.Length >= 1)
                pdlPath = args[0];

            using XmlReader r = XmlReader.Create(pdlPath, settings);
            r.MoveToContent();

            while (r.Read())
            {
                if (r.Depth == 1 && r.NodeType == XmlNodeType.Element)
                    ParsePacket(r);
                
                //Console.WriteLine(r.Name + " " + r["name"]);
            }

            string fileText = string.Format(PacketFormat.fileFormat, packetEnums, genPacket);
            File.WriteAllText("GenPackets.cs", fileText);
        }

        private static void ParsePacket(XmlReader r)
        {
            if(r.NodeType == XmlNodeType.EndElement)
                return;

            if(r.Name.ToLower() != "packet")
            {
                Console.WriteLine("Invalid packet node");
                return;
            }

            string? packetName = r["name"];
            if(string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet without name");
                return;
            }

            var t = ParseMembers(r);
            if(t == null)
                return;

            genPacket += string.Format(PacketFormat.packetFormat, 
                packetName, t.Item1, t.Item2, t.Item3);
            packetEnums += string.Format(PacketFormat.packetEnumFormat, 
                               packetName, ++packetId) + Environment.NewLine + "\t";
        }

        // {1} : 멤버 변수들
        // {2} : 멤버 변수 읽는 부분
        // {3} : 멤버 변수 쓰는 부분
        private static Tuple<string, string, string>? ParseMembers(XmlReader r)
        {
            string packetName = r["name"]!;

            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            int depth = r.Depth + 1;
            while (r.Read())
            {
                if(r.Depth != depth)
                    break;

                string? memberName = r["name"];
                if(string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    return null;
                }

                if(string.IsNullOrEmpty(memberCode) == false)
                    memberCode += Environment.NewLine;
                if(string.IsNullOrEmpty(readCode) == false)
                    readCode += Environment.NewLine;
                if(string.IsNullOrEmpty(writeCode) == false)
                    writeCode += Environment.NewLine;

                string memeberType = r.Name.ToLower();
                switch (memeberType)
                {
                    case "byte":
                    case "sbyte":
                        memberCode += string.Format(PacketFormat.memberFormat, memeberType, memberName);
                        readCode += string.Format(PacketFormat.readByteFormat, memberName, memeberType);
                        writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memeberType);
                        break;
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        memberCode += string.Format(PacketFormat.memberFormat, memeberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memeberType), memeberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memeberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memeberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                        var t = ParseList(r);
                        memberCode += t.Item1;
                        readCode += t.Item2;
                        writeCode += t.Item3;
                        break;
                    default:
                        break;
                }
            }

            memberCode = memberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        private static Tuple<string, string, string>? ParseList(XmlReader r)
        {
            string? listName = r["name"];
            if(string.IsNullOrEmpty(listName))
            {
                Console.WriteLine("List without name");
                return null;
            }

            var t = ParseMembers(r);

            string memberCode = string.Format(PacketFormat.memberListFormat, 
                FirstCharToUpper(listName), 
                FirstCharToLower(listName),
                t.Item1,
                t.Item2,
                t.Item3);

            string readCode = string.Format(PacketFormat.readListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            string writeCode = string.Format(PacketFormat.writeListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static string ToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "bool":
                    return "ToBoolean";
                case "short":
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "int":
                    return "ToInt32";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                default:
                    return "";
            }
        }

        public static string FirstCharToUpper(string input)
        {
            if(string.IsNullOrEmpty(input))
                return "";

            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        public static string FirstCharToLower(string input)
        {
            if(string.IsNullOrEmpty(input))
                return "";

            return input.First().ToString().ToLower() + input.Substring(1);
        }
    }
}