using ServerCore;
using System;
using System.Text;

public enum PacketID
{
    S_BroadcastEnterGame = 1,
	C_LeaveGame = 2,
	S_BroadcastLeaveGame = 3,
	S_PlayerList = 4,
	C_Move = 5,
	S_BroadcastMove = 6,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


public class S_BroadcastEnterGame : IPacket
{
    public int playerId;
	public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol => (ushort)PacketID.S_BroadcastEnterGame;

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
		
		this.posX = BitConverter.ToSingle(s[count..]);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(s[count..]);
		count += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(s[count..]);
		count += sizeof(float);
		
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

            success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.S_BroadcastEnterGame);
            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s[count..], this.playerId);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s[count..], this.posX);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s[count..], this.posY);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s[count..], this.posZ);
			count += sizeof(float);
			
            success &= BitConverter.TryWriteBytes(s, count);   //size
        }         

        if (!success)
            return default;

        return SendBufferHelper.Close(count);
    }
}public class C_LeaveGame : IPacket
{
    

    public ushort Protocol => (ushort)PacketID.C_LeaveGame;

    public void Read(ArraySegment<byte> segment)
    {
        if (segment.Array == null)
            return;

        var s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);

        
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

            success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.C_LeaveGame);
            count += sizeof(ushort);

            
            success &= BitConverter.TryWriteBytes(s, count);   //size
        }         

        if (!success)
            return default;

        return SendBufferHelper.Close(count);
    }
}public class S_BroadcastLeaveGame : IPacket
{
    public int playerId;

    public ushort Protocol => (ushort)PacketID.S_BroadcastLeaveGame;

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

            success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.S_BroadcastLeaveGame);
            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s[count..], this.playerId);
			count += sizeof(int);
			
            success &= BitConverter.TryWriteBytes(s, count);   //size
        }         

        if (!success)
            return default;

        return SendBufferHelper.Close(count);
    }
}public class S_PlayerList : IPacket
{
    public class Player
	{
	    public bool isSelf;
		public int playerId;
		public float posX;
		public float posY;
		public float posZ;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        this.isSelf = BitConverter.ToBoolean(s[count..]);
			count += sizeof(bool);
			
			this.playerId = BitConverter.ToInt32(s[count..]);
			count += sizeof(int);
			
			this.posX = BitConverter.ToSingle(s[count..]);
			count += sizeof(float);
			
			this.posY = BitConverter.ToSingle(s[count..]);
			count += sizeof(float);
			
			this.posZ = BitConverter.ToSingle(s[count..]);
			count += sizeof(float);
			
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	
	        success &= BitConverter.TryWriteBytes(s[count..], this.isSelf);
			count += sizeof(bool);
			
			success &= BitConverter.TryWriteBytes(s[count..], this.playerId);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s[count..], this.posX);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s[count..], this.posY);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s[count..], this.posZ);
			count += sizeof(float);
			
	        return success;
	    }
	}
	
	public List<Player> players = [];

    public ushort Protocol => (ushort)PacketID.S_PlayerList;

    public void Read(ArraySegment<byte> segment)
    {
        if (segment.Array == null)
            return;

        var s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.players.Clear();
		ushort playerLen = BitConverter.ToUInt16(s[count..]);
		count += sizeof(ushort);
		for (int i = 0; i < playerLen; i++)
		{
		    var player = new Player();
		    player.Read(s, ref count);
		    players.Add(player);
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
            
            //Size만큼 미리 건너뛰기
            count += sizeof(ushort);    

            success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.S_PlayerList);
            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s[count..], (ushort)this.players.Count);
			count += sizeof(ushort);
			foreach (var player in this.players)
			    success &= player.Write(s, ref count);
			
            success &= BitConverter.TryWriteBytes(s, count);   //size
        }         

        if (!success)
            return default;

        return SendBufferHelper.Close(count);
    }
}public class C_Move : IPacket
{
    public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol => (ushort)PacketID.C_Move;

    public void Read(ArraySegment<byte> segment)
    {
        if (segment.Array == null)
            return;

        var s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.posX = BitConverter.ToSingle(s[count..]);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(s[count..]);
		count += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(s[count..]);
		count += sizeof(float);
		
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

            success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.C_Move);
            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s[count..], this.posX);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s[count..], this.posY);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s[count..], this.posZ);
			count += sizeof(float);
			
            success &= BitConverter.TryWriteBytes(s, count);   //size
        }         

        if (!success)
            return default;

        return SendBufferHelper.Close(count);
    }
}public class S_BroadcastMove : IPacket
{
    public int playerId;
	public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol => (ushort)PacketID.S_BroadcastMove;

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
		
		this.posX = BitConverter.ToSingle(s[count..]);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(s[count..]);
		count += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(s[count..]);
		count += sizeof(float);
		
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

            success &= BitConverter.TryWriteBytes(s[count..], (ushort)PacketID.S_BroadcastMove);
            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s[count..], this.playerId);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s[count..], this.posX);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s[count..], this.posY);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s[count..], this.posZ);
			count += sizeof(float);
			
            success &= BitConverter.TryWriteBytes(s, count);   //size
        }         

        if (!success)
            return default;

        return SendBufferHelper.Close(count);
    }
}
