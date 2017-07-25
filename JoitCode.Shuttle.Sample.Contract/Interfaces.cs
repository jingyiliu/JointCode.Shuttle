using System;
using JointCode.Serialization;
using JointCode.Shuttle.Services;

namespace JoitCode.Shuttle.Sample.Contract
{
    [ServiceInterface]
    public interface IUpdatableService
    {
        void PrintMessage();
    }

    [ServiceInterface]
    public interface ISimpleService
    {
        void PrintMessage();
        string GetOutput(string input);
    }

    [ServiceInterface]
    public interface IFakeService
    {
        void PrintMessage();
        string PrintAndReturn(string msg);
    }

    [SerializableType, Serializable]
    public class TestData
    {
        public string Message { get; set; }
        public int Number { get; set; }
    }

    [SerializableType, Serializable]
    public class CommonData
    {
        public string Country { get; set; }
        public int Code { get; set; }
        public TestData TestData { get; set; }
    }

    [SerializableType, Serializable]
    public class BusinessData
    {
        public int EntityId { get; set; }
        public string EntityName { get; set; }
    }
}
