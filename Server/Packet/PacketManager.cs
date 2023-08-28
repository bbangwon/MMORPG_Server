using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class PacketManager
    {
        #region Singleton
        static PacketManager? _instance;
        public static PacketManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PacketManager();
                return _instance;
            }
        } 
        #endregion

        Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
        Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

        public void Register()
        {
            _onRecv.Add((ushort)PacketID.PlayerInfoReq, MakePacket<PlayerInfoReq>);
            _handler.Add((ushort)PacketID.PlayerInfoReq, PacketHandler.PlayerInfoReqHandler);
        }

        public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
        {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array!, buffer.Offset);
            count += sizeof(ushort);
            ushort id = BitConverter.ToUInt16(buffer.Array!, buffer.Offset + count);
            count += sizeof(ushort);

            Console.WriteLine($"[Packet Received] {(PacketID)id}");
            Action<PacketSession, ArraySegment<byte>>? action;
            if (_onRecv.TryGetValue(id, out action))
            {
                action.Invoke(session, buffer);
            }
        }

        void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
        {
            T pkt = new T();
            pkt.Read(buffer);

            Action<PacketSession, IPacket>? action = null;
            if (_handler.TryGetValue(pkt.Protocol, out action))
            {
                action.Invoke(session, pkt);
            }
        }
    }
}
