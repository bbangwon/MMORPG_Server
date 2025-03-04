
namespace DummyClient
{
    public class SessionManager
    {
        static readonly SessionManager session = new();
        public static SessionManager Instance => session;

        readonly List<ServerSession> sessions = [];
        readonly Lock _lock = new ();
        Random rand = new();

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
                    var movePacket = new C_Move
                    {
                        posX = rand.Next(-50, 50),
                        posY = 0,
                        posZ = rand.Next(-50, 50)
                    };
                    session.Send(movePacket.Write());
                }
            }
        }
    }
}
