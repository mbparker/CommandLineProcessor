namespace CommandLineLibrary.Contracts.Commands
{
    public interface IExecutableCommand : ICommand
    {
        void Execute(ICommandContext context);
    }
}