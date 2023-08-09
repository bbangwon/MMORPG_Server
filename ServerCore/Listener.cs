using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        Socket? _listenSocket = null;
        Func<Session>? _sessionFactory = null;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            // Listen Socket
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory = sessionFactory;
            //Bind
            _listenSocket.Bind(endPoint);

            //Listen
            //Backlog :  최대 대기수(라이브로 나갔을때 조절)
            _listenSocket.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            //이벤트 재 사용할때
            args.AcceptSocket = null;

            bool pending = _listenSocket!.AcceptAsync(args);
            if (!pending) 
                OnAcceptCompleted(null, args);
        }

        void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory!.Invoke();
                session.Start(args.AcceptSocket!);
                session.OnConnected(args.AcceptSocket!.RemoteEndPoint!);
            }
            else
            {
                Console.WriteLine(args.SocketError);
            }

            RegisterAccept(args);
        }
    }
}
