using System;
using JointCode.Shuttle.Services;

namespace JoitCode.Shuttle.Sample.Contract
{
    [ServiceInterface]
    public interface ICommonService
    {
        int Age { get; }
        string Name { get; }
        string FirstName { get; }
        string LastName { get; }
        int NameLength { get; }

        void AddName(string name);
        string GetName(int index);
    }

    [ServiceInterface]
    public interface IServiceFunctionTest
    {
        string 中文属性 { get; } // 验证 byVal 属性
        TestData TestData { get; } // 验证 byBin 属性
        string this[int i] { get; set; } // 验证索引器
        int Count { get; }

        void PrintMessage();

        // 空方法
        void CallSimpleMethod();
        int SendAndReturnSimpleValue(int input);
        int SendAndReturnSimpleValue2(string name, int age);
        int SendAndReturnSimpleValue3(string s1, int age, string s2);

        CommonData ReturnByBin();

        BusinessData SendAndReturnByBin(string s1, int i1, CommonData commonData, out TestData testData);

        void SendAndReturnByRef(string s1, int i1, ICommonService serice1, ref ICommonService serice2, out ICommonService serice3);
        void SendAndReturnByVal(int i1, ref int i2, out int i3,
            string s1, ref string s2, out string s3,
            DateTime d1, ref DateTime d2, out DateTime d3,
            Version v1, ref Version v2, out Version v3);

        BusinessData TestAllFunctions(
            string s1, ref string s2, out string s3,
            Version v1, ref Version v2, out Version v3,
            int i1, ref int i2, out int i3,
            CommonData commonData, out CommonData commonData2,
            ICommonService serice1, ref ICommonService serice2, out ICommonService serice3);
    }

//    /// <summary>
//    /// 远程服务功能测试基类
//    /// </summary>
//    /// <seealso cref="MarshalTestRunner" />
//    public abstract class AbstractServiceFunctionTestRunner : MarshalTestRunner
//    {
//        protected ShuttleDomain _shuttleDomain;
//        protected IServiceFunctionTest _shuttleFunctionTest;

//        protected AbstractServiceFunctionTestRunner(int[] assemblyIndexes)
//            : base(assemblyIndexes)
//        { }

//        protected bool DoSetup(string assemblySymbol, ServiceContainer svcContainer)
//        {
//            if (!Initialized)
//                return false;

//            Console.WriteLine("Begin RegisterServices in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);
//            RegisterServices();
//            Console.WriteLine("End RegisterServices in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);

//            Console.WriteLine("Begin creating _shuttleDomain in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);
//            // 初始化
//            _shuttleDomain = ShuttleDomainHelper.Create(assemblySymbol, assemblySymbol, svcContainer ?? new ServiceContainer(true));
//            Console.WriteLine("End creating _shuttleDomain in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);

