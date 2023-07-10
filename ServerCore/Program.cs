namespace ServerCore
{
    internal class Program
    {
        static void MainThread(object? state)
        {
            for (int i = 0; i < 5; i++)
                Console.WriteLine("Hello Thread!");
        }

        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            for (int i = 0; i < 5; i++)
            {
                Task t = new Task(() => { while (true) { } });
                t.Start();
            }

            //for (int i = 0;i < 4;i++)
            //    ThreadPool.QueueUserWorkItem(obj => { while (true) { } });
            
            ThreadPool.QueueUserWorkItem(MainThread);

            //for (int i = 0; i < 1000; i++)
            //{
            //    Thread t = new Thread(MainThread);
            //    //t.Name = "Test Thread";
            //    t.IsBackground = true;
            //    t.Start();
            //}


            //Console.WriteLine("Waiting for Thread");

            //t.Join();
            //Console.WriteLine("Hello, World!");
            while (true) { }
        }
    }
}