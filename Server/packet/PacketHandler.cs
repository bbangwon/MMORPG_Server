using Server;
using ServerCore;

class PacketHandler
{
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        if(packet is C_Chat chatPacket && 
            session is ClientSession clientSession)
        {
            if (clientSession.Room == null)
                return;

            GameRoom room = clientSession.Room;
            room.Push(() =>
                room.Broadcast(clientSession, chatPacket.chat)
            );            
        }
    }
}

