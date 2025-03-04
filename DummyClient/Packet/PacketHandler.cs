using DummyClient;
using ServerCore;

class PacketHandler
{
    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        if(session is ServerSession serverSession &&
            packet is S_BroadcastEnterGame broadcastEnterGame)
        {

        }
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        if (session is ServerSession serverSession &&
            packet is S_BroadcastLeaveGame broadcastLeaveGame)
        {

        }
    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        if (session is ServerSession serverSession &&
            packet is S_PlayerList playerList)
        {

        }
    }


    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        if (session is ServerSession serverSession &&
            packet is S_BroadcastMove broadcastMove)
        {

        }
    }
}

