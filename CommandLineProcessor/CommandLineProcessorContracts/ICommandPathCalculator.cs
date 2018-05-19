namespace CommandLineProcessorContracts
{
    using CommandLineProcessorContracts.Commands;

    public interface ICommandPathCalculator
    {
        string CalculateFullyQualifiedPath(ICommand activeCommand, string input);
    }
}