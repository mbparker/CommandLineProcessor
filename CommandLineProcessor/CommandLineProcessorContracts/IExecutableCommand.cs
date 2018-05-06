namespace CommandLineProcessorContracts
{
    public interface IExecutableCommand : ICommand
    {
        void Execute(params object[] args);

        object[] GetArguments();
    }
}