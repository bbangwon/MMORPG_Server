using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket? clientSocket)
        {
            try
            {
                if (clientSocket == null)
                    return;

                //Recv
                byte[] recvBuff = new byte[1024];
                int recvBytes = clientSocket.Receive(recvBuff);

                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes).Trim();
                Console.WriteLine($"[From Client] {recvData}");

                //Send
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");
                clientSocket.Send(sendBuff);

                //Close
                clientSocket.Shutdown(SocketShutdown.Both); //Shutdown 예고(우아한 종료)
                clientSocket.Close();   //종료
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost =  Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening...");

            while (true)
            {
                    
            }
        }
    }
}