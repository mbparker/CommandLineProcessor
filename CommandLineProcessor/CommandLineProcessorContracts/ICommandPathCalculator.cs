namespace CommandLineProcessorContracts
{
    public interface ICommandPathCalculator
    {
        string CalculateFullyQualifiedPath(ICommand activeCommand, string input);
    }
}