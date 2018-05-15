namespace CommandLineProcessorContracts
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts.Events;

    using CommandLineProcessorEntity;

    public interface ICommandLineProcessorService
    {
        event EventHandler<CommandLineCommandChangedEventArgs> ActiveCommandChanged;

        event EventHandler<CommandLineErrorEventArgs> CommandRegistrationError;

        event EventHandler<CommandLineProcessInputEventArgs> ProcessingInputElement;

        event EventHandler<CommandLineProcessInputEventArgs> ProcessingRawInput;

        event EventHandler<CommandLineErrorEventArgs> ProcessInputError;

        event EventHandler<CommandLineStatusChangedEventArgs> StatusChanged;

        event EventHandler<CommandLineHelpEventArgs> HelpRequest;

        ICommand ActiveCommand { get; }

        ICommandHistoryService HistoryService { get; }

        string LastInput { get; }

        string LastRawInput { get; }

        CommandLineSettings Settings { get; set; }

        int StackDepth { get; }

        CommandLineStatus Status { get; }

        void ProcessInput(string input);

        void RegisterCommands(IEnumerable<ICommand> commands);
    }
}