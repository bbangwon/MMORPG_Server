using ServerCore;

namespace Server
{
    public class GameRoom : IJobQueue
    {
        readonly List<ClientSession> sessions = [];
        readonly Lock _lock = new();
        readonly JobQueue jobQueue = new();

        public void Broadcast(ClientSession clientSession, string chat)
        {
            var packet = new S_Chat
            {
                playerId = clientSession.SessionId,
                chat = $"{chat} I am {clientSession.SessionId}"
            };
            var segment = packet.Write();

            foreach (var session in sessions)
                session.Send(segment);
        }

        public void Enter(ClientSession session)
        {
            sessions.Add(session);
            session.Room = this; 
        }

        public void Leave(ClientSession session)
        {
            sessions.Remove(session); 
        }

        public void Push(Action job)
        {
            jobQueue.Push(job);
        }
    }
}
