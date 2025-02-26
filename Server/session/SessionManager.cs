namespace Server
{
    class SessionManager
    {
        static SessionManager? instance;
        public static SessionManager Instance
        {
            get
            {
                instance ??= new SessionManager();
                return instance;
            }
        }

        int sessionId = 0;
        readonly Dictionary<int, ClientSession> sessions = [];
        readonly Lock _lock = new();

        public ClientSession Generate()
        {
            lock(_lock)
            {
                int sessionId = ++this.sessionId;

                var session = new ClientSession
                {
                    SessionId = sessionId
                };
                sessions.Add(sessionId, session);

                Console.WriteLine($"Connected : {sessionId}");

                return session;
            }
        }

        public ClientSession? Find(int id)
        {
            lock(_lock)
            {
                sessions.TryGetValue(id, out ClientSession? session);
                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                sessions.Remove(session.SessionId);
            }
        }
    }
}
