namespace CommandLineLibrary.Contracts
{
    using CommandLineLibrary.Contracts.Commands;

    public interface ICommandPathCalculator
    {
        string CalculateFullyQualifiedPath(ICommand activeCommand, string input);
    }
}