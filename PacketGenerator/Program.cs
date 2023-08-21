using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,  //주석 무시
                IgnoreWhitespace = true //스페이스 무시
            };

            using XmlReader r = XmlReader.Create("PDL.xml", settings);
            r.MoveToContent();

            while (r.Read())
            {
                if (r.Depth == 1 && r.NodeType == XmlNodeType.Element)
                    ParsePacket(r);
                
                //Console.WriteLine(r.Name + " " + r["name"]);
            }           
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

            ParseMembers(r);
        }

        private static void ParseMembers(XmlReader r)
        {
            string packetName = r["name"]!;

            int depth = r.Depth + 1;
            while (r.Read())
            {
                if(r.Depth != depth)
                    break;

                string? memberName = r["name"];
                if(string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    return;
                }

                string memeberType = r.Name.ToLower();
                switch (memeberType)
                {
                    case "bool":
                        break;
                    case "byte":
                        break;
                    case "short":
                        break;
                    case "ushort":
                        break;  
                    case "int":
                        break;
                    case "long":
                        break;
                    case "float":
                        break;
                    case "double":
                        break;
                    case "string":
                        break;
                    case "list":
                        break;
                    default:
                        break;
                }

            }
        }
    }
}