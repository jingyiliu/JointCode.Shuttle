using System;

namespace JoitCode.Shuttle.Sample
{
    public abstract class AbstractTestRunner : IDisposable
    {
        public abstract bool Setup();
        public abstract void RunTest();
        public abstract void Dispose();
    }
}