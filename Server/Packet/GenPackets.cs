using ServerCore;
using System.Net;
using System.Text;

public enum PacketID
{
    C_Chat = 1,
	S_Chat = 2,
	
}

interface IPacket
{	
    ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte>? Write();
}


class C_Chat : IPacket
{
    public string chat;  
    
    public ushort Protocol { get { return (ushort)PacketID.C_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array!, segment.Offset, segment.Count);
            
        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort chatLen = BitConverter.ToUInt16(s[count..]);
		count += sizeof(ushort);
		chat = Encoding.Unicode.GetString(s[count..(count + chatLen)]);
		count += chatLen;
    }

    public ArraySegment<byte>? Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array!, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.C_Chat);
        count += sizeof(ushort);
        ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array!, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s[count..], chatLen);
		count += sizeof(ushort);
		count += chatLen;
        success &= BitConverter.TryWriteBytes(s, count);
        if(!success)
            return null;
        return SendBufferHelper.Close(count);
    }
}
class S_Chat : IPacket
{
    public int playerId;
	public string chat;  
    
    public ushort Protocol { get { return (ushort)PacketID.S_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array!, segment.Offset, segment.Count);
            
        count += sizeof(ushort);
        count += sizeof(ushort);
        playerId = BitConverter.ToInt32(s[count..]);
		count += sizeof(int);
		ushort chatLen = BitConverter.ToUInt16(s[count..]);
		count += sizeof(ushort);
		chat = Encoding.Unicode.GetString(s[count..(count + chatLen)]);
		count += chatLen;
    }

    public ArraySegment<byte>? Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array!, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.S_Chat);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s[count..], this.playerId);
		count += sizeof(int);
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array!, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s[count..], chatLen);
		count += sizeof(ushort);
		count += chatLen;
        success &= BitConverter.TryWriteBytes(s, count);
        if(!success)
            return null;
        return SendBufferHelper.Close(count);
    }
}
