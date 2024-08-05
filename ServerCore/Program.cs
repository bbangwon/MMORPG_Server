// See https://aka.ms/new-console-template for more information

Task t = new Task(ThreadMain);
t.Start();

Thread.Sleep(1000);

_stop = true;

Console.WriteLine("Stop 호출");
Console.WriteLine("종료 대기중");

t.Wait();   //Task가 종료될때까지 대기. Thread.Join과 같은 역할

Console.WriteLine("종료 성공");

Console.ReadLine();

partial class Program
{
    volatile static bool _stop = false;

    static void ThreadMain()
    {
        Console.WriteLine("쓰레드 시작!");

        while (_stop == false)
        {
            //누군가가 stop 신호를 해주기를 기다린다.
        }

        Console.WriteLine("쓰레드 종료!");
    }
}

