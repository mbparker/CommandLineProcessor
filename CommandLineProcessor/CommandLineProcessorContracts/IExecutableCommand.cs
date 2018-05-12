namespace CommandLineProcessorContracts
{
    public interface IExecutableCommand : ICommand
    {
        void Execute(ICommandContext context, params object[] args);

        object[] GetArguments(ICommandContext context);
    }
}