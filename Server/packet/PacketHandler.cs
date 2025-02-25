using ServerCore;

class PacketHandler
{
    public static void C_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        if(packet is C_PlayerInfoReq p)
        {
            Console.WriteLine($"PlayerInfoReq: playerId: {p.playerId} Name: {p.name}");

            foreach (var skill in p.skills)
            {
                Console.WriteLine($"SkillId: {skill.id} Level: {skill.level} Duration: {skill.duration}");
            }
        }
    }
}

