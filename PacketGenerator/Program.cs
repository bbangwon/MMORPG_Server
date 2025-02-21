// See https://aka.ms/new-console-template for more information

using System.Xml;

XmlReaderSettings settings = new XmlReaderSettings()
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
        Console.WriteLine("Invalid packet node");
        ParsePacket(r);
    }
}

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

    ParseMembers(r);

}

void ParseMembers(XmlReader r)
{
    string? packletName = r["name"];

    int depth = r.Depth + 1;
    while(r.Read())
    {
        if (r.Depth != depth)
            break;

        string? memeberName = r["name"];
        if(string.IsNullOrEmpty(memeberName))
        {
            Console.WriteLine("Member without name");
            return;
        }

        string memeberType = r.Name.ToLower();
        switch(memeberType)
        {
            case "bool":
            case "byte":
            case "short":
            case "ushort":
            case "int":
            case "long":
            case "float":
            case "double":
            case "string":
            case "list":
                break;
            default:
                break;
        }  
    }   
}