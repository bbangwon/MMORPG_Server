using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    class Listener
    {
        Socket? listenSocket;
        Func<Session>? sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session>? sessionFactory)
        {
            listenSocket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.sessionFactory += sessionFactory;

            listenSocket.Bind(endPoint);
            listenSocket.Listen(10);

            SocketAsyncEventArgs args = new();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccpet(args);
        }

        void RegisterAccpet(SocketAsyncEventArgs args)
        {
            if(listenSocket == null)
                return;

            args.AcceptSocket = null;
            bool pending = listenSocket.AcceptAsync(args);
            if(pending == false)
                OnAcceptCompleted(null, args);
        }

        void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                if(args.AcceptSocket != null)
                {
                    Session? session = sessionFactory?.Invoke();
                    session?.Start(args.AcceptSocket);
                    session?.OnConnected(args.AcceptSocket.RemoteEndPoint!);
                }
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            RegisterAccpet(args);
        }
    }
}
