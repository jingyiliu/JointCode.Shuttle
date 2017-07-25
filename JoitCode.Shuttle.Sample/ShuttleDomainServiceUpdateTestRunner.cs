#define DOASSERT

using System;
using JointCode.Shuttle.Services;
using JoitCode.Shuttle.Sample.Contract;

namespace JoitCode.Shuttle.Sample
{
    [ServiceClass(typeof(IUpdatableService), Lifetime = LifetimeEnum.Transient)]
    public class UpdatableService3 : IUpdatableService
    {
        public void PrintMessage()
        {
            Console.WriteLine(this.GetType().Name + " says: now we are running in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);
        }
    }

    [ServiceClass(typeof(IUpdatableService), Lifetime = LifetimeEnum.Transient)]
    public class UpdatableService4 : IUpdatableService
    {
        public void PrintMessage()
        {
            Console.WriteLine(this.GetType().Name + " says: now we are running in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);
        }
    }

    public class RemoteServiceEnd3 : RemoteServiceEnd
    {
        public override void RegisterServices()
        {
            var guid = Guid.NewGuid();
            _shuttleDomain.RegisterServiceGroup(ref guid,
                new ServiceTypePair(typeof(IUpdatableService), typeof(UpdatableService3)));
        }

        public override void ConsumeServices() { }
    }

    public class RemoteServiceEnd4 : RemoteServiceEnd
    {
        public override void RegisterServices()
        {
            var guid = Guid.NewGuid();
            _shuttleDomain.RegisterServiceGroup(ref guid,
                new ServiceTypePair(typeof(IUpdatableService), typeof(UpdatableService4)));
        }

        public override void ConsumeServices() { }
    }

    class ShuttleDomainServiceUpdateTestRunner : ShuttleTestRunner
    {
        AppDomain _serviceEnd3Domain, _serviceEnd4Domain;
        RemoteServiceEnd _serviceEnd3, _serviceEnd4;

        void Initialize()
        {
            _serviceEnd3Domain = AppDomain.CreateDomain("ServiceEndDomain3", null, null);
            _serviceEnd3 = (RemoteServiceEnd)_serviceEnd3Domain.CreateInstanceAndUnwrap
                (typeof(RemoteServiceEnd3).Assembly.FullName, typeof(RemoteServiceEnd3).FullName);

            _serviceEnd4Domain = AppDomain.CreateDomain("ServiceEndDomain4", null, null);
            _serviceEnd4 = (RemoteServiceEnd)_serviceEnd4Domain.CreateInstanceAndUnwrap
                (typeof(RemoteServiceEnd4).Assembly.FullName, typeof(RemoteServiceEnd4).FullName);

            // 分别在 3 个 AppDomain 中创建 ShuttleDomain
            _serviceEnd3.CreateShuttleDomain();
            _serviceEnd4.CreateShuttleDomain();
            var key = Guid.NewGuid().ToString();
            _shuttleDomain = ShuttleDomainHelper.Create(key, key);
        }

        public override bool Setup()
        {
            Initialize();
            return true;
        }

        public override void RunTest()
        {
            IUpdatableService updatableService;

            // 注册 _serviceEnd3Domain 的服务
            _serviceEnd3.RegisterServices();
            // 消费服务
            _shuttleDomain.TryGetService(out updatableService);
            updatableService.PrintMessage();
            // 卸载 _serviceEnd3Domain 的服务
            _serviceEnd3.DisposeShuttleDomain();

            //updatableService = null;

            // 注册 _serviceEnd4Domain 的服务
            _serviceEnd4.RegisterServices();
            // 消费服务
            _shuttleDomain.TryGetService(out updatableService);
            updatableService.PrintMessage();
        }

        public override void Dispose()
        {
            _shuttleDomain.Dispose();
            AppDomain.Unload(_serviceEnd3Domain);
            AppDomain.Unload(_serviceEnd4Domain);
        }
    }

    class ShuttleDomainServiceUpdateTest : Test
    {
        internal ShuttleDomainServiceUpdateTest()
        {
            Name = "SD service update";
            Description = "Use SD to register/unregister services";
        }

        internal override void Run()
        {
            var test = new ShuttleDomainServiceUpdateTestRunner();
            test.Setup();
            test.RunTest();
            test.Dispose();
        }
    }
}