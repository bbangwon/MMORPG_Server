// See https://aka.ms/new-console-template for more information

static void MainThread(object? state)
{
    for (int i = 0; i < 5; i++)
        Console.WriteLine("Hello Thread!");
}

//쓰레드 풀에서 쓰레드를 가져와서 실행
ThreadPool.QueueUserWorkItem(MainThread);

//// 쓰레드 생성
//var t = new Thread(MainThread);

////스레드 이름 지정
//t.Name = "Test Thread";

//// 쓰레드가 생성될때 기본적으로 Foreground로 생성되는데,
//// 이것을 Background로 변경하면 메인 쓰레드가 종료되면 같이 종료된다.
//t.IsBackground = true;

//t.Start();
//Console.WriteLine("Waiting for Thread!");

//t.Join();
//Console.WriteLine("Hello, World!");
Console.ReadLine();
