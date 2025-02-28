namespace ServerCore
{
    public class PriorityQueue<T> where T : IComparable<T>    
    {
        readonly List<T> heap = [];
        public int Count => heap.Count;

        public void Push(T data)
        {
            heap.Add(data);

            int now = heap.Count - 1;
            while(now > 0)
            {
                int next = (now - 1) / 2;
                if (heap[now].CompareTo(heap[next]) < 0)
                    break;

                (heap[next], heap[now]) = (heap[now], heap[next]);
                now = next;
            }
        }

        public T Pop()
        {
            T ret = heap[0];

            int last = heap.Count - 1;
            heap[0] = heap[last];
            heap.RemoveAt(last);
            last--;

            int now = 0;
            while (true)
            {
                int left = 2 * now + 1;
                int right = 2 * now + 2;

                int next = now;
                if (left <= last && heap[next].CompareTo(heap[left]) < 0)
                    next = left;
                
                if (right <= last && heap[next].CompareTo(heap[right]) < 0)
                    next = right;
                
                if (next == now)
                    break;

                (heap[next], heap[now]) = (heap[now], heap[next]);
                now = next;
            }
            return ret;
        }  
        
        public T? Peek()
        {
            if(heap.Count == 0)
                return default;

            return heap[0];
        }
    }
}
