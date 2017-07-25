#define DOASSERT

using System;
using System.Collections.Generic;
using JointCode.Common;
using JoitCode.Shuttle.Sample.Contract;
using NLite.Test;

namespace JoitCode.Shuttle.Sample
{
    public class CommonService1 : MarshalByRefObject, ICommonService
    {
        readonly List<string> _names;

        public CommonService1()
            : this(40, "Jingyi")
        { }

        public CommonService1(int age, string name)
        {
            Age = age;
            Name = name;
            _names = new List<string>();
        }

        public int Age { get; private set; }
        public string Name { get; private set; }
        public string FirstName { get { return _names[0]; } }
        public string LastName { get { return _names[_names.Count - 1]; } }
        public int NameLength { get { return _names.Count; } }

        public void AddName(string name)
        {
            _names.Add(name);
        }

        public string GetName(int index)
        {
            return _names[index];
        }
    }

    public class ServiceFunctionTest1 : MarshalByRefObject, IServiceFunctionTest
    {
        int _count;
        readonly List<string> _strings = new List<string>(9);

        public string 中文属性
        {
            get { return "这是中文内容哦"; }
        }

        public TestData TestData { get { return new TestData { Message = "TestData.TestMessage", Number = 100 }; } }

        public string this[int i]
        {
            get
            {
                return (i >= _strings.Count) ? null : _strings[i];
            }
            set
            {
                if (i >= _strings.Count)
                    return;
                _strings[i] = value;
            }
        }

        public int Count { get { return _count; } }

        public void PrintMessage()
        {
            Console.WriteLine(this.GetType().Name + " | now we are running in AppDomain [{0}]!", AppDomain.CurrentDomain.FriendlyName);
        }

        public void CallSimpleMethod()
        {
            _count += 1;
        }

        public int SendAndReturnSimpleValue(int input)
        {
            return input + 3;
        }

        public int SendAndReturnSimpleValue2(string name, int age)
        {
            return age + name.Length;
        }

        public int SendAndReturnSimpleValue3(string s1, int age, string s2)
        {
            return age + s1.Length + s2.Length;
        }

        public CommonData ReturnByBin()
        {
            var testData = new TestData { Number = 40, Message = "Age" };
            return new CommonData { Code = 1100, Country = "China", TestData = testData };
        }

        public BusinessData SendAndReturnByBin(string s1, int i1, CommonData commonData, out TestData testData)
        {
            testData = new TestData
            {
                Message = "[" + s1 + commonData.TestData.Message + "]",
                Number = commonData.TestData.Number + i1
            };

            return new BusinessData { EntityId = commonData.Code + i1, EntityName = "<" + commonData.Country + ">" };
        }

        public void SendAndReturnByVal(int i1, ref int i2, out int i3,
            string s1, ref string s2, out string s3,
            DateTime d1, ref DateTime d2, out DateTime d3,
            Version v1, ref Version v2, out Version v3)
        {
            i2 = i1 + i2;
            i3 = i1 + 5;

            s2 = s2 + "|" + i1.ToString();
            s3 = i1.ToString() + "|" + s1;

            d2 = d1;
            d3 = d1.AddYears(1);

            v2 = new Version(v1.Major + v2.Major, v1.Minor + v2.Minor);
            v3 = new Version(v1.Major + 5, v1.Minor + 5, v1.Build + 5, v1.Revision + 5);
        }

        ICommonService DoCreateOutCommonService(string s1, int i1, ICommonService serice)
        {
            var result = new CommonService1(i1 + serice.Age, s1 + serice.Name);
            var length = serice.NameLength;

            // Names 除了原来的内容之外，再加上 0-4 的 5 个数组
            for (int i = 0; i < length; i++)
                result.AddName(serice.GetName(i));
            for (int i = length; i < length + 5; i++)
                result.AddName((i - length).ToString());

            return result;
        }

        ICommonService DoCreateRefCommonService(string s1, int i1, ICommonService serice)
        {
            var result = new CommonService1(i1 + serice.Age, s1 + serice.Name);
            var length = serice.NameLength;

            // Names 除了原来的内容之外，再加上 0-4 的 5 个数组
            for (int i = 0; i < length; i++)
                result.AddName(serice.GetName(i));
            for (int i = length; i < length + 5; i++)
                result.AddName((i - length).ToString());

            return result;
        }

        public void SendAndReturnByRef(string s1, int i1, ICommonService serice1, ref ICommonService serice2, out ICommonService serice3)
        {
            serice2 = DoCreateRefCommonService(s1, i1, serice1);
            serice3 = DoCreateOutCommonService("<" + serice1.Name + ">", i1 + serice1.Age, serice1);
        }

