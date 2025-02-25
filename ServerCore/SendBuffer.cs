namespace ServerCore
{

    public class SendBufferHelper
    {
        public static readonly ThreadLocal<SendBuffer> CurrentBuffer = new(() => null!);
        public static int ChunkSize { get; set; } = 4096;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            CurrentBuffer.Value ??= new SendBuffer(ChunkSize);

            if(reserveSize > CurrentBuffer.Value.FreeSize)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            return CurrentBuffer.Value.Open(reserveSize);
        }

        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value?.Close(usedSize) ?? default;
        }
    }

    public class SendBuffer(int chunkSize)
    {
        readonly byte[] buffer = new byte[chunkSize];
        int usedSize = 0;

        public int FreeSize => buffer.Length - usedSize;

        public ArraySegment<byte> Open(int reserveSize)
        {
            if (reserveSize > FreeSize)
                return default;

            return new ArraySegment<byte>(buffer, usedSize, reserveSize);
        }

        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new(buffer, this.usedSize, usedSize);
            this.usedSize += usedSize;
            return segment;
        }
    }
}
