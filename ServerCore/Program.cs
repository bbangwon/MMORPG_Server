// See https://aka.ms/new-console-template for more information

lock(_lock)
{
    
}

//SpinLock 사용방법
//SpinLock는 몇번 시도 하다가 답이 없을 경우에는 중간 중간 yield를 호출한다.
bool lockTaken = false;
try
{
    //lockTaken이 true가 되면 Enter가 성공한 것이다.
    _lock2.Enter(ref lockTaken);
}
finally
{
    //Exception이 발생하더라도 Exit는 호출되어야 한다.
    if (lockTaken)
        _lock2.Exit();
}


partial class Program
{
    //내부적으로는 Monitor를 사용한다.
    static object _lock = new object();
    static SpinLock _lock2 = new SpinLock();

    //많이 느림
    //같은 프로그램이 아니더라도 순서를 맞추는 동기화가 가능함
    //MMORPG에서는 거의 사용하지 않음
    static Mutex _lock3 = new Mutex();

    //[] [] [] [] []
    class Reward
    {

    }

    static ReaderWriterLockSlim _lock4 = new ReaderWriterLockSlim();

    //리워드를 찾아서 반환해주는
    // 99.99999%
    static Reward? GetRewardById(int id)
    {
        //아무도 WriteLock을 가지고 있지 않다면 스레드들이 동시에 읽기를 할 수 있다.
        //마치 Lock을 걸지 않은 것처럼 동작한다.
        _lock4.EnterReadLock();

        _lock4.ExitReadLock();
        return null;
    }

    // 0.00001%
    static void AddReward(Reward reward)
    {
        _lock4.EnterWriteLock();

        _lock4.ExitWriteLock();
    }
}

