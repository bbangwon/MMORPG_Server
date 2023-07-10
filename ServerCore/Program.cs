namespace ServerCore
{
    class Program
    {
        //volatile 최적화하지 마라 문법
        volatile static bool _stop = false;

        static void ThreadMain()
        {
            Console.WriteLine("쓰레드 시작!");
            while (!_stop)
            {
                //누군가  stop 신호를 해주기를 기다린다
            }

            Console.WriteLine("쓰레드 종료!");
        }

        static void Main(string[] args)
        {
            Task t = new Task(ThreadMain);
            t.Start();

            Thread.Sleep(1000);

            _stop = true;

            Console.WriteLine("Stop 호출");
            Console.WriteLine("종료 대기중"); 
            
            t.Wait();   

            Console.WriteLine("종료 성공");
        }

        //static void MainThread(object? state)
        //{
        //    for (int i = 0; i < 5; i++)
        //        Console.WriteLine("Hello Thread!");
        //}

        //static void Main(string[] args)
        //{
        //    ThreadPool.SetMinThreads(1, 1);
        //    ThreadPool.SetMaxThreads(5, 5);

        //    for (int i = 0; i < 5; i++)
        //    {
        //        Task t = new Task(() => { while (true) { } });
        //        t.Start();
        //    }

        //    //for (int i = 0;i < 4;i++)
        //    //    ThreadPool.QueueUserWorkItem(obj => { while (true) { } });

        //    ThreadPool.QueueUserWorkItem(MainThread);

        //    //for (int i = 0; i < 1000; i++)
        //    //{
        //    //    Thread t = new Thread(MainThread);
        //    //    //t.Name = "Test Thread";
        //    //    t.IsBackground = true;
        //    //    t.Start();
        //    //}


        //    //Console.WriteLine("Waiting for Thread");

        //    //t.Join();
        //    //Console.WriteLine("Hello, World!");
        //    while (true) { }
        //}
    }
}