using DummyClient;
using ServerCore;
using UnityEngine;

class PacketHandler
{
    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        if (session is ServerSession serverSession &&
            packet is S_BroadcastEnterGame broadcastEnterGame)
        {
            PlayerManager.Instance.EnterGame(broadcastEnterGame);
        }
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        if (session is ServerSession serverSession &&
            packet is S_BroadcastLeaveGame broadcastLeaveGame)
        {
            PlayerManager.Instance.LeaveGame(broadcastLeaveGame);
        }
    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        if (session is ServerSession serverSession &&
            packet is S_PlayerList playerList)
        {
            PlayerManager.Instance.Add(playerList);
        }
    }


    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        if (session is ServerSession serverSession &&
            packet is S_BroadcastMove broadcastMove)
        {
            PlayerManager.Instance.Move(broadcastMove);
        }
    }
}

