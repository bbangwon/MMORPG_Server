using ServerCore;

namespace Server
{
    public class GameRoom : IJobQueue
    {
        readonly List<ClientSession> sessions = [];
        readonly JobQueue jobQueue = new();
        readonly List<ArraySegment<byte>> pendingList = new();

        public void Broadcast(ArraySegment<byte> segment)
        {
            pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            sessions.Add(session);
            session.Room = this;

            //신입생한테 모든 플레이어 목록 전송
            var players = new S_PlayerList
            {
                players = [.. sessions.Select(s => new S_PlayerList.Player
                {
                    isSelf = (s == session),
                    playerId = s.SessionId,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ
                })]
            };
            session.Send(players.Write());

            //신입생 입장을 모두에게 알림
            var enter = new S_BroadcastEnterGame
            {
                playerId = session.SessionId,
                posX = 0,
                posY = 0,
                posZ = 0
            };
            Broadcast(enter.Write());
        }

        public void Leave(ClientSession session)
        {
            sessions.Remove(session);

            //퇴장을 모두에게 알림
            var leave = new S_BroadcastLeaveGame
            {
                playerId = session.SessionId
            };
            Broadcast(leave.Write());
        }

        public void Push(Action job)
        {
            jobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (var session in sessions)
                session.Send(pendingList);

            //Console.WriteLine($"Flushed {pendingList.Count} items");
            pendingList.Clear();
        }

        public void Move(ClientSession clientSession, C_Move packet)
        {
            clientSession.PosX = packet.posX;
            clientSession.PosY = packet.posY;
            clientSession.PosZ = packet.posZ;

            var movePacket = new S_BroadcastMove
            {
                playerId = clientSession.SessionId,
                posX = packet.posX,
                posY = packet.posY,
                posZ = packet.posZ
            };
            Broadcast(movePacket.Write());
        }
    }
}
