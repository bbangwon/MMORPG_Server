// See https://aka.ms/new-console-template for more information

using PacketGenerator;
using System.Xml;

string genPackets = string.Empty;
ushort packetId = 0;
string packetEnums = string.Empty;

var settings = new XmlReaderSettings()
{
    IgnoreComments = true,
    IgnoreWhitespace = true
};

using XmlReader r = XmlReader.Create("PDL.xml", settings);
r.MoveToContent();

while (r.Read())
{
    if(r.Depth == 1 && r.NodeType == XmlNodeType.Element)
    {
        ParsePacket(r);
    }
}

string fileText = string.Format(PacketFormat.fileFormat, packetEnums, genPackets);
File.WriteAllText("GenPackets.cs", fileText);

void ParsePacket(XmlReader r)
{
    if (r.NodeType == XmlNodeType.EndElement)
        return;

    if (r.Name.ToLower() != "packet")
        return;

    string? packetName = r["name"];
    if (string.IsNullOrEmpty(packetName))
    {
        Console.WriteLine("Packet without name");
        return; 
    }

    var t = ParseMembers(r);
    genPackets += string.Format(
        PacketFormat.packetFormat, 
        packetName,
        t.Item1,
        t.Item2,
        t.Item3
        );

    packetEnums += string.Format(PacketFormat.packetEnumFormat, packetName, ++packetId);
    packetEnums += Environment.NewLine;
    packetEnums += "\t";
}

// {1} : 멤버 변수들
// {2} : 멤버 변수 Read 부분
// {3} : 멤버 변수 Write 부분
(string, string, string) ParseMembers(XmlReader r)
{
    string? packletName = r["name"];

    string memberCode = string.Empty;
    string readCode = string.Empty;
    string writeCode = string.Empty;

    int depth = r.Depth + 1;
    while(r.Read())
    {
        if (r.Depth != depth)
            break;

        string? memberName = r["name"];
        if(string.IsNullOrEmpty(memberName))
        {
            Console.WriteLine("Member without name");
            return default;
        }

        if(!string.IsNullOrEmpty(memberCode))
            memberCode += Environment.NewLine;
        if (!string.IsNullOrEmpty(readCode))
            readCode += Environment.NewLine;
        if (!string.IsNullOrEmpty(writeCode))
            writeCode += Environment.NewLine;

        string memberType = r.Name.ToLower();
        switch(memberType)
        {
            case "byte":
            case "sbyte":
                memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType);
                writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                break;
            case "bool":
            case "short":
            case "ushort":
            case "int":
            case "long":
            case "float":
            case "double":
                memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                break;
            case "string":
                memberCode += string.Format(PacketFormat.stringMemberFormat, memberType, memberName);
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
    writeCode = writeCode.Replace("\n", "\n\t\t\t");
    return (memberCode, readCode, writeCode);
}

(string, string, string) ParseList(XmlReader r)
{
    string? listName = r["name"];
    if (string.IsNullOrEmpty(listName))
    {
        Console.WriteLine("List without name");
        return default;
    }

    var t = ParseMembers(r);
    t.Item3 = t.Item3.Replace("\n\t\t\t", "\n\t\t");

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

    return (memberCode, readCode, writeCode);
}

string ToMemberType(string memberType)
{
    return memberType switch
    {
        "bool" => "ToBoolean",
        "short" => "ToInt16",
        "ushort" => "ToUInt16",
        "int" => "ToInt32",
        "long" => "ToInt64",
        "float" => "ToSingle",
        "double" => "ToDouble",
        _ => string.Empty
    };
}

string FirstCharToUpper(string input)
{
    if(string.IsNullOrEmpty(input))
        return string.Empty;

    return input[0].ToString().ToUpper() + input[1..];
}

string FirstCharToLower(string input)
{
    if (string.IsNullOrEmpty(input))
        return string.Empty;

    return input[0].ToString().ToLower() + input[1..];
}

