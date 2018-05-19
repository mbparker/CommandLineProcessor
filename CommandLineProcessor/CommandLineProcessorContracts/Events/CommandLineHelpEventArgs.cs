namespace CommandLineProcessorContracts.Events
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts.Commands;

    public class CommandLineHelpEventArgs : EventArgs
    {
        public CommandLineHelpEventArgs(ICommandDescriptor commandInfo, IEnumerable<ICommandDescriptor> subCommandInfo)
        {
            CommandInfo = commandInfo;
            SubCommandInfo = subCommandInfo;
        }

        public ICommandDescriptor CommandInfo { get; }

        public IEnumerable<ICommandDescriptor> SubCommandInfo { get; }
    }
}