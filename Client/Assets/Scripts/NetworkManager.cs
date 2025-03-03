using DummyClient;
using ServerCore;
using System;
using System.Collections;
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

        StartCoroutine(CoSendPacket());
    }
    
    void Update()
    {
        IPacket packet = PacketQueue.Instance.Pop();
        if (packet != null)
        {
            PacketManager.Instance.HandlePacket(session, packet);
        }
    }

    IEnumerator CoSendPacket()
    {
        while(true)
        {
            yield return new WaitForSeconds(3.0f);

            var chatPacket = new C_Chat
            {
                chat = "Hello Unity!"
            };
            ArraySegment<byte> bytes = chatPacket.Write();
            session.Send(bytes);
        }
    }

    private void OnDestroy()
    {
        session.Disconnect();
    }
}