        public BusinessData TestAllFunctions(string s1, ref string s2, out string s3,
            Version v1, ref Version v2, out Version v3,
            int i1, ref int i2, out int i3,
            CommonData commonData, out CommonData commonData2,
            ICommonService serice1, ref ICommonService serice2, out ICommonService serice3)
        {
            s2 = "[" + s1 + s2 + "]";
            s3 = "<" + s1 + ">";

            i2 = i1 + i2;
            i3 = i1 + 100;

            v2 = new Version(v1.Major + v2.Major, v1.Minor + v2.Minor);
            v3 = new Version(v1.Major + 1, v1.Minor + 1, v1.Build + 1, v1.Revision + 1);

            var testData = new TestData { Message = "[" + commonData.TestData.Message + "]", Number = commonData.TestData.Number + 100 };
            commonData2 = new CommonData { Code = commonData.Code + 100, Country = "[" + commonData.Country + "]", TestData = testData };

            var result = new BusinessData { EntityId = 1500, EntityName = "Fake" };

            serice3 = DoCreateOutCommonService("[" + serice1.Name + "]", i1 + serice1.Age, serice1);
            serice2 = DoCreateRefCommonService(s1, i1, serice1);

            return result;
        }
    }

    partial class MarshalByrefObjectPerformanceTestRunner
    {
        protected void CallSimpleMethod(ServiceFunctionTest1 test)
        {
            test.CallSimpleMethod();
        }
        protected void SendAndReturnSimpleValue(ServiceFunctionTest1 test)
        {
            var result = test.SendAndReturnSimpleValue(3);
            if (result != 6)
                throw new AssertionException();
        }
        protected void SendAndReturnSimpleValue2(ServiceFunctionTest1 test)
        {
            var result = test.SendAndReturnSimpleValue2("China", 3);
            if (result != 8)
                throw new AssertionException();
        }
        protected void SendAndReturnSimpleValue3(ServiceFunctionTest1 test)
        {
            var result = test.SendAndReturnSimpleValue3("China", 3, "HX");
            if (result != 10)
                throw new AssertionException();
        }
        protected void ReturnByBin(ServiceFunctionTest1 test)
        {
            var result = test.ReturnByBin();
#if DOASSERT
            if (result.Code != 1100 || result.Country != "China" || result.TestData.Number != 40 || result.TestData.Message != "Age")
                throw new AssertionException();
#endif
        }
        protected void SendAndReturnByBin(ServiceFunctionTest1 test)
        {
            TestData output;
            var testData = new TestData { Message = "Age", Number = 40 };

            var result = test.SendAndReturnByBin("China", 8,
                new CommonData { Country = "HX", Code = 86, TestData = testData }, out output);

#if DOASSERT
            if (output.Message != "[ChinaAge]" || output.Number != 48
                || result.EntityId != 94 || result.EntityName != "<HX>")
                throw new AssertionException();
#endif
        }
        protected void SendAndReturnByVal(ServiceFunctionTest1 test)
        {
            int i3, i2 = 100;
            string s3, s2 = "Liu";

            DateTime d3, d2 = DateTime.Now, d1 = DateTime.MinValue;
            Version v3, v2 = new Version(1, 1, 1, 1), v1 = new Version(2, 2, 2, 2);

            //Guid g3, g2 = Guid.Empty;
            //TimeSpan t3, t2 = 10.Seconds();
            //DateTime d3, d2 = DateTime.Now.AddMonths(1);

            test.SendAndReturnByVal(40, ref i2, out i3,
                "Jingyi", ref s2, out s3,
                //Guid.Empty, ref g2, out g3,
                //1.Seconds(), ref t2, out t3,
                d1, ref d2, out d3,
                v1, ref v2, out v3);

#if DOASSERT
            if (i2 != 140 || i3 != 45
                || s2 != "Liu|40" || s3 != "40|Jingyi"
                //|| t2.Seconds != 11 || t3.Seconds != 5
                //|| d2.Seconds != 11 || d3.Seconds != 5
                || v2.Major != 3 || v2.Minor != 3
                || v3.Major != 7 || v3.Minor != 7 || v3.Build != 7 || v3.Revision != 7)
                throw new AssertionException();
#endif
        }
    }

    public partial class MarshalByrefObjectPerformanceTestRunner : AbstractTestRunner
    {
        int _loopTimes = 100000;
        AppDomain _appDomain;
        ServiceFunctionTest1 _remoteTest, _localTest;

        public override bool Setup()
        {
            _appDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, null);
            _remoteTest = CreateRemoteTestObject();
            _localTest = new ServiceFunctionTest1();
            return true;
        }

        ServiceFunctionTest1 CreateRemoteTestObject()
        {
            return (ServiceFunctionTest1)_appDomain.CreateInstanceAndUnwrap
                (typeof(ServiceFunctionTest1).Assembly.FullName, typeof(ServiceFunctionTest1).FullName);
        }

