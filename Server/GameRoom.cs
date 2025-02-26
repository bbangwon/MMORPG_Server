namespace Server
{
    public class GameRoom
    {
        readonly List<ClientSession> sessions = [];
        readonly Lock _lock = new();

        public void Broadcast(ClientSession clientSession, string chat)
        {
            var packet = new S_Chat
            {
                playerId = clientSession.SessionId,
                chat = chat
            };
            var segment = packet.Write();

            lock (_lock)
            {
                foreach (var session in sessions)
                {
                    session.Send(segment);
                }
            }
        }

        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                sessions.Add(session);
                session.Room = this; 
            }
        }

        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                sessions.Remove(session); 
            }
        }
    }
}
