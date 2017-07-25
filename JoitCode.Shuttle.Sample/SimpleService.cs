using System;
using JointCode.Shuttle.Services;
using JoitCode.Shuttle.Sample.Contract;

namespace JoitCode.Shuttle.Sample
{
    [ServiceClass(typeof(ISimpleService), Lifetime = LifetimeEnum.Singleton)]
    public class SimpleService : ISimpleService
    {
        public void PrintMessage()
        {
            Console.WriteLine(this.GetType().Name + " | now we are running in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);
        }

        public string GetOutput(string input)
        {
            Console.WriteLine(this.GetType().Name + " | now we are running in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);
            return "(" + input + ")";
        }
    }
}