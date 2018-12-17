namespace CommandLineLibrary.Contracts
{
    using System.Collections.Generic;

    using CommandLineLibrary.Contracts.Commands;

    public interface ICommandLineInterface
    {
        bool AutomaticHelp { get; set; }

        bool OutputDiagnostics { get; set; }

        bool OutputErrors { get; set; }

        void Run(IEnumerable<ICommand> commandLexicon);
    }
}