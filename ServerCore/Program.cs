// See https://aka.ms/new-console-template for more information


Task t1 = new Task(Thread_1);
Task t2 = new Task(Thread_2);
t1.Start();
t2.Start();

Task.WaitAll(t1, t2);
Console.WriteLine(_num);

class SpinLock
{
    volatile int _locked = 0;
    public void Acquire()
    {
        while (true)
        {
            //반환값이 원래 값이 나오기 때문에 1이 나온다면 이미 락이 걸려있는 상태
            //이때 다른 Task가 Release를 해서 0으로 만들어준다면 락이 풀리는 것
            //int original = Interlocked.Exchange(ref _locked, 1);

            //original은 경합하지 않는 Stack에 저장되어 있는 값이기 때문에 if문으로 비교해도 된다.
            //if (original == 0)
            //    break;

            //_locked가 expected와 같을 때만 바꾼다
            int expected = 0;   //내가 예상한 값 
            int desired = 1;    //바꿀 값

            //CompareExchange는 원래 값(_locked)과 세번째 param 비교해서 같으면 새로운 값으로 바꾸는 것이다.
            int original = Interlocked.CompareExchange(ref _locked, desired, expected);
            if (original == expected)   //원래 값이 내가 예상한 값과 같다면 락을 걸었다는 것
                break;

            //쉬다 올게~
            //Thread.Sleep(1);    //무조건 휴식 : 1ms 정도 쉬고 싶어요. 
            //Thread.Sleep(0);    //조건부 양보 : 우선순위가 나보다 같거나 높은 스레드가 없으면 다시 본인한테
            Thread.Yield();     //관대한 양보 : 지금 실행이 가능한 스레드가 있으면 실행하세요.
        }
    }

    public void Release()
    {
        _locked = 0;
    }
}

partial class Program
{
    static int _num = 0;
    static SpinLock _lock = new SpinLock();

    static void Thread_1()
    {
        for(int i = 0; i < 100000; i++)
        {
            _lock.Acquire();
            _num++;
            _lock.Release();
        }
    }

    static void Thread_2()
    {
        for (int i = 0; i < 100000; i++)
        {
            _lock.Acquire();
            _num--;
            _lock.Release();
        }
    }
}

