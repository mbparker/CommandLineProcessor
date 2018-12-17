namespace CommandLineLibrary.Contracts.Events
{
    using System;

    public class CommandLineStatusChangedEventArgs : EventArgs
    {
        public CommandLineStatusChangedEventArgs(CommandLineStatus priorStatus, CommandLineStatus status)
        {
            PriorStatus = priorStatus;
            Status = status;
        }

        public CommandLineStatus PriorStatus { get; }

        public CommandLineStatus Status { get; }
    }
}