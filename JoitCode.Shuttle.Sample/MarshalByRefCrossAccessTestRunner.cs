using System;

namespace JoitCode.Shuttle.Sample
{
    public class MarshalByRefCrossAccess1 : MarshalByRefObject
    {
        public void Run()
        {
            Console.Write("Now, we are running in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine();
        }
    }

    public class MarshalByRefCrossAccess2 : MarshalByRefObject
    {
        public void Run(MarshalByRefCrossAccess1 arg)
        {
            Console.WriteLine("Currently, we are running in AppDomain [{0}]: ", AppDomain.CurrentDomain.FriendlyName);
            arg.Run();
        }
    }

    public class MarshalByRefCrossAccessTestRunner : AbstractTestRunner
    {
        AppDomain _remoteDomain;

        public override bool Setup()
        {
            _remoteDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, null);
            return true;
        }

        public override void RunTest()
        {
            var access2 = (MarshalByRefCrossAccess2)_remoteDomain.CreateInstanceAndUnwrap
                (typeof(MarshalByRefCrossAccess2).Assembly.FullName, typeof(MarshalByRefCrossAccess2).FullName);
            var access1 = new MarshalByRefCrossAccess1();
            access2.Run(access1);
        }

        public override void Dispose()
        {
            AppDomain.Unload(_remoteDomain);
        }
    }

    class MarshalByRefCrossAccessTest : Test
    {
        internal MarshalByRefCrossAccessTest()
        {
            Name = "MBO bidirectional commu";
            Description = "MBO bidirectional AppDomain communication";
        }

        internal override void Run()
        {
            var test = new MarshalByRefCrossAccessTestRunner();
            test.Setup();
            test.RunTest();
            test.Dispose();
        }
    }
}