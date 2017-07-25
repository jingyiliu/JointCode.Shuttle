//using System;
//using System.Reflection;
//using System.Reflection.Emit;
//using JointCode.Common.Extensions;
//using JointCode.Expressions;
//using JointCode.ServiceInjector;
//using JointCode.Shuttle;

//namespace JoitCode.Shuttle.SimpleSample
//{
//    public static class ShuttleDomainHelper
//    {
//        public static ShuttleDomain Create(string assemblySymbol, string assemblyName)
//        {
//            return Create(assemblySymbol, assemblyName, null);
//        }

//        public static ShuttleDomain Create(string assemblySymbol, string assemblyName, ServiceContainer svContainer)
//        {
//            var dynAsmOptions = new DynamicAssemblyOptions
//            {
//                AccessMode = AssemblyBuilderAccess.Run,
//                AssemblyName = new AssemblyName(assemblyName)
//            };

//            var options = new ShuttleDomainOptions
//            {
//                DynamicAssemblySymbol = assemblySymbol,
//                DynamicAssemblyOptions = dynAsmOptions,
//                DefaultLeaseTime = 10.Seconds(),
//                PollingInterval = 5.Seconds()
//            };

//            try
//            {
//                return ShuttleDomain.Create(ref options, svContainer);
//            }
//            catch(Exception e)
//            {
//                if (e.InnerException != null)
//                    Console.WriteLine(e.InnerException.Message);
//                else
//                    Console.WriteLine(e.Message);
//                return null;
//            }
//        }
//    }
//}