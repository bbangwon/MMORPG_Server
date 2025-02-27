using ServerCore;

namespace Server
{
    public class GameRoom : IJobQueue
    {
        readonly List<ClientSession> sessions = [];
        readonly JobQueue jobQueue = new();
        readonly List<ArraySegment<byte>> pendingList = new();

        public void Broadcast(ClientSession clientSession, string chat)
        {
            var packet = new S_Chat
            {
                playerId = clientSession.SessionId,
                chat = $"{chat} I am {clientSession.SessionId}"
            };
            var segment = packet.Write();

            pendingList.Add(segment);
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

        public void Flush()
        {
            foreach (var session in sessions)
                session.Send(pendingList);

            Console.WriteLine($"Flushed {pendingList.Count} items");
            pendingList.Clear();
        }
    }
}
