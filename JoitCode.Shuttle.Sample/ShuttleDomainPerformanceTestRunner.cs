#define DOASSERT

using System;
using System.IO;
using System.Reflection;
using JointCode.Common;
using JoitCode.Shuttle.Sample.Contract;
using NLite.Test;

namespace JoitCode.Shuttle.Sample
{
    /// <summary>
    /// 测试与 MarshalByrefObject 相比，Shuttle 的性能
    /// </summary>
    class ShuttleDomainPerformanceTestRunner : ShuttleTestRunner
    {
        static int _loopTimes = 10000;
        static AppDomain _remoteDomain;
        static IServiceFunctionTest _remotingFunctionTest;
        static string _serviceEnd1AsmName;
        RemoteServiceEnd _serviceEnd1;

        static ShuttleDomainPerformanceTestRunner()
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var serviceEnd1Asm = AssemblyName.GetAssemblyName(Path.Combine(currentDir, ServiceEnd1Dll));
            _serviceEnd1AsmName = serviceEnd1Asm.FullName;
        }

        public override bool Setup()
        {
            _remoteDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, null);
            _remotingFunctionTest = CreateRemoteFunctionTestObject();

            _serviceEnd1 = (RemoteServiceEnd)_remoteDomain.CreateInstanceAndUnwrap
                (_serviceEnd1AsmName, ServiceEnd1Type);

            // 创建 ShuttleDomain
            _serviceEnd1.CreateShuttleDomain();

            var key = Guid.NewGuid().ToString();
            _shuttleDomain = ShuttleDomainHelper.Create(key, key);

            // 注册服务
            _serviceEnd1.RegisterServices();

            return true;
        }

        IServiceFunctionTest CreateRemoteFunctionTestObject()
        {
            return (IServiceFunctionTest)_remoteDomain.CreateInstanceAndUnwrap
                (_serviceEnd1AsmName, ServiceFunctionTestType);
        }

        public override void RunTest()
        {
            Console.WriteLine();
            Console.WriteLine("准备进行性能测试...");
            Console.WriteLine("=========================================================================");

            _shuttleDomain.TryGetService(out _shuttleFunctionTest);

            // 热身
            Remoting_CreateAndCall();
            ShuttleDomain_CreateAndCall();

            Remoting_CallSimpleMethod();
            ShuttleDomain_CallSimpleMethod();

            Remoting_SendAndReturnSimpleValue();
            ShuttleDomain_SendAndReturnSimpleValue();

            Remoting_SendAndReturnSimpleValue2();
            ShuttleDomain_SendAndReturnSimpleValue2();

            Remoting_SendAndReturnSimpleValue3();
            ShuttleDomain_SendAndReturnSimpleValue3();

            Remoting_ReturnByBin();
            ShuttleDomain_ReturnByBin();

            Remoting_SendAndReturnByBin();
            ShuttleDomain_SendAndReturnByBin();

            Remoting_SendAndReturnByVal();
            ShuttleDomain_SendAndReturnByVal();


            // 开始性能测试
            Console.WriteLine();
            Console.WriteLine("测试创建远程服务实例并调用远程服务方法...");
            Console.WriteLine("=========================================================================");

            CodeTimer.Time("Remoting_CreateAndCall", Remoting_CreateAndCall, _loopTimes);
            CodeTimer.Time("ShuttleDomain_CreateAndCall", ShuttleDomain_CreateAndCall, _loopTimes);

            Console.WriteLine();
            Console.WriteLine("测试复用已创建的远程服务实例并反复调用其服务方法...");
            Console.WriteLine("=========================================================================");

            CodeTimer.Time("Remoting_CallSimpleMethod", Remoting_CallSimpleMethod, _loopTimes);
            CodeTimer.Time("ShuttleDomain_CallSimpleMethod", ShuttleDomain_CallSimpleMethod, _loopTimes);

            CodeTimer.Time("Remoting_SendAndReturnSimpleValue", Remoting_SendAndReturnSimpleValue, _loopTimes);
            CodeTimer.Time("ShuttleDomain_SendAndReturnSimpleValue", ShuttleDomain_SendAndReturnSimpleValue, _loopTimes);

            CodeTimer.Time("Remoting_SendAndReturnSimpleValue2", Remoting_SendAndReturnSimpleValue2, _loopTimes);
            CodeTimer.Time("ShuttleDomain_SendAndReturnSimpleValue2", ShuttleDomain_SendAndReturnSimpleValue2, _loopTimes);

            CodeTimer.Time("Remoting_SendAndReturnSimpleValue3", Remoting_SendAndReturnSimpleValue3, _loopTimes);
            CodeTimer.Time("ShuttleDomain_SendAndReturnSimpleValue3", ShuttleDomain_SendAndReturnSimpleValue3, _loopTimes);

            CodeTimer.Time("Remoting_ReturnByBin", Remoting_ReturnByBin, _loopTimes);
            CodeTimer.Time("ShuttleDomain_ReturnByBin", ShuttleDomain_ReturnByBin, _loopTimes);

            CodeTimer.Time("Remoting_SendAndReturnByVal", Remoting_SendAndReturnByVal, _loopTimes);
            CodeTimer.Time("ShuttleDomain_SendAndReturnByVal", ShuttleDomain_SendAndReturnByVal, _loopTimes);
        }

        public override void Dispose()
        {
            _shuttleDomain.Dispose();
            AppDomain.Unload(_remoteDomain);
        }

        void ShuttleDomain_CreateAndCall()
        {
            IServiceFunctionTest functionTest;
            _shuttleDomain.TryGetService(out functionTest);
            var result = functionTest.SendAndReturnSimpleValue(3);
            if (result != 6)
                throw new AssertionException();
        }

        void Remoting_CreateAndCall()
        {
            var functionTest = CreateRemoteFunctionTestObject();
            var result = functionTest.SendAndReturnSimpleValue(3);
            if (result != 6)
                throw new AssertionException();
        }

        void Remoting_CallSimpleMethod()
        {
            CallSimpleMethod(_remotingFunctionTest);
        }

        void Remoting_SendAndReturnSimpleValue()
        {
            SendAndReturnSimpleValue(_remotingFunctionTest);
        }

        void Remoting_SendAndReturnSimpleValue2()
        {
            SendAndReturnSimpleValue2(_remotingFunctionTest);
        }

        void Remoting_SendAndReturnSimpleValue3()
        {
            SendAndReturnSimpleValue3(_remotingFunctionTest);
        }

        void Remoting_ReturnByBin()
        {
            ReturnByBin(_remotingFunctionTest);
        }

        void Remoting_SendAndReturnByBin()
        {
            SendAndReturnByBin(_remotingFunctionTest);
        }

        void Remoting_SendAndReturnByVal()
        {
            SendAndReturnByVal(_remotingFunctionTest);
        }
    }

    class ShuttleDomainPerformanceTest : Test
    {
        internal ShuttleDomainPerformanceTest()
        {
            Name = "SD performance test";
            Description = "SD vs MBO cross-AppDomain performance";
        }

        internal override void Run()
        {
            var test = new ShuttleDomainPerformanceTestRunner();
            test.Setup();
            test.RunTest();
            test.Dispose();
        }
    }
}