        public override void RunTest()
        {
            // 热身
            Remote_CreateAndCall();
            Local_CreateAndCall();

            Remote_CallSimpleMethod();
            Local_CallSimpleMethod();

            Remote_SendAndReturnSimpleValue();
            Local_SendAndReturnSimpleValue();

            Remote_SendAndReturnSimpleValue2();
            Local_SendAndReturnSimpleValue2();

            Remote_SendAndReturnSimpleValue3();
            Local_SendAndReturnSimpleValue3();

            Remote_ReturnByBin();
            Local_ReturnByBin();

            Remote_SendAndReturnByBin();
            Local_SendAndReturnByBin();

            Remote_SendAndReturnByVal();
            Local_SendAndReturnByVal();


            // 开始性能测试
            Console.WriteLine();
            Console.WriteLine("测试创建远程服务实例并调用远程服务方法...");
            Console.WriteLine("=========================================================================");

            CodeTimer.Time("Remote_CreateAndCall", Remote_CreateAndCall, _loopTimes);
            CodeTimer.Time("Local_CreateAndCall", Local_CreateAndCall, _loopTimes);

            Console.WriteLine();
            Console.WriteLine("测试复用已创建的远程服务实例并反复调用其服务方法...");
            Console.WriteLine("=========================================================================");

            CodeTimer.Time("Remote_CallSimpleMethod", Remote_CallSimpleMethod, _loopTimes);
            CodeTimer.Time("Local_CallSimpleMethod", Local_CallSimpleMethod, _loopTimes);

            CodeTimer.Time("Remote_SendAndReturnSimpleValue", Remote_SendAndReturnSimpleValue, _loopTimes);
            CodeTimer.Time("Local_SendAndReturnSimpleValue", Local_SendAndReturnSimpleValue, _loopTimes);

            CodeTimer.Time("Remote_SendAndReturnSimpleValue2", Remote_SendAndReturnSimpleValue2, _loopTimes);
            CodeTimer.Time("Local_SendAndReturnSimpleValue2", Local_SendAndReturnSimpleValue2, _loopTimes);

            CodeTimer.Time("Remote_SendAndReturnSimpleValue3", Remote_SendAndReturnSimpleValue3, _loopTimes);
            CodeTimer.Time("Local_SendAndReturnSimpleValue3", Local_SendAndReturnSimpleValue3, _loopTimes);

            CodeTimer.Time("Remote_ReturnByBin", Remote_ReturnByBin, _loopTimes);
            CodeTimer.Time("Local_ReturnByBin", Local_ReturnByBin, _loopTimes);

            CodeTimer.Time("Remote_SendAndReturnByVal", Remote_SendAndReturnByVal, _loopTimes);
            CodeTimer.Time("Local_SendAndReturnByVal", Local_SendAndReturnByVal, _loopTimes);
        }

        public override void Dispose()
        {
            AppDomain.Unload(_appDomain);
        }

        void Remote_CreateAndCall()
        {
            var remoteTest = CreateRemoteTestObject();
            var result = remoteTest.SendAndReturnSimpleValue(3);
            if (result != 6)
                throw new AssertionException();
        }

        void Remote_CallSimpleMethod()
        {
            CallSimpleMethod(_remoteTest);
        }

        void Remote_SendAndReturnSimpleValue()
        {
            SendAndReturnSimpleValue(_remoteTest);
        }

        void Remote_SendAndReturnSimpleValue2()
        {
            SendAndReturnSimpleValue2(_remoteTest);
        }

        void Remote_SendAndReturnSimpleValue3()
        {
            SendAndReturnSimpleValue3(_remoteTest);
        }

        void Remote_ReturnByBin()
        {
            ReturnByBin(_remoteTest);
        }

        void Remote_SendAndReturnByBin()
        {
            SendAndReturnByBin(_remoteTest);
        }

        void Remote_SendAndReturnByVal()
        {
            SendAndReturnByVal(_remoteTest);
        }



        void Local_CreateAndCall()
        {
            var localTest = new ServiceFunctionTest1();
            var result = localTest.SendAndReturnSimpleValue(3);
            if (result != 6)
                throw new AssertionException();
        }

        void Local_CallSimpleMethod()
        {
            CallSimpleMethod(_localTest);
        }

        void Local_SendAndReturnSimpleValue()
        {
            SendAndReturnSimpleValue(_localTest);
        }

        void Local_SendAndReturnSimpleValue2()
        {
            SendAndReturnSimpleValue2(_localTest);
        }

        void Local_SendAndReturnSimpleValue3()
        {
            SendAndReturnSimpleValue3(_localTest);
        }

        void Local_ReturnByBin()
        {
            ReturnByBin(_localTest);
        }

        void Local_SendAndReturnByBin()
        {
            SendAndReturnByBin(_localTest);
        }

        void Local_SendAndReturnByVal()
        {
            SendAndReturnByVal(_localTest);
        }
    }

    class MarshalByObjectPerformanceTest : Test
    {
        internal MarshalByObjectPerformanceTest()
        {
            Name = "MBO Performance Test";
            Description = "MBO in- vs inter-AppDomain commu performance";
        }

        internal override void Run()
        {
            var test = new MarshalByrefObjectPerformanceTestRunner();
            test.Setup();
            test.RunTest();
            test.Dispose();
        }
    }
}