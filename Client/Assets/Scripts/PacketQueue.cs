using System.Collections.Generic;

public class PacketQueue
{
    public static PacketQueue Instance { get; } = new();
    Queue<IPacket> packetQueue = new();
    object _lock = new();

    public void Push(IPacket packet)
    {
        lock (_lock)
        {
            packetQueue.Enqueue(packet);
        }
    }

    public IPacket Pop()
    {
        lock (_lock)
        {
            if (packetQueue.Count == 0)
                return null;

            return packetQueue.Dequeue();
        }
    }

}
