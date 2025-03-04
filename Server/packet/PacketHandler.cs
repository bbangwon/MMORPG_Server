using Server;
using ServerCore;

class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        if(session is ClientSession clientSession &&
            packet is C_LeaveGame leavePacket)
        {
            if (clientSession.Room == null)
                return;

            GameRoom room = clientSession.Room;
            room.Push(() => room.Leave(clientSession));
        }
    }

    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        if (session is ClientSession clientSession &&
            packet is C_Move movePacket)
        {
            if (clientSession.Room == null)
                return;

            Console.WriteLine($"{movePacket.posX} {movePacket.posY} {movePacket.posZ}");

            GameRoom room = clientSession.Room;
            room.Push(() => room.Move(clientSession, movePacket));
        }
    }
}

