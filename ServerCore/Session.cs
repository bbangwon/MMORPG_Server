using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public abstract class Session
    {
        Socket? socket;

        //연결 해제 관리 1일 경우 연결 해제
        int _disconnected = 0;

        object _lock = new();
        Queue<byte[]> sendQueue = new();

        List<ArraySegment<byte>> pendingList = new();
        SocketAsyncEventArgs sendArgs = new();
        SocketAsyncEventArgs recvArgs = new();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        public void Start(Socket socket)
        {
            this.socket = socket;
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
            recvArgs.SetBuffer(new byte[1024], 0, 1024);
            RegisterReceive();

            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
        }

        public void Send(byte[] sendBuff)
        {
            lock (_lock)
            {
                sendQueue.Enqueue(sendBuff);
                if (pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            //멀티 스레드 코드
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            if(socket?.RemoteEndPoint != null)
            {
                OnDisconnected(socket.RemoteEndPoint);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        #region 네트워크 통신

        void RegisterSend()
        {
            if (socket == null)
                return;

            while (sendQueue.Count > 0)
            {
                byte[] buff = sendQueue.Dequeue();
                pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }

            sendArgs.BufferList = pendingList;

            bool pending = socket.SendAsync(sendArgs);
            if (pending == false)
                OnSendCompleted(null, sendArgs);
        }

        void OnSendCompleted(object? sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    //TODO
                    try
                    {
                        sendArgs.BufferList = null;
                        pendingList.Clear();

                        OnSend(sendArgs.BytesTransferred);                        

                        if (sendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                        throw;
                    }
                }
                else
                {
                    // Disconnect
                    Disconnect();
                }
            }
        }

        void RegisterReceive()
        {
            if (socket == null)
                return;

            bool pending = socket.ReceiveAsync(recvArgs);
            if (pending == false)
                OnReceiveCompleted(null, recvArgs);
        }

        void OnReceiveCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                //TODO
                try
                {
                    OnRecv(new ArraySegment<byte>(args.Buffer!, args.Offset, args.BytesTransferred));
                    RegisterReceive();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnReceiveCompleted Failed {e}");
                }
            }
            else
            {
                // Disconnect
                Disconnect();
            }
        }
        #endregion
    }
}
