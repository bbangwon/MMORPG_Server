using ServerCore;

class PacketManager
{
    #region Singleton
    static PacketManager _instance = new();
    public static PacketManager Instance => _instance;
    public PacketManager()
    {
        Register();
    }
    #endregion
    readonly Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> onRecv = [];
    readonly Dictionary<ushort, Action<PacketSession, IPacket>> handler = [];

    public void Register()
    {
          
        onRecv.Add((ushort)PacketID.C_Chat, MakePacket<C_Chat>);
        handler.Add((ushort)PacketID.C_Chat, PacketHandler.C_ChatHandler);


    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        if (buffer.Array == null)
            return;

        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        if(onRecv.TryGetValue(id, out var action))
        {
            action.Invoke(session, buffer);
        }
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        var packet = new T();
        packet.Read(buffer);

        if(handler.TryGetValue(packet.Protocol, out var action))
        {
            action.Invoke(session, packet);
        }
    }
}
