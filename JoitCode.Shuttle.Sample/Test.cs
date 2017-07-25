namespace JoitCode.Shuttle.Sample
{
    abstract class Test
    {
        internal string Name { get; set; }
        internal string Description { get; set; }
        internal abstract void Run();
    }
}