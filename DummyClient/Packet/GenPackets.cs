using ServerCore;
using System;
using System.Text;

public enum PacketID
{
    C_Chat = 1,
	S_Chat = 2,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


public class C_Chat : IPacket
{
    public string chat = string.Empty;

    public ushort Protocol => (ushort)PacketID.C_Chat;

    public void Read(ArraySegment<byte> segment)
    {
        if (segment.Array == null)
            return;

        var s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);

        ushort chatLen = BitConverter.ToUInt16(s[count..]);
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
		count += chatLen;
		
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

            success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.C_Chat);
            count += sizeof(ushort);

            ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, s[(count + sizeof(ushort))..]);
			success &= BitConverter.TryWriteBytes(s[count..], chatLen);
			count += sizeof(ushort);
			count += chatLen;
			
            success &= BitConverter.TryWriteBytes(s, count);   //size
        }         

        if (!success)
            return default;

        return SendBufferHelper.Close(count);
    }
}public class S_Chat : IPacket
{
    public int playerId;
	public string chat = string.Empty;

    public ushort Protocol => (ushort)PacketID.S_Chat;

    public void Read(ArraySegment<byte> segment)
    {
        if (segment.Array == null)
            return;

        var s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.playerId = BitConverter.ToInt32(s[count..]);
		count += sizeof(int);
		
		ushort chatLen = BitConverter.ToUInt16(s[count..]);
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
		count += chatLen;
		
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

            success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.S_Chat);
            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s[count..], this.playerId);
			count += sizeof(int);
			
			ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, s[(count + sizeof(ushort))..]);
			success &= BitConverter.TryWriteBytes(s[count..], chatLen);
			count += sizeof(ushort);
			count += chatLen;
			
            success &= BitConverter.TryWriteBytes(s, count);   //size
        }         

        if (!success)
            return default;

        return SendBufferHelper.Close(count);
    }
}
