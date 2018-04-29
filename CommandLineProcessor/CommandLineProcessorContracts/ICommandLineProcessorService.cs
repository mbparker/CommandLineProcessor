namespace CommandLineProcessorContracts
{
    using System.Collections.Generic;

    public interface ICommandLineProcessorService
    {
        ICommand ActiveCommand { get; }

        string LastInput { get; }

        CommandLineState State { get; }

        void ProcessInput(string input);

        void RegisterCommands(IEnumerable<ICommand> commands);
    }
}