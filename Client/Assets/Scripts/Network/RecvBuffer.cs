using System;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> _buffer;
        int _readPos = 0;
        int _writePos = 0;

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize => _writePos - _readPos;
        public int FreeSize => _buffer.Count - _writePos;

        public ArraySegment<byte> ReadSegment => new ArraySegment<byte>(_buffer.Array!, _buffer.Offset + _readPos, DataSize);
        public ArraySegment<byte> WriteSegment => new ArraySegment<byte>(_buffer.Array!, _buffer.Offset + _writePos, FreeSize);

        public void Clean()
        {
            int dataSize = DataSize;
            if (dataSize == 0)
            {
                // 남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
                _readPos = _writePos = 0;
            }
            else
            {
                // 남은 데이터가 있으면 앞으로 복사
                Array.Copy(_buffer.Array!, _buffer.Offset + _readPos, _buffer.Array!, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;

            _readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;

            _writePos += numOfBytes;
            return true;
        }
    }
}
