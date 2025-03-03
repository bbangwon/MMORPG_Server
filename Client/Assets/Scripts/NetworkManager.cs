using DummyClient;
using ServerCore;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession session = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new(ipAddr, 7777);

        Connector connector = new();
        connector.Connect(endPoint, () => { return session; }, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        session.Disconnect();
    }
}
