using System;
using JointCode.Common;
using JointCode.Shuttle;
using JoitCode.Shuttle.Sample.Contract;

namespace JoitCode.Shuttle.Sample
{
    public abstract class ShuttleTestRunner : AbstractTestRunner
    {
        protected ShuttleDomain _shuttleDomain;
        protected IServiceFunctionTest _shuttleFunctionTest;

        protected const string ServiceEnd1Dll = "JoitCode.Shuttle.Sample.ServiceEnd1.dll";
        protected const string ServiceEnd2Dll = "JoitCode.Shuttle.Sample.ServiceEnd2.dll";
        protected const string ServiceEnd1Type = "JoitCode.Shuttle.Sample.ServiceEnd1.RemoteServiceEnd1";
        protected const string ServiceEnd2Type = "JoitCode.Shuttle.Sample.ServiceEnd2.RemoteServiceEnd2";
        protected const string ServiceFunctionTestType = "JoitCode.Shuttle.Sample.ServiceEnd1.ServiceFunctionTest";

        protected void CallSimpleMethod(IServiceFunctionTest test)
        {
            test.CallSimpleMethod();
        }
        protected void SendAndReturnSimpleValue(IServiceFunctionTest test)
        {
            var result = test.SendAndReturnSimpleValue(3);
            if (result != 6)
                throw new AssertionException();
        }
        protected void SendAndReturnSimpleValue2(IServiceFunctionTest test)
        {
            var result = test.SendAndReturnSimpleValue2("China", 3);
            if (result != 8)
                throw new AssertionException();
        }
        protected void SendAndReturnSimpleValue3(IServiceFunctionTest test)
        {
            var result = test.SendAndReturnSimpleValue3("China", 3, "HX");
            if (result != 10)
                throw new AssertionException();
        }
        protected void ReturnByBin(IServiceFunctionTest test)
        {
            var result = test.ReturnByBin();
#if DOASSERT
            if (result.Code != 1100 || result.Country != "China" || result.TestData.Number != 40 || result.TestData.Message != "Age")
                throw new AssertionException();
#endif
        }
        protected void SendAndReturnByBin(IServiceFunctionTest test)
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
        protected void SendAndReturnByVal(IServiceFunctionTest test)
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
            #region 服务端实现
            //i2 = i1 + i2;
            //i3 = i1 + 5;

            //s2 = s2 + "|" + i1.ToString();
            //s3 = i1.ToString() + "|" + s1;

            //d2 = d1;
            //d3 = d1.AddYears(1);

            //v2 = new Version(v1.Major + v2.Major, v1.Minor + v2.Minor);
            //v3 = new Version(v1.Major + 5, v1.Minor + 5, v1.Build + 5, v1.Revision + 5); 
            #endregion
        }

        protected void ShuttleDomain_CallSimpleMethod()
        {
            CallSimpleMethod(_shuttleFunctionTest);
        }

        protected void ShuttleDomain_SendAndReturnSimpleValue()
        {
            SendAndReturnSimpleValue(_shuttleFunctionTest);
        }

        protected void ShuttleDomain_SendAndReturnSimpleValue2()
        {
            SendAndReturnSimpleValue2(_shuttleFunctionTest);
        }

        protected void ShuttleDomain_SendAndReturnSimpleValue3()
        {
            SendAndReturnSimpleValue3(_shuttleFunctionTest);
        }

        protected void ShuttleDomain_ReturnByBin()
        {
            ReturnByBin(_shuttleFunctionTest);
        }

        protected void ShuttleDomain_SendAndReturnByBin()
        {
            SendAndReturnByBin(_shuttleFunctionTest);
        }

        protected void ShuttleDomain_SendAndReturnByVal()
        {
            SendAndReturnByVal(_shuttleFunctionTest);
        }
    }
}