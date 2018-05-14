namespace CommandLineProcessorContracts
{
    using System;

    public class CommandExecutedEventArgs : EventArgs
    {
        public CommandExecutedEventArgs(ICommand command)
        {
            Command = command;
        }

        public ICommand Command { get; }
    }
}