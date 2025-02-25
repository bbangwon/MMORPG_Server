using ServerCore;

namespace Server
{
    class PacketHandler
    {
        public static void PlayerInfoReqHandler(PacketSession session, IPacket packet)
        {
            if(packet is PlayerInfoReq p)
            {
                Console.WriteLine($"PlayerInfoReq: playerId: {p.playerId} Name: {p.name}");

                foreach (var skill in p.skills)
                {
                    Console.WriteLine($"SkillId: {skill.id} Level: {skill.level} Duration: {skill.duration}");
                }
            }
        }
    }
}
