// See https://aka.ms/new-console-template for more information


Task t1 = new Task(Thread_1);
Task t2 = new Task(Thread_2);
t1.Start();
t2.Start();

Task.WaitAll(t1, t2);
Console.WriteLine(_num);

//Event 방식은 조금 더 오래걸림
class Lock
{
    // bool <- 커널
    //문이 열려있는 상태 : true
    ManualResetEvent _available = new ManualResetEvent(true);
    public void Acquire()
    {
        //입장 시도
        _available.WaitOne();
        //입장 성공시 자동 문이 닫힘
        _available.Reset(); // <= WaitOne()에서 이미 문이 닫힘
        
    }

    public void Release()
    {
        //퇴장
        _available.Set();   //flag = true
        //문이 열림
    }
}

partial class Program
{
    static int _num = 0;
    static Mutex _lock = new Mutex();

    static void Thread_1()
    {
        for(int i = 0; i < 100000; i++)
        {
            _lock.WaitOne();
            _lock.WaitOne();
            _num++;
            _lock.ReleaseMutex();
            _lock.ReleaseMutex();
        }
    }

    static void Thread_2()
    {
        for (int i = 0; i < 100000; i++)
        {
            _lock.WaitOne();
            _num--;
            _lock.ReleaseMutex();
        }
    }
}

