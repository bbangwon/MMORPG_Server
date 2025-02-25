using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;
            while(true)
            {
                if (buffer.Array == null)
                    return processLen;

                //최소한 헤더는 파싱할 수 있는지 확인
                if (buffer.Count < HeaderSize)
                    break;

                //패킷이 완전체로 도착했는지 확인
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;

                //여기까지 왔으면 패킷 조립 가능
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));                

                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        Socket? socket;

        //연결 해제 관리 1일 경우 연결 해제
        int _disconnected = 0;

        RecvBuffer recvBuffer = new(1024);

        object _lock = new();
        Queue<ArraySegment<byte>> sendQueue = new();

        List<ArraySegment<byte>> pendingList = new();
        SocketAsyncEventArgs sendArgs = new();
        SocketAsyncEventArgs recvArgs = new();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        public void Start(Socket socket)
        {
            this.socket = socket;
            
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterReceive();
        }

        public void Send(ArraySegment<byte> sendBuff)
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
                ArraySegment<byte> buff = sendQueue.Dequeue();
                pendingList.Add(buff);
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

            recvBuffer.Clean();
            var segment = recvBuffer.WriteSegment;
            recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

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
                    if(recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    int processLen = OnRecv(recvBuffer.ReadSegment);
                    if(processLen < 0 || recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    if (recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

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
