namespace ServerCore
{
    //재귀적 락을 허용할지 (No),
    //Yes일 경우 WriteLock->WriteLock 가능, WriteLock->ReadLock 가능
    //스핀락 정책(5000번 시도 -> Yield)
    class Lock
    {        
        const int EMPTY_FLAG =  0x00000000;
        const int WRITE_MASK =  0x7FFF0000;
        const int READ_MASK =   0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;

        // [Unused(1)] [WriteThreadId(15)] [ReadCount(16)]
        // WriteThreadId : WriteLock을 가지고 있는 ThreadId
        // ReadCount : ReadLock을 가지고 있는 개수
        int _flag = EMPTY_FLAG;
        int _writeCount = 0;

        public void WriteLock()
        {
            //동일 쓰레드가 WriteLock을 가지고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if(lockThreadId == Environment.CurrentManagedThreadId)
            {
                _writeCount++;
                return;
            }

            // 아무도 WriteLock or ReadLock을 가지고 있지 않을 때, 경합해서 소유권을 얻는다.
            int desired = (Environment.CurrentManagedThreadId << 16) & WRITE_MASK;
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    // 시도를 해서 성공하면 리턴
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                }
                Thread.Yield();
            }
        }

        public void WriteUnlock()
        {
            // 동일 쓰레드가 여러번 WriteLock을 걸었을 때, WriteCount만 감소시킨다.
            int lockCOunt = --_writeCount;
            if (lockCOunt == 0)
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            // 동일 쓰레드가 WriteLock을 가지고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (lockThreadId == Environment.CurrentManagedThreadId)
            {
                Interlocked.Increment(ref _flag);
                return;
            }

            // 아무도 WriteLock을 가지고 있지 않을 때, ReadCount를 1 늘림
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    // ReadCount를 1 증가시킨다.
                    int expected = (_flag & READ_MASK);
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return;
                }
                Thread.Yield();
            }
        }

        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag);
        }

    }
}
