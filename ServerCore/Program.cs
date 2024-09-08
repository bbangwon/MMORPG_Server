// See https://aka.ms/new-console-template for more information

using ServerCore;

Task t1 = new Task(() =>
{
    for (int i = 0; i < 100000; i++)
    {
        _lock.WriteLock();
        count++;
        _lock.WriteUnlock();
    }
});

Task t2 = new Task(() =>
{
    for (int i = 0; i < 100000; i++)
    {
        _lock.WriteLock();
        count--;
        _lock.WriteUnlock();
    }
});

t1.Start();
t2.Start();

Task.WaitAll(t1, t2);

Console.WriteLine(count);


partial class Program
{
    static volatile int count = 0;
    static Lock _lock = new Lock();
}

