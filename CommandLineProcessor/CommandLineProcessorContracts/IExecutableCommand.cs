namespace CommandLineProcessorContracts
{
    public interface IExecutableCommand : ICommand
    {
        void Execute(ICommandContext context);
    }
}