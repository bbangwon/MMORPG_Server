using DummyClient;
using ServerCore;

class PacketHandler
{
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        if(packet is S_Chat chatPacket &&
                session is ServerSession serverSession)
        {
            //if(chatPacket.playerId == 1)
            //{
                //Console.WriteLine(chatPacket.chat);
            //}

        }
    }
}

