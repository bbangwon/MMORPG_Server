using ServerCore;
using System.Collections.Generic;
using System;

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
    readonly Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> onRecv = new();
    readonly Dictionary<ushort, Action<PacketSession, IPacket>> handler = new();

    public void Register()
    {
          
        onRecv.Add((ushort)PacketID.S_Chat, MakePacket<S_Chat>);
        handler.Add((ushort)PacketID.S_Chat, PacketHandler.S_ChatHandler);


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
