using DummyClient;
using ServerCore;
using UnityEngine;

class PacketHandler
{
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        if(packet is S_Chat chatPacket &&
                session is ServerSession serverSession)
        {

            //if(chatPacket.playerId == 1)
            {
                Debug.Log(chatPacket.chat);

                GameObject go = GameObject.Find("Player");
                if(go == null)
                    Debug.Log("Player not found");
                else
                    Debug.Log("Player found");

                //Console.WriteLine(chatPacket.chat);
            }

        }
    }
}