    //            var guid = Guid.NewGuid();

//            Console.WriteLine("Begin RegisterServiceGroup in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);
//            _shuttleDomain.RegisterServiceGroup(ref guid,
//                new ServiceTypePair(typeof(ICommonService), typeof(CommonService)),
//                new ServiceTypePair(typeof(IServiceFunctionTest), typeof(ServiceFunctionTest)));
//            Console.WriteLine("End RegisterServiceGroup in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);

//            Console.WriteLine("Begin TryGetService in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);
//            _shuttleDomain.TryGetService(out _shuttleFunctionTest);
//            Console.WriteLine("End TryGetService in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);

//            return true;
//        }

//        protected void DoShutDown()
//        {
//            Console.WriteLine("Begin to release service _shuttleFunctionTest...");
//            _shuttleDomain.ReleaseService(_shuttleFunctionTest);
//            Console.WriteLine("End releasing service _shuttleFunctionTest...");

//            Console.WriteLine("Begin to dispose service _shuttleFunctionTest...");
//            _shuttleDomain.Dispose();
//            Console.WriteLine("End disposing service _shuttleFunctionTest...");
//        }

//        protected void CallSimpleMethod(IServiceFunctionTest test)
//        {
//            test.CallSimpleMethod();
//        }
//        protected void SendAndReturnSimpleValue(IServiceFunctionTest test)
//        {
//            var result = test.SendAndReturnSimpleValue(3);
//            if (result != 6)
//                throw new AssertionException();
//        }
//        protected void SendAndReturnSimpleValue2(IServiceFunctionTest test)
//        {
//            var result = test.SendAndReturnSimpleValue2("China", 3);
//            if (result != 8)
//                throw new AssertionException();
//        }
//        protected void SendAndReturnSimpleValue3(IServiceFunctionTest test)
//        {
//            var result = test.SendAndReturnSimpleValue3("China", 3, "HX");
//            if (result != 10)
//                throw new AssertionException();
//        }
//        protected void ReturnByBin(IServiceFunctionTest test)
//        {
//            var result = test.ReturnByBin();
//#if DOASSERT
//            if (result.Code != 1100 || result.Country != "China" || result.TestData.Number != 40 || result.TestData.Message != "Age")
//                throw new AssertionException();
//#endif
//        }
//        protected void SendAndReturnByBin(IServiceFunctionTest test)
//        {
//            TestData output;
//            var testData = new TestData { Message = "Age", Number = 40 };

//            var result = test.SendAndReturnByBin("China", 8,
//                new CommonData { Country = "HX", Code = 86, TestData = testData }, out output);

//#if DOASSERT
//            if (output.Message != "[ChinaAge]" || output.Number != 48
//                || result.EntityId != 94 || result.EntityName != "<HX>")
//                throw new AssertionException();
//#endif
//        }
//        protected void SendAndReturnByVal(IServiceFunctionTest test)
//        {
//            int i3, i2 = 100;
//            string s3, s2 = "Liu";
//            Version v3, v2 = new Version(1, 1, 1, 1), v1 = new Version(2, 2, 2, 2);

//            //Guid g3, g2 = Guid.Empty;
//            //TimeSpan t3, t2 = 10.Seconds();
//            //DateTime d3, d2 = DateTime.Now.AddMonths(1);

//            test.SendAndReturnByVal(40, ref i2, out i3,
//                "Jingyi", ref s2, out s3,
//                //Guid.Empty, ref g2, out g3,
//                //1.Seconds(), ref t2, out t3,
//                //DateTime.Now, ref d2, out d3,
//                v1, ref v2, out v3);

//#if DOASSERT
//            if (i2 != 140 || i3 != 45
//                || s2 != "Liu|40" || s3 != "40|Jingyi"
//                //|| t2.Seconds != 11 || t3.Seconds != 5
//                //|| d2.Seconds != 11 || d3.Seconds != 5
//                || v2.Major != 3 || v2.Minor != 3
//                || v3.Major != 7 || v3.Minor != 7 || v3.Build != 7 || v3.Revision != 7)
//                throw new AssertionException();
//#endif
//        }
//        protected void SendAndReturnByRef(IServiceFunctionTest test)
//        {
//            ICommonService serivice = new CommonService(20, "Jingyi"), service2 = new CommonService(100, "Johnny"), service3;
//            serivice.AddName("Zhongguo");
//            test.SendAndReturnByRef("Liu", 100, serivice, ref service2, out service3);

//#if DOASSERT
//            var age2 = service2.Age;
//            var name2 = service2.Name;
//            var nameLength2 = service2.NameLength;
//            var firstName2 = service2.FirstName;
//            var lastName2 = service2.LastName;

//            var age3 = service3.Age;
//            var name3 = service3.Name;
//            var nameLength3 = service3.NameLength;
//            var firstName3 = service3.FirstName;
//            var lastName3 = service3.LastName;

//            if (age2 != 200 || name2 != "LiuJohnny" || nameLength2 != 5 || firstName2 != "0" || lastName2 != "4"
//                || age3 != 120 || name3 != "LiuJingyi" || nameLength3 != 6 || firstName3 != "Zhongguo" || lastName3 != "4")
//                throw new AssertionException();
//#endif
//        }
//        protected void TestAllFunctions(IServiceFunctionTest test)
//        {
//            string s3, s2 = "s2";
//            Version v3, v2 = new Version(1, 1, 1, 1), v1 = new Version(2, 2, 2, 2);
//            int i3, i2 = 2;

//            var testData = new TestData { Message = "Jingyi", Number = 40 };
//            CommonData commonData = new CommonData { Code = 100, Country = "China", TestData = testData }, commonData2;

//            ICommonService serice = new CommonService(20, "Jingyi"), service2 = new CommonService(), service3;

//            var result = test.TestAllFunctions("s1", ref s2, out s3,
//                v1, ref v2, out v3,
//                1, ref i2, out i3,
//                commonData, out commonData2,
//                serice, ref service2, out service3);

//#if DOASSERT
//            //var age2 = service2.Age;
//            //var name2 = service2.Name;
//            //var nameLength2 = service2.NameLength;
//            //var firstName2 = service2.FirstName;
//            //var lastName2 = service2.LastName;

//            //var age3 = service3.Age;
//            //var name3 = service3.Name;
//            //var nameLength3 = service3.NameLength;
//            //var firstName3 = service3.FirstName;
//            //var lastName3 = service3.LastName;

//            if (s2 != "[s1s2]" || s3 != "<s1>" || i2 != 3 || i3 != 101
//                || v2.Major != 3 || v2.Minor != 3
//                || v3.Major != 3 || v3.Minor != 3 || v3.Build != 3 || v3.Revision != 3
//                || commonData2.Code != 200 || commonData2.Country != "[China]" 
//                || commonData2.TestData.Message != "[Jingyi]" || commonData2.TestData.Number != 140
//                //|| age2 != 41 || name2 != "s1Jingyi" || nameLength2 != 5 || firstName2 != "0" || lastName2 != "4"
//                //|| age3 != 81 || name3 != "[Jingyi]Jingyi" || nameLength3 != 5 || firstName3 != "0" || lastName3 != "4"
//                || result.EntityId != 1500 || result.EntityName != "Fake")
//                throw new AssertionException();
//#endif
//        }
//        protected void TestTemplate1(IServiceFunctionTest test)
//        {
//            ICommonService serivice = new CommonService(20, "Jingyi"), service2 = new CommonService(100, "Johnny");
//            var testData = new TestData { Message = "Jingyi", Number = 40 };
//            test.TestTemplate1("Jingyi", 40, testData, serivice);
//        }


//        protected void ShuttleDomain_CallSimpleMethod()
//        {
//            CallSimpleMethod(_shuttleFunctionTest);
//        }

//        protected void ShuttleDomain_SendAndReturnSimpleValue()
//        {
//            SendAndReturnSimpleValue(_shuttleFunctionTest);
//        }

//        protected void ShuttleDomain_SendAndReturnSimpleValue2()
//        {
//            SendAndReturnSimpleValue2(_shuttleFunctionTest);
//        }

//        protected void ShuttleDomain_SendAndReturnSimpleValue3()
//        {
//            SendAndReturnSimpleValue3(_shuttleFunctionTest);
//        }

//        protected void ShuttleDomain_ReturnByBin()
//        {
//            ReturnByBin(_shuttleFunctionTest);
//        }

//        protected void ShuttleDomain_SendAndReturnByBin()
//        {
//            SendAndReturnByBin(_shuttleFunctionTest);
//        }

//        protected void ShuttleDomain_SendAndReturnByVal()
//        {
//            SendAndReturnByVal(_shuttleFunctionTest);
//        }

//        protected void ShuttleDomain_SendAndReturnByRef()
//        {
//            SendAndReturnByRef(_shuttleFunctionTest);
//        }

//        protected void ShuttleDomain_TestAllFunctions()
//        {
//            TestAllFunctions(_shuttleFunctionTest);
//        }

//        protected void ShuttleDomain_TestTemplate1()
//        {
//            TestTemplate1(_shuttleFunctionTest);
//        }
//    }
}