using System;
using System.Collections.Generic;
using JointCode.Common.Extensions;
using JointCode.Shuttle;
using NLite.Test;

namespace JoitCode.Shuttle.Sample
{
    class Program
    {
        const char Tab = '\t';
        static bool _reEnter;
        static List<Test> _tests = new List<Test>(); 

        static void PrintNotification()
        {
            Console.WriteLine("JointCode.Shuttle 简介...");
            Console.WriteLine("=========================================================================");
            Console.WriteLine("JointCode.Shuttle 是一个用于进程内 AppDomain 间通信的服务架构（不支持跨进程）。");
            Console.WriteLine("在进行跨 AppDomain 通信时，很多人使用运行时库默认提供的 MarshalByrefObject 方式。");
            Console.WriteLine("JointCode.Shuttle 与 MarshalByrefObject 主要有两点不同：性能 + 通信方向。");
            Console.WriteLine("在使用 MarshalByrefObject 进行跨 AppDomain 通信时，通常只能从父 AppDomain 访问子 AppDomain，反之则不能。");
            Console.WriteLine("但使用在 JointCode.Shuttle 开发时，父子 AppDomain 之间的通信是双向的。");

            Console.WriteLine();
            Console.WriteLine("环境要求...");
            Console.WriteLine("=========================================================================");
            Console.WriteLine("JointCode.Shuttle 只需要 .net framework 2.0 即可开发和运行，但目前仅支持 x86 目标平台，不支持 x64。");
            Console.WriteLine("本示例程序由于引用了其他组件，需要 .net 2.0 sp2 或 3.5 以上方可运行。");

            Console.WriteLine();
            Console.WriteLine("开发说明...");
            Console.WriteLine("=========================================================================");
            Console.WriteLine("请注意，在测试中我们将远程 IServiceFunctionTest 服务实例保存在测试基类 AbstractTestRunner 的一个字段中，这样做是为了方便起见。");
            Console.WriteLine("在真正的项目开发中，不能把远程对象引用保持到本地实例字段中，因为远程对象的生命期是由远端控制的，一般在一段时间不使用之后就会失效。");
            Console.WriteLine("如果在远程对象失效之后继续调用其方法，则会导致异常。");

            //Console.WriteLine();
            //Console.WriteLine("关于本测试...");
            //Console.WriteLine("=========================================================================");
            //Console.WriteLine("本测试分为 2 部分。我们首先演示了 JointCode.Shuttle 的功能，接着测试了其与 .net remoting 对比的性能。");
            //Console.WriteLine("在本示例中，我们创建了 1 个应用程序集、1 个契约程序集以及 2 个集成了服务提供者和消费者两种角色的程序集。");
        }

        static void Main(string[] args)
        {
            _tests.AddRange(new Test[]
            {
                new ShuttleDomainFunctionalTest(), 
                new ShuttleDomainPerformanceTest(), 
                new MarshalByObjectPerformanceTest(),
                new ShuttleDomainLifeTimeManagementTest(), 
                new ShuttleDomainAnyAppDomainAccessTest(), 
                new MarshalByRefCrossAccessTest(), 
                new ShuttleDomainServiceUpdateTest(), 
            });

            // 显示测试说明
            PrintNotification();

            // 初始化 ShuttleDomain
            ShuttleDomain.Initialize();

            // 初始化性能测量组件
            CodeTimer.Initialize();

            Console.WriteLine();
            Console.WriteLine("Beginning Tests...");
            Console.WriteLine("=========================================================================");

            do
            {
                if (_reEnter)
                {
                    var input = Console.ReadLine();
                    if (String.Equals("x", input, StringComparison.InvariantCultureIgnoreCase))
                        break;
                }

                Console.WriteLine("Test List (SD stands for ShuttleDomain, MBO stands for MarshalByRefObject)");
                Console.WriteLine("=========================================================================");

                var testInfo = BuildTestInfo();
                Console.Write(testInfo);

                Console.WriteLine
                    ("Input the index of test to run:");
                int index;
                var strIndex = Console.ReadLine();
                if (!int.TryParse(strIndex, out index))
                    throw new InvalidOperationException(string.Format("The [{0}] is not a number!", strIndex));
                if (index > _tests.Count - 1)
                    throw new InvalidOperationException(string.Format("The specified index [{0}] is out of range!", strIndex));

                var test = _tests[index];
                test.Run();

                Console.WriteLine();
                var oldForeColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine
                    ("press [x] to exit the tests，or [c] to run other tests.");
                Console.ForegroundColor = oldForeColor;
                Console.WriteLine();

                _reEnter = true;

            } while (true);

            Console.WriteLine();
            Console.WriteLine("Tests completed!");
            Console.Read();
        }

        static string BuildTestInfo()
        {
            var testInfo = "Index" + Tab + "Name" + Tab + Tab + Tab + "Description";
            testInfo += Environment.NewLine + "-------------------------------------------------------------------------";
            for (int i = 0; i < _tests.Count; i++)
            {
                var test = _tests[i];
                testInfo += Environment.NewLine + 
                    i + "." + Tab
                    + test.Name + Tab
                    + test.Description;
            }

            testInfo += Environment.NewLine + "=========================================================================" + Environment.NewLine;
            return testInfo;
        }
    }
}
