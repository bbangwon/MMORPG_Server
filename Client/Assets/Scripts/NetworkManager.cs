using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession session = new();
    
    void Start()
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new(ipAddr, 7777);

        Connector connector = new();
        connector.Connect(endPoint, () => { return session; }, 1);
    }
    
    void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in list)
            PacketManager.Instance.HandlePacket(session, packet);
    }

    private void OnDestroy()
    {
        session.Disconnect();
    }

    public void Send(ArraySegment<byte> sendBuff)
    {
        session.Send(sendBuff);
    }
}
