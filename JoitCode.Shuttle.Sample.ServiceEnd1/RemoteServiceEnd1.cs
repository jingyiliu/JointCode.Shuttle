using System;
using System.Collections.Generic;
using JointCode.Shuttle.Services;
using JoitCode.Shuttle.Sample.Contract;

namespace JoitCode.Shuttle.Sample.ServiceEnd1
{
    [ServiceClass(typeof(ICommonService), Lifetime = LifetimeEnum.Transient)]
    public class CommonService : MarshalByRefObject, ICommonService
    {
        readonly List<string> _names;

        public CommonService()
            : this(40, "Jingyi")
        { }

        public CommonService(int age, string name)
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

    [ServiceClass(typeof(IServiceFunctionTest), Lifetime = LifetimeEnum.Transient)]
    public class ServiceFunctionTest : MarshalByRefObject, IServiceFunctionTest
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
            var result = new CommonService(i1 + serice.Age, s1 + serice.Name);
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
            var result = new CommonService(i1 + serice.Age, s1 + serice.Name);
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

    public class RemoteServiceEnd1 : RemoteServiceEnd
    {
        public override void RegisterServices()
        {
            var guid = Guid.NewGuid();
            _shuttleDomain.RegisterServiceGroup(ref guid,
                new ServiceTypePair(typeof(ICommonService), typeof(CommonService)),
                new ServiceTypePair(typeof(IServiceFunctionTest), typeof(ServiceFunctionTest)));
        }

        public override void ConsumeServices()
        {
            IFakeService fakeService;
            if (_shuttleDomain.TryGetService(out fakeService))
            {
                Console.WriteLine("AppDomain [{0}], before calling the remote service: ", AppDomain.CurrentDomain.FriendlyName);
                var result = fakeService.PrintAndReturn("JointCode.Shuttle");
                Console.WriteLine("AppDomain [{0}], after calling the remote service with result [{1}] ", AppDomain.CurrentDomain.FriendlyName, result);
                Console.WriteLine();
            }
        }
    }
}
