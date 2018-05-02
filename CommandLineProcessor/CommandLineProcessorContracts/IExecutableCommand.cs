namespace CommandLineProcessorContracts
{
    public interface IExecutableCommand : ICommand
    {
        object[] GetArgs();

        void Execute(params object[] args);
    }
}