using ServerCore;
using System.Net;
using System.Text;

public enum PacketID
{
    C_PlayerInfoReq = 1,
	S_Test = 2,
	
}

interface IPacket
{	
    ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte>? Write();
}


class C_PlayerInfoReq : IPacket
{
    public byte testByte;
	public long playerId;
	public string name;
	
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
		        att = BitConverter.ToInt32(s[count..]);
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
		public List<Attribute> attributes = new List<Attribute>();
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        id = BitConverter.ToInt32(s[count..]);
			count += sizeof(int);
			level = BitConverter.ToInt16(s[count..]);
			count += sizeof(short);
			duration = BitConverter.ToSingle(s[count..]);
			count += sizeof(float);
			this.attributes.Clear();
			ushort attributeLen = BitConverter.ToUInt16(s[count..]);
			count += sizeof(ushort);
			
			for (int i = 0; i < attributeLen; i++)
			{
			    Attribute attribute = new Attribute();
			    attribute.Read(s, ref count);
			    this.attributes.Add(attribute);
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
			foreach (Attribute attribute in this.attributes)
			{
			    success &= attribute.Write(s, ref count);
			}
	        return success;
	    }
	}
	public List<Skill> skills = new List<Skill>();  
    
    public ushort Protocol { get { return (ushort)PacketID.C_PlayerInfoReq; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array!, segment.Offset, segment.Count);
            
        count += sizeof(ushort);
        count += sizeof(ushort);
        testByte = (byte)s[count];
		count += sizeof(byte);
		playerId = BitConverter.ToInt64(s[count..]);
		count += sizeof(long);
		ushort nameLen = BitConverter.ToUInt16(s[count..]);
		count += sizeof(ushort);
		name = Encoding.Unicode.GetString(s[count..(count + nameLen)]);
		count += nameLen;
		this.skills.Clear();
		ushort skillLen = BitConverter.ToUInt16(s[count..]);
		count += sizeof(ushort);
		
		for (int i = 0; i < skillLen; i++)
		{
		    Skill skill = new Skill();
		    skill.Read(s, ref count);
		    this.skills.Add(skill);
		}
    }

    public ArraySegment<byte>? Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array!, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.C_PlayerInfoReq);
        count += sizeof(ushort);
        s[count] = (byte)this.testByte;
		count += sizeof(byte);
		success &= BitConverter.TryWriteBytes(s[count..], this.playerId);
		count += sizeof(long);
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array!, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s[count..], nameLen);
		count += sizeof(ushort);
		count += nameLen;
		success &= BitConverter.TryWriteBytes(s[count..], (ushort)this.skills.Count);
		count += sizeof(ushort);
		foreach (Skill skill in this.skills)
		{
		    success &= skill.Write(s, ref count);
		}
        success &= BitConverter.TryWriteBytes(s, count);
        if(!success)
            return null;
        return SendBufferHelper.Close(count);
    }
}
class S_Test : IPacket
{
    public int testInt;  
    
    public ushort Protocol { get { return (ushort)PacketID.S_Test; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array!, segment.Offset, segment.Count);
            
        count += sizeof(ushort);
        count += sizeof(ushort);
        testInt = BitConverter.ToInt32(s[count..]);
		count += sizeof(int);
    }

    public ArraySegment<byte>? Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array!, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.S_Test);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s[count..], this.testInt);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);
        if(!success)
            return null;
        return SendBufferHelper.Close(count);
    }
}
