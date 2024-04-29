using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

public class ThreadSafeRandom
{
    [ThreadStatic] static ThreadLocal<System.Random> random;

    // threadsafe implementation of random next [minInclusive, maxExclusive)
    public static int GetRandom(int min, int max)
    {
        if(random == null)
            random = new ThreadLocal<System.Random>(() => new System.Random(GetThreadSeed()));

        return random.Value.Next(min, max);
    }

    public static float GetRandomProb()
    {
        if (random == null)
            random = new ThreadLocal<System.Random>(() => new System.Random(GetThreadSeed()));

        return (float)random.Value.NextDouble();
    }

    private static int GetThreadSeed() => Thread.CurrentThread.ManagedThreadId;
}
