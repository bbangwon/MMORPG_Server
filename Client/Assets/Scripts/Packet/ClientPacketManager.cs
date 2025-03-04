using ServerCore;
using System.Collections.Generic;
using System;

public class PacketManager
{
    #region Singleton
    static PacketManager _instance = new();
    public static PacketManager Instance => _instance;
    public PacketManager()
    {
        Register();
    }
    #endregion
    readonly Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> makeFunc = new();
    readonly Dictionary<ushort, Action<PacketSession, IPacket>> handler = new();

    public void Register()
    {
          
        makeFunc.Add((ushort)PacketID.S_BroadcastEnterGame, MakePacket<S_BroadcastEnterGame>);
        handler.Add((ushort)PacketID.S_BroadcastEnterGame, PacketHandler.S_BroadcastEnterGameHandler);

      
        makeFunc.Add((ushort)PacketID.S_BroadcastLeaveGame, MakePacket<S_BroadcastLeaveGame>);
        handler.Add((ushort)PacketID.S_BroadcastLeaveGame, PacketHandler.S_BroadcastLeaveGameHandler);

      
        makeFunc.Add((ushort)PacketID.S_PlayerList, MakePacket<S_PlayerList>);
        handler.Add((ushort)PacketID.S_PlayerList, PacketHandler.S_PlayerListHandler);

      
        makeFunc.Add((ushort)PacketID.S_BroadcastMove, MakePacket<S_BroadcastMove>);
        handler.Add((ushort)PacketID.S_BroadcastMove, PacketHandler.S_BroadcastMoveHandler);


    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
    {
        if (buffer.Array == null)
            return;

        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        if(makeFunc.TryGetValue(id, out var func))
        {
            IPacket packet = func.Invoke(session, buffer);
            if(onRecvCallback != null)
                onRecvCallback.Invoke(session, packet);
            else
                HandlePacket(session, packet);
        }
    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        var packet = new T();
        packet.Read(buffer);

        return packet;
    }

    public void HandlePacket(PacketSession session, IPacket packet)
    {
        if (handler.TryGetValue(packet.Protocol, out var action))
        {
            action.Invoke(session, packet);
        }
    }
}
