namespace ServerCore
{
    public interface IJobQueue
    {
        void Push(Action job);
    }

    public class JobQueue : IJobQueue
    {
        readonly Queue<Action> jobQueue = new();
        readonly Lock _lock = new();
        bool flush = false;

        public void Push(Action job)
        {
            bool flush = false;
            lock (_lock)
            {
                jobQueue.Enqueue(job);
                if(!this.flush)
                {
                    flush = this.flush = true;
                }
            }

            if(flush)
                Flush();
        }

        private void Flush()
        {
            while(true)
            {
                Action? action = Pop();
                if (action == null)
                    return;

                action.Invoke();
            }
        }

        Action? Pop()
        {
            lock (_lock)
            {
                if (jobQueue.Count == 0)
                {
                    this.flush = false;
                    return null;
                }

                return jobQueue.Dequeue();
            }
        }
    }
}
