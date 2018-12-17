namespace CommandLineLibrary.Contracts
{
    using System;
    using System.Collections.Generic;

    using CommandLineLibrary.Contracts.Commands;
    using CommandLineLibrary.Contracts.Events;

    using CommandLineLibrary.Models;

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

        ICommandContext ActiveContext { get; }

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