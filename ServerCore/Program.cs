namespace ServerCore
{
    /// <summary>
    /// Read는 마음대로 하는데.. Write할때만 ReadLock도 걸리는..
    /// </summary>
    class RWLock
    {
        static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        class Reward
        {

        }

        static Reward GetRewardById(int id)
        {
            _lock.EnterReadLock();
            
            _lock.ExitReadLock();
            return null;
        }

        static void AddReward(Reward reward)
        {
            _lock.EnterWriteLock();

            _lock.ExitWriteLock();
        }


    }

    class Lock
    {
        /// <summary>
        /// 첫 인자값으로 문이 열린채로 시작할지 문이 닫힌채로 시작할지 설정
        /// bool <- 커널
        /// </summary>
        AutoResetEvent _auto = new AutoResetEvent(true);
        ManualResetEvent _manual = new ManualResetEvent(true);

        public void Acquire()
        {
            //_auto.WaitOne();   //입장시도(true인 상태여서)
            ////_auto.Reset();     //bool  => false로 바꿔주지만 WaitOne에서 같이 처리됨

            _manual.WaitOne();
            _manual.Reset();
        }

        public void Release()
        {
            //_auto.Set();       //풀어줌

            _manual.Set();
        }
    }

    class SpinLock
    {
        volatile int _locked = 0;

        public void Acquire()
        {
            while (true) 
            {
                //int original = Interlocked.Exchange(ref _locked, 1);
                //if (original == 0) //아무도 없는 상태, 1을 리턴했다면 이미 잡고 있는 상태
                //    break;

                // CAS Compare-And-Swap
                int expected = 0;   //내가 예상한 값
                int desired = 1;    //내가 원하는 값(예상한 값이 맞다면 1로 대입하고 싶다..)
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    break;

                //쉬다 올게~
                //Thread.Sleep(1);
                //Thread.Sleep(0);
                Thread.Yield();
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }

    class FastLock
    {
        int id;
    }

    class SessionManager
    {
        static object _lock = new object();

        public static void TestSession()
        {
            lock (_lock)
            {

            }
        }

        public static void Test()
        {
            lock(_lock)
            {
                UserManager.TestUser();
            }
        }
    }

    class UserManager
    {
        static object _lock = new object();

        public static void Test()
        {            
            lock(_lock)
            {
                SessionManager.TestSession();
            }
        }

        public static void TestUser()
        {
            lock(_lock)
            {

            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ThreadStudy8();
            //ThreadStudy7();
            //ThreadStudy6();

            //ThreadStudy5();
            //ThreadStudy4();
            //ThreadStudy3();
            //ThreadStudy();
            //ThreadStudy2();
            //CacheStudy();
        }

        static int number = 0;
        static object _obj = new object();
        static Lock _lock = new Lock();
        static Mutex _mutex = new Mutex();

        private static void ThreadStudy8()
        {
            void Thread_1()
            {
                for (int i = 0; i < 100000; i++)
                {
                    _mutex.WaitOne();
                    number++;
                    _mutex.ReleaseMutex();
                }
            }

            void Thread_2()
            {
                for (int i = 0; i < 100000; i++)
                {
                    _mutex.WaitOne();
                    number--;
                    _mutex.ReleaseMutex();
                }
            }

            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }

        private static void ThreadStudy7()
        {
            void Thread_1()
            {
                for (int i = 0; i < 100000; i++)
                {
                    _lock.Acquire();
                    number++;
                    _lock.Release();
                }
            }

            void Thread_2()
            {
                for (int i = 0; i < 100000; i++)
                {
                    _lock.Acquire();
                    number--;
                    _lock.Release();
                }
            }

            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }

        private static void ThreadStudy6()
        {
            void Thread_1()
            {
                for (int i = 0; i < 100; i++)
                {
                    //상호배제 Mutual Exclusive
                    SessionManager.Test();

                    //Monitor.Enter(_obj);
                    //number++;
                    //Monitor.Exit(_obj);
                }
            }

            void Thread_2()
            {
                for (int i = 0; i < 100; i++)
                {
                    UserManager.Test();
                    //lock(_obj)
                    //{
                    //    number--;
                    //}
                }
            }

            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();           
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }

        private static void ThreadStudy5()
        {
            void Thread_1()
            {
                for (int i = 0; i < 100000; i++)
                {
                    int prev = number;
                    Interlocked.Increment(ref number);                    
                    int next = number;
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