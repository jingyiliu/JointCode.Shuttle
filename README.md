# JointCode.Shuttle
**JointCode.Shuttle** is a fast, flexible and easy-to-use service framework for inter-AppDomain communication. It's a replacement for MarshalByrefObject which is provided by the runtime libraries. Functionalities including:<br>

1. Service (interface) oriented.
2. Access services registered in any AppDomain from an AppDomain.
3. Better performance: 60 ~ 70 faster than MarshalByrefObject.
4. Services are manageable: you can dynamically register / unregister service group.
5. Strong type, easy to use (while the MarshalByrefObject way relies on magic string to find the service type).
6. Built-in IoC functionality for automatic service dependencies management.
7. Supports for lazy type / assembly loading.
8. The remote service lifetime can be managed by leasing, or by user (while the MarshalByrefObject way does not provide remote service       life management).
9. Simple and quick to get started.
10. Support .net 2.0.

### A simple sample
This example is also available as a stand-alone integration test:

```cs
using System;
using System.Reflection;
using System.Reflection.Emit;
using JointCode.Common.Extensions;
using JointCode.Expressions;
using JointCode.ServiceInjector;
using JointCode.Shuttle;
using JointCode.Shuttle.Services;

namespace JoitCode.Shuttle.SimpleSample
{
    public static class ShuttleDomainHelper
    {
        public static ShuttleDomain Create(string assemblySymbol, string assemblyName)
        {
            return Create(assemblySymbol, assemblyName, null);
        }

        public static ShuttleDomain Create(string assemblySymbol, string assemblyName, ServiceContainer svContainer)
        {
            var dynAsmOptions = new DynamicAssemblyOptions
            {
                AccessMode = AssemblyBuilderAccess.Run,
                AssemblyName = new AssemblyName(assemblyName)
            };

            var options = new ShuttleDomainOptions
            {
                DynamicAssemblySymbol = assemblySymbol,
                DynamicAssemblyOptions = dynAsmOptions,
                DefaultLeaseTime = 10.Seconds(),
                PollingInterval = 5.Seconds()
            };

            try
            {
                return ShuttleDomain.Create(ref options, svContainer);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    Console.WriteLine(e.InnerException.Message);
                else
                    Console.WriteLine(e.Message);
                return null;
            }
        }
    }

    [ServiceInterface]
    public interface ISimpleService
    {
        string GetOutput(string input);
    }

    [ServiceClass(typeof(ISimpleService), Lifetime = LifetimeEnum.Transient)]
    public class SimpleService : ISimpleService
    {
        public string GetOutput(string input)
        {
            return string.Format
                ("SimpleService.GetOutput says: now, we are running in AppDomain: {0}, and the input passed from the caller is: {1}",
                    AppDomain.CurrentDomain.FriendlyName, input);
        }
    }

    public class ServiceProvider : MarshalByRefObject
    {
        // 这里必须使用一个字段来持有 ShuttleDomain 实例的引用，因为它是当前 AppDomain 与外部 AppDomain 之间通信的桥梁。
        // 如果该实例被垃圾回收，通过该实例注册的所有服务会被注销，且当前 AppDomain 与外部 AppDomain 之间将无法通信。
        // We need a field to keep the _shuttleDomain alive, because if it is garbage collected, we'll lose all communications
        // with other AppDomains.
        ShuttleDomain _shuttleDomain;

        public void RegisterServices()
        {
            // 注册服务组时，需要传递一个 Guid 对象
            // A Guid is needed when registering service group
            var guid = Guid.NewGuid();
            _shuttleDomain.RegisterServiceGroup(ref guid,
                new ServiceTypePair(typeof(ISimpleService), typeof(SimpleService)));
        }

        public void CreateShuttleDomain()
        {
            // 创建一个 ShuttleDomain
            // Create a ShuttleDomain object
            _shuttleDomain = ShuttleDomainHelper.Create("domain1", "domain1");
        }

        public void DisposeShuttleDomain()
        {
            _shuttleDomain.Dispose();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // 要使用 JointCode.Shuttle 进行跨 AppDomain 通信，首先必须初始化 ShuttleDomain。
            // 这个初始化操作一般在默认 AppDomain 执行，但也可以在其他 AppDomain 中执行，都是一样的。
            // To be able to make inter-AppDomain communication using JointCode.Shuttle, firstly we must initialize the ShuttleDomain.
            // It does not matter whether the initialization operation is done in default AppDomain or any other AppDomains, but it must 
            // be done before we creating any ShuttleDomain instances.
            ShuttleDomain.Initialize();

            // 在默认 AppDomain 中创建一个子 AppDomain。
            // Creating a child AppDomain in default AppDomain.
            var serviceEnd1Domain = AppDomain.CreateDomain("ServiceEndDomain1", null, null);

            // 创建一个 ServiceProvider 对象以用于操作该子 AppDomain。
            // Creating a ServiceProvider instance for operating that AppDomain.
            var serviceProvider = (ServiceProvider)serviceEnd1Domain.CreateInstanceAndUnwrap
                (typeof(Program).Assembly.FullName, "JoitCode.Shuttle.SimpleSample.ServiceProvider");

            // 在子 AppDomain 中，创建一个 ShuttleDomain 实例。
            // Creating a ShuttleDomain instance in the child AppDomain.
            serviceProvider.CreateShuttleDomain();

            // 在子 AppDomain 中，注册 ISimpleService 服务。
            // Registering ISimpleService service in child AppDomain.
            serviceProvider.RegisterServices();

            // 在默认 AppDomain 中，创建一个 ShuttleDomain。
            // 事实上，在应用程序的每个 AppDomain 中都需要有一个且只能有一个 ShuttleDomain 对象。
            // 该对象用于与其他 AppDomain 中的 ShuttleDomain 对象通信。
            // Creating a ShuttleDomain instance in default AppDomain.
            // Actually, we needs to create one and only one ShuttleDomain instance in every AppDomains.
            // The ShuttleDomain instance communicates with other ShuttleDomain instances in other AppDomains.
            var str = Guid.NewGuid().ToString();
            var shuttleDomain = ShuttleDomainHelper.Create(str, str);

            // 在默认 AppDomain 中，获取子 AppDomain 中注册的 ISimpleService 服务实例。
            // 目前服务实例的默认生存期为 1 分钟。每次调用服务方法时，服务实例的生存期延长 30 秒。
            // Get the ISimpleService service in default AppDomain, which is registered by the child AppDomain.
            // The lifetime of service is default to 1 minute, every call to the service method extends that time for 30 seconds.
            ISimpleService service;
            if (shuttleDomain.TryGetService(out service))
            {
                try
                {
                    Console.WriteLine("Currently, we are running in AppDomain {0} before calling the remote service method...", 
                        AppDomain.CurrentDomain.FriendlyName);

                    Console.WriteLine();
                    // 调用子 AppDomain 中注册的 ISimpleService 服务实例的服务方法。
                    // Call the service method of ISimpleService service.
                    var output = service.GetOutput("China");
                    Console.WriteLine(output);

                    Console.WriteLine();
                    Console.WriteLine("Tests completed...");
                }
                catch
                {
                    Console.WriteLine();
                    Console.WriteLine("Failed to invoke the remote service method...");
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Failed to create remote service instance...");
            }

            // 通知子 AppDomain 立即释放 ISimpleService 服务实例，而不用等待其生存期结束。
            // 此为可选操作，因为即使不手动释放 ISimpleService 服务实例，在其生命期结束之时系统也会自动释放该实例
            //（如果 ISimpleService 实现了 IDisposable，还会调用其 Dispose 方法）
            // Indicate the child AppDomain to release the ISimpleService service immediately, instead of waiting for its lifetime to end.
            // This is optional, because even if we do not do this explicitly, the ISimpleService service will still get released in the 
            // child AppDomain automatically when its lifetime ends.
            // And, if the ISimpleService derives from IDisposable, the Dispose method will also get called at that time.
            shuttleDomain.ReleaseService(service);

            // 在子 AppDomain 中，释放缓存的 ShuttleDomain 实例。这将会注销通过该实例注册的所有服务（在本示例中，即 ISimpleService 服务），
            // 并切断该 AppDomain 与所有 AppDomain 的通信。
            // Releasing the ShuttleDomain instance in the child AppDomain, this will unregister all services registered by that instance,
            // and shut down all communications with all AppDomains.
            serviceProvider.DisposeShuttleDomain();

            Console.Read();
        }
    }
}
```
