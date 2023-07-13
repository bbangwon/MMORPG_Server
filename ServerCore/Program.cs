namespace ServerCore
{
    class Program
    {


        static void Main(string[] args)
        {
            ThreadStudy5();

            //ThreadStudy4();
            //ThreadStudy3();
            //ThreadStudy();
            //ThreadStudy2();
            //CacheStudy();
        }





        static int number = 0;
        private static void ThreadStudy5()
        {
            void Thread_1()
            {
                for (int i = 0; i < 100000; i++)
                {
                    Interlocked.Increment(ref number);                    
                }
            }

            void Thread_2()
            {
                for (int i = 0; i < 100000; i++)
                {
                    Interlocked.Decrement(ref number);
                }
            }

            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }




        static volatile int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;

        static void ThreadStudy4()
        {
            int _answer;
            bool _complete;

            void A()
            {
                //Store가 2번 발생
                _answer = 123;
                Thread.MemoryBarrier(); // Barrier 1 (Write)
                _complete = true;
                Thread.MemoryBarrier(); // Barrier 2 (Write)
            }

            //Read가 2번 발생
            void B()
            {                
                Thread.MemoryBarrier(); // Barrier 3 (Read)
                if (_complete)
                {
                    Thread.MemoryBarrier(); // Barrier 4 (Read)
                    Console.WriteLine(_answer);
                }
            }
        }

        static void ThreadStudy3()
        {
            void Thread_1()
            {
                y = 1;

                // -----------------
                Thread.MemoryBarrier();
                r1 = x;
            }

            void Thread_2()
            {
                x = 1;

                // -----------------
                Thread.MemoryBarrier();
                r2 = y;
            }

            int count = 0;
            while(true)
            {
                count++;
                x = y = r1 = r2 = 0;

                Task t1 = new Task(Thread_1);
                Task t2 = new Task(Thread_2);
                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                if (r1 == 0 && r2 == 0)
                    break;
            }

            Console.WriteLine($"{count}번 만에 빠져나옴!");
        }

        static void CacheStudy()
        {            
            int[,] arr = new int[10000, 10000];

            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                    for (int x = 0; x < 10000; x++)
                        arr[x, y] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(x, y) 순서 걸린 시간 {end - now}");
            }

            //Cache Hit 측정
            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                    for (int x = 0; x < 10000; x++)
                        arr[y, x] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(y, x) 순서 걸린 시간 {end - now}");
            }
        }


        #region ThreadStudy2
        //volatile 최적화하지 마라 문법
        static bool _stop = false;

        static void ThreadStudy2()
        {
            static void ThreadMain()
            {
                Console.WriteLine("쓰레드 시작!");
                while (!_stop)
                {
                    //누군가  stop 신호를 해주기를 기다린다
                }

                Console.WriteLine("쓰레드 종료!");
            }

            Task t = new Task(ThreadMain);
            t.Start();

            Thread.Sleep(1000);

            _stop = true;

            Console.WriteLine("Stop 호출");
            Console.WriteLine("종료 대기중");

            t.Wait();

            Console.WriteLine("종료 성공");
        } 
        #endregion


        #region ThreadStudy1
        static void ThreadStudy()
        {
            void MainThread(object? state)
            {
                for (int i = 0; i < 5; i++)
                    Console.WriteLine("Hello Thread!");
            }

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
        #endregion
    }
}