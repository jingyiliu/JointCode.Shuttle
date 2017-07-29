using System;
using JointCode.Shuttle.Services;
using JoitCode.Shuttle.SimpleContract;

namespace JoitCode.Shuttle.SimpleServiceEnd
{
    [ServiceClass(typeof(ISimpleService2), Lifetime = LifetimeEnum.Transient)]
    public class SimpleService2 : ISimpleService2
    {
        public string GetOutput(string input)
        {
            return string.Format
                ("SimpleService2.GetOutput says: now, it's SimpleService2 reporting that we are running in AppDomain: {0}, and the input passed from the caller is: {1}",
                    AppDomain.CurrentDomain.FriendlyName, input);
        }
    }

    public class SimpleRemoteServiceEnd2 : SimpleRemoteServiceEnd
    {
        public override void RegisterServices()
        {
            var guid = Guid.NewGuid();
            _shuttleDomain.RegisterServiceGroup(ref guid,
                new ServiceTypePair(typeof(ISimpleService2), typeof(SimpleService2)));
        }
    }
}
