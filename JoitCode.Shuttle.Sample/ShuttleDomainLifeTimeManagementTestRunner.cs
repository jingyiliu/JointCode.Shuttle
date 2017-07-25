using System;
using JointCode.Shuttle;
using JointCode.Shuttle.Services;
using JoitCode.Shuttle.Sample.Contract;

namespace JoitCode.Shuttle.Sample
{
    [ServiceInterface]
    public interface IRemoteLifetimeService : IDisposable
    {
        void Execute();
    }

    [ServiceClass(typeof(IRemoteLifetimeService), Lifetime = LifetimeEnum.Transient)]
    public class RemoteLifetimeService : IRemoteLifetimeService
    {
        public RemoteLifetimeService()
        {
            Console.WriteLine("RemoteLifetimeService created in AppDomain [{0}]...", AppDomain.CurrentDomain.FriendlyName);
        }

        public void Execute()
        {
            Console.WriteLine("RemoteLifetimeService executed in AppDomain [{0}]...", AppDomain.CurrentDomain.FriendlyName);
        }

        public void Dispose()
        {
            Console.WriteLine("RemoteLifetimeService disposed in AppDomain [{0}]...", AppDomain.CurrentDomain.FriendlyName);
        }
    }

    [ServiceInterface]
    public interface ISingletonService : IDisposable
    {
        void Execute();
    }

    [ServiceClass(typeof(ISingletonService), Lifetime = LifetimeEnum.Singleton)]
    public class SingletonService : ISingletonService
    {
        public SingletonService()
        {
            Console.WriteLine("SingletonService created in AppDomain [{0}]...", AppDomain.CurrentDomain.FriendlyName);
        }

        public void Execute()
        {
            Console.WriteLine("SingletonService executed in AppDomain [{0}]...", AppDomain.CurrentDomain.FriendlyName);
        }

        public void Dispose()
        {
            Console.WriteLine("SingletonService disposed in AppDomain [{0}]...", AppDomain.CurrentDomain.FriendlyName);
        }
    }

    public class RemoteStarter1 : MarshalByRefObject
    {
        ShuttleDomain _shuttleDomain;

        public void CreateShuttleDomain()
        {
            // 创建一个 ShuttleDomain
            var key = this.GetType().Name;
            _shuttleDomain = ShuttleDomainHelper.Create(key, key);
        }

        public void RegisterServices()
        {
            var guid = Guid.NewGuid();
            _shuttleDomain.RegisterServiceGroup(ref guid,
                new ServiceTypePair(typeof(IRemoteLifetimeService), typeof(RemoteLifetimeService)),
                new ServiceTypePair(typeof(ISingletonService), typeof(SingletonService)));
        }

        public void DisposeShuttleDomain()
        {
            _shuttleDomain.Dispose();
        }
    }

    class ShuttleDomainLifeTimeManagementTestRunner : AbstractTestRunner
    {
        AppDomain _remoteDomain;
        RemoteStarter1 _remoteStarter1;

        ShuttleDomain _shuttleDomain;
        IRemoteLifetimeService _lifetimeService;
        ISingletonService _singletonService;

        public override bool Setup()
        {
            _remoteDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, null);

            _remoteStarter1 = (RemoteStarter1)_remoteDomain.CreateInstanceAndUnwrap
                (typeof(RemoteStarter1).Assembly.FullName, typeof(RemoteStarter1).FullName);
            _remoteStarter1.CreateShuttleDomain();
            _remoteStarter1.RegisterServices();

            var key = Guid.NewGuid().ToString();
            _shuttleDomain = ShuttleDomainHelper.Create(key, key);

            return true;
        }

        public override void RunTest()
        {
            _shuttleDomain.TryGetService(out _lifetimeService);
            _shuttleDomain.TryGetService(out _singletonService);
            _lifetimeService.Execute();
            _singletonService.Execute();
        }

        public override void Dispose()
        {
            // 服务端服务对象会被释放
            _lifetimeService.Dispose();

            // 服务端服务对象不会被释放
            _singletonService.Dispose(); 

            // 如果要多次运行该测试 (ShuttleDomainLifeTimeManagementTestRunner)，此处必须释放 _shuttleDomain。
            // 因为一个 AppDomain 只允许存在一个 ShuttleDomain 对象，类似于单例。
            // 如果只是运行该测试一次，则不必如此。
            _shuttleDomain.Dispose();
        }
    }

    class ShuttleDomainLifeTimeManagementTest : Test
    {
        internal ShuttleDomainLifeTimeManagementTest()
        {
            Name = "SD lifetime management";
            Description = "Test SD lifetime management";
        }

        internal override void Run()
        {
            Console.WriteLine("Prepare to run the life time management test in AppDomain [{0}]...", AppDomain.CurrentDomain.FriendlyName);

            var test = new ShuttleDomainLifeTimeManagementTestRunner();
            test.Setup();
            test.RunTest();
            test.Dispose();

            Console.WriteLine();
        }
    }
}
