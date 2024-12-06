﻿using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Session
    {
        Socket? socket;

        //연결 해제 관리 1일 경우 연결 해제
        int _disconnected = 0;

        public void Start(Socket socket)
        {
            this.socket = socket;
            SocketAsyncEventArgs recvArgs = new();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
            recvArgs.SetBuffer(new byte[1024], 0, 1024);

            RegisterReceive(recvArgs);
        }

        public void Send(byte[] sendBuff)
        {
            socket?.Send(sendBuff);
        }

        public void Disconnect()
        {
            //멀티 스레드 코드
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            socket?.Shutdown(SocketShutdown.Both);
            socket?.Close();
        }

        #region 네트워크 통신
        void RegisterReceive(SocketAsyncEventArgs args)
        {
            if (socket == null)
                return;

            bool pending = socket.ReceiveAsync(args);
            if (pending == false)
                OnReceiveCompleted(null, args);
        }

        void OnReceiveCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                //TODO
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer!, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {recvData}");
                    RegisterReceive(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnReceiveCompleted Failed {e}");
                }
            }
            else
            {
                // Disconnect
            }
        } 
        #endregion
    }
}
