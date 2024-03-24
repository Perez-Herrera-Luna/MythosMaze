using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine;

public class ThreadSafeRandom
{
    [ThreadStatic] static ThreadLocal<System.Random> random;

    // threadsafe implementation of random range [minInclusive, maxExclusive)
    public static int GetRandom(int min, int max)
    {
        if(random == null)
            random = new ThreadLocal<System.Random>(() => new System.Random(GetThreadSeed()));

        return random.Value.Next(min, max);
    }

    private static int GetThreadSeed() => Thread.CurrentThread.ManagedThreadId;
}
