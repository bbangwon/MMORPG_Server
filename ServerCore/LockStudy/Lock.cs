namespace ServerCore.LockStudy
{
    //재귀적 락을 허용할지 (No) => WriteLock을 잡고 있는 상태에서 WriteLock OK, WriteLock->ReadLock OK, ReadLock -> WriteLock NO
    //스핀락 정책(5000번 -> yield)
    class MyReaderWriterLock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7FFF0000;
        const int READ_MASK = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;

        //첫비트는 음수가 될 가능성이 있으므로 사용 안함
        // [Unused(1)] [WriteThreadId(15)] [ReadCount(16)]
        int _flag = EMPTY_FLAG;
        int _wirteCount = 0;

        public void WriteLock()
        {
            // 동일 쓰레드가 WriteLock을 이미 획득하고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                _wirteCount++;
                return;
            }

            // 아무도 WriteLock or ReadLock을 획득하고 있지 않을 때, 경합해서 소유권을 얻는다
            int desired = Thread.CurrentThread.ManagedThreadId << 16 & WRITE_MASK;
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    //시도를 해서 성공하면 return
                    //동시 다발적인 문제가 있음.. Interlock계열 함수로 변경 필요
                    //if (_flag == EMPTY_FLAG)
                    //{
                    //    _flag = desired;
                    //    return;
                    //}

                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _wirteCount = 1;
                        return;
                    }

                }
                Thread.Yield();
            }
        }

        public void WriteUnlock()
        {
            //초기상태로 변경
            int lockCount = --_wirteCount;
            if (lockCount == 0)
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            // 동일 쓰레드가 WriteLock을 이미 획득하고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                Interlocked.Increment(ref _flag);
                return;
            }

            //아무도 WriteLock을 획득하고 있지 않으면, ReadCount를 1 늘린다.
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = _flag & READ_MASK;
                    //_flag가 예상한 값.. expected에는 Write ThreadId가 0일 경우
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return;

                    //if((_flag & WRITE_MASK) == 0)
                    //{
                    //    _flag = _flag + 1;
                    //    return;
                    //}
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
