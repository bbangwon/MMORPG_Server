using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class PacketHandler
    {
        public static void PlayerInfoReqHandler(PacketSession session, IPacket packet)
        {
            if (packet is not PlayerInfoReq p)
            {
                Console.WriteLine("PlayerInfoReq cast failed");
                return;
            }

            Console.WriteLine($"PlayerInfoReq {p.playerId} {p.name}");

            foreach (PlayerInfoReq.Skill skill in p.skills)
            {
                Console.WriteLine($"Skill Id({skill.id}) Level({skill.level}) Duration({skill.duration})");
            }
        }
    }
}
