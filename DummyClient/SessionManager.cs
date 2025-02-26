
namespace DummyClient
{
    public class SessionManager
    {
        static readonly SessionManager session = new();
        public static SessionManager Instance => session;

        readonly List<ServerSession> sessions = [];
        readonly Lock _lock = new ();

        public ServerSession Generate()
        {
            lock(_lock)
            {
                var session = new ServerSession();
                sessions.Add(session);
                return session;
            }
        }

        public void SendForEach()
        {
            lock(_lock)
            {
                foreach (var session in sessions)
                {
                    var chatPacket = new C_Chat
                    {
                        chat = $"Hello, Server!"
                    };

                    var segment = chatPacket.Write();
                    session.Send(segment);
                }
            }
        }
    }
}
