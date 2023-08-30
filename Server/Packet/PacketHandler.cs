using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void C_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        if (packet is not C_PlayerInfoReq p)
        {
            Console.WriteLine("PlayerInfoReq cast failed");
            return;
        }

        Console.WriteLine($"PlayerInfoReq {p.playerId} {p.name}");

        foreach (C_PlayerInfoReq.Skill skill in p.skills)
        {
            Console.WriteLine($"Skill Id({skill.id}) Level({skill.level}) Duration({skill.duration})");
        }
    }
}
