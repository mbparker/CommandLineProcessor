namespace CommandLineProcessorContracts.Events
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts.Commands;

    public class CommandLineHelpEventArgs : EventArgs
    {
        public CommandLineHelpEventArgs(
            ICommandDescriptor commandInfo,
            IEnumerable<ICommandDescriptor> subCommandInfo,
            IDictionary<string, string> syntaxInfo)
        {
            CommandInfo = commandInfo;
            SubCommandInfo = subCommandInfo;
            SyntaxInfo = syntaxInfo;
        }

        public ICommandDescriptor CommandInfo { get; }

        public IEnumerable<ICommandDescriptor> SubCommandInfo { get; }

        public IDictionary<string, string> SyntaxInfo { get; }
    }
}