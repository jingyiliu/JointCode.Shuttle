using System;
using JointCode.Shuttle.Services;
using JoitCode.Shuttle.Sample.Contract;

namespace JoitCode.Shuttle.Sample.ServiceEnd2
{
    [ServiceClass(typeof(IFakeService), Lifetime = LifetimeEnum.Singleton)]
    public class FakeService : IFakeService
    {
        int _printTimes;

        public void PrintMessage()
        {
            Console.WriteLine(this.GetType().Name + " | now we are running in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);
        }

        public string PrintAndReturn(string msg)
        {
            ++_printTimes;
            var result = "<" + msg + " ## " + _printTimes + ">";
            Console.WriteLine(this.GetType().Name + " | now we are running in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);
            return result;
        }
    }

    public class RemoteServiceEnd2 : RemoteServiceEnd
    {
        public override void RegisterServices()
        {
            var guid = Guid.NewGuid();
            _shuttleDomain.RegisterServiceGroup(ref guid,
                new ServiceTypePair(typeof(IFakeService), typeof(FakeService)));
        }

        public override void ConsumeServices()
        {
            ISimpleService service;
            if (_shuttleDomain.TryGetService(out service))
            {
                Console.WriteLine("AppDomain [{0}], before calling the remote service: ", AppDomain.CurrentDomain.FriendlyName);
                var result = service.GetOutput("China");
                Console.WriteLine("AppDomain [{0}], after calling the remote service with result [{1}] ", AppDomain.CurrentDomain.FriendlyName, result);
                Console.WriteLine();
            }
        }
    }
}
