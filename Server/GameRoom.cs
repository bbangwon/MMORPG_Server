using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameRoom
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        object _lock = new object();

        public void Broadcast(ClientSession clientSession, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = clientSession.SessionId;
            packet.chat = $"{chat} ({clientSession.SessionId})";

            ArraySegment<byte>? segment = packet.Write();

            lock (_lock)
            {
                foreach (ClientSession s in _sessions)
                {
                    s.Send(segment!.Value);
                }
            }
        }

        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Add(session);
                session.Room = this;
            }
        }

        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);
            }
        }
    }
}
