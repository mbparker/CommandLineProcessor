namespace CommandLineProcessorContracts.Commands
{
    public interface IExecutableCommand : ICommand
    {
        void Execute(ICommandContext context);
    }
}