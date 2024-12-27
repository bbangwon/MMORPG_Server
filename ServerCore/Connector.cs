using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Connector
    {
        Func<Session>? _sessionFactory;
        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            Socket socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new();
            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = endPoint;
            args.UserToken = socket;

            _sessionFactory = sessionFactory;

            RegisterConnect(args);
        }

        void RegisterConnect(SocketAsyncEventArgs args)
        {
            var socket = args.UserToken as Socket;
            if (socket == null)
                return;

            bool pending = socket.ConnectAsync(args);
            if (pending == false)
                OnConnectCompleted(null, args);
        }

        void OnConnectCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                if(args.ConnectSocket != null 
                    && _sessionFactory != null
                    && args.RemoteEndPoint != null)
                {
                    Session session = _sessionFactory.Invoke();
                    session.Start(args.ConnectSocket);
                    session.OnConnected(args.RemoteEndPoint);
                }
            }
            else
            {
                Console.WriteLine($"OnConnectCompleted Fail: {args.SocketError}");
            }
        }
    }
}
