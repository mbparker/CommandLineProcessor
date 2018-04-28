namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public interface ICommandLineProcessorService
    {
        string CurrentInput { get; }

        void ProcessInput(string input);

        void RegisterCommands(IEnumerable<ICommand> commands);
    }
}