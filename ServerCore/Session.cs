using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Session
    {
        Socket _socket;
        int _disconnected = 0;

        public void Start(Socket socket)
        {
            _socket = socket;
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
                        
            //버퍼 생성
            recvArgs.SetBuffer(new byte[1024], 0, 1024);
            RegisterRecv(recvArgs);
        }

        public void Send(byte[] sendBuff)
        {
            _socket.Send(sendBuff);
        }

        public void Disconnect()
        {
            //멀티스레드를 고려한 플래그 세팅
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크 통신
        void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);
            if (!pending)
                OnRecvCompleted(null, args);
        }

        void OnRecvCompleted(object? sender, SocketAsyncEventArgs args)
        {
            //데이터를 가져옴
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {

                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer!, args.Offset, args.BytesTransferred).Trim();
                    Console.WriteLine($"[From Client] {recvData}");
                    RegisterRecv(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {
                //TODO : Disconnect
            }
        } 
        #endregion
    }
}
