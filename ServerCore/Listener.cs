using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    class Listener
    {
        Socket? listenSocket;
        Action<Socket>? onAcceptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            listenSocket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.onAcceptHandler += onAcceptHandler;

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
                    onAcceptHandler?.Invoke(args.AcceptSocket);
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
