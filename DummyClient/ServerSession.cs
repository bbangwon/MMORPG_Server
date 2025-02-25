using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
{
    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            var packet = new PlayerInfoReq { playerId = 1001, name = "ABCD" };
            
            var skill = new PlayerInfoReq.Skill { id = 101, level = 1, duration = 3.0f };
            skill.attributes.Add(new PlayerInfoReq.Skill.Attribute { att = 77 });
            packet.skills.Add(skill);

            packet.skills.Add(new PlayerInfoReq.Skill { id = 201, level = 1, duration = 4.0f });
            packet.skills.Add(new PlayerInfoReq.Skill { id = 301, level = 1, duration = 5.0f });
            packet.skills.Add(new PlayerInfoReq.Skill { id = 401, level = 1, duration = 6.0f });


            //for (int i = 0; i < 5; i++)
            {
                var s = packet.Write();
                if(s != default)
                    Send(s);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array!, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }

}
