using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
{
    class ServerSession : Session
    {
        // unsafe구문을 이용한 코드
        //static unsafe void ToBytes(ArraySegment<byte> s, int offset, ulong value)
        //{
        //    fixed (byte* ptr = &s.Array![offset])
        //    {
        //        *(ulong*)ptr = value;
        //    }
        //}

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
            //C_PlayerInfoReq packet = new C_PlayerInfoReq() { 
            //    playerId = 1001,
            //    name = "ABCD"
            //};

            //packet.skills.Add(
            //    new C_PlayerInfoReq.Skill() 
            //    { 
            //        id = 101, 
            //        level = 1, 
            //        duration = 3.0f,
            //        attributes = new List<C_PlayerInfoReq.Skill.Attribute>()
            //        {
            //            new C_PlayerInfoReq.Skill.Attribute() { att = 77 },                        
            //        }                   
            //    });
            //packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 102, level = 2, duration = 5.0f });
            //packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 103, level = 3, duration = 7.0f });


            //ArraySegment<byte>? openSegment = packet.Write();
            //if(openSegment != null)
            //    Send(openSegment.Value);

            //for (int i = 0; i < 5; i++)
            {

                //Send
                //ArraySegment<byte> s = SendBufferHelper.Open(4096);

                //ushort count = 0;

                ////바로 직접 쓰는 방법
                //bool success = true;
                
                //count += 2;
                //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array!, s.Offset + count, s.Count - count), packet.packetId);
                //count += 2;
                //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array!, s.Offset + count, s.Count - count), packet.playerId);
                //count += 8;

                ////Size 채워주기
                //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array!, s.Offset, s.Count), count);

                //안정성을 위해 만들어졌으나.. 속도가 느려짐
                //byte[] size = BitConverter.GetBytes(packet.size);
                //byte[] packetId = BitConverter.GetBytes(packet.packetId);
                //byte[] playerId = BitConverter.GetBytes(packet.playerId);

                //Buffer.BlockCopy(size, 0, s.Array!, s.Offset + count, 2);
                //count += 2;

                //Buffer.BlockCopy(packetId, 0, s.Array!, s.Offset + count, 2);
                //count += 2;

                //Buffer.BlockCopy(playerId, 0, s.Array!, s.Offset + count, 8);
                //count += 8;

                //ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);

                //if(success)
                //    Send(sendBuff);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array!, buffer.Offset, buffer.Count).Trim();
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
