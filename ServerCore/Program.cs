// See https://aka.ms/new-console-template for more information

int count = 0;
while(true)
{
    count++;
    x = 0;
    y = 0;
    r1 = 0;
    r2 = 0;

    Task t1 = new Task(Thread_1);
    Task t2 = new Task(Thread_2);

    t1.Start();
    t2.Start();

    Task.WaitAll(t1, t2);

    if(r1 == 0 && r2 == 0)
        break;
}
Console.WriteLine($"{count}번에 빠져나옴");

Console.ReadLine();



partial class Program
{
    static int x = 0;
    static int y = 0;
    static int r1 = 0;
    static int r2 = 0;

    static void Thread_1()
    {
        y = 1;  //Store y
        Thread.MemoryBarrier();
        //----------------------
        r1 = x; //Load x
    }

    static void Thread_2()
    {
        x = 1;  //Store x
        Thread.MemoryBarrier();
        //----------------------
        r2 = y; //Load x
    }

}
