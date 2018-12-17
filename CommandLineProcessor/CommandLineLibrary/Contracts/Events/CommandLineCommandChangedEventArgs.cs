namespace CommandLineLibrary.Contracts.Events
{
    using System;

    using CommandLineLibrary.Contracts.Commands;

    public class CommandLineCommandChangedEventArgs : EventArgs
    {
        public CommandLineCommandChangedEventArgs(ICommand priorCommand, ICommand activeCommand)
        {
            PriorCommand = priorCommand;
            ActiveCommand = activeCommand;
        }

        public ICommand ActiveCommand { get; }

        public ICommand PriorCommand { get; }
    }
}