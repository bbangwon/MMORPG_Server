using ServerCore;

namespace Server
{
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int execTick;
        public Action action;
        public readonly int CompareTo(JobTimerElem other)
        {
            return other.execTick - execTick;
        }
    }


    public class JobTimer
    {
        readonly PriorityQueue<JobTimerElem> priorityQueue = new();
        readonly Lock _lock = new();

        public static JobTimer Instance { get; } = new();

        public void Push(Action action, int tickAfter = 0)
        {
            var job = new JobTimerElem
            {
                execTick = Environment.TickCount + tickAfter,
                action = action
            };

            lock (_lock)
            {
                priorityQueue.Push(job);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = Environment.TickCount;
                JobTimerElem job;

                lock (_lock)
                {
                    if (priorityQueue.Count == 0)
                        break;

                    job = priorityQueue.Peek();
                    if (job.execTick > now)
                        break;

                    priorityQueue.Pop();
                }
                job.action.Invoke();
            }
        }
    }
}